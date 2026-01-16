using Microsoft.EntityFrameworkCore;
using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Models;
using Spydersoft.TechRadar.Data.Api.Telemetry;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Spydersoft.TechRadar.Data.Api.Services;


/// <summary>
/// Class RadarDataService.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RadarDataItemService" /> class.
/// </remarks>
/// <param name="context">The context.</param>
/// <param name="telemetry">The telemetry.</param>
public class RadarDataItemService(TechRadarContext context, TechRadarTelemetryMetrics telemetry) : IRadarDataItemService
{
    #region Private Properties

    /// <summary>
    /// The context
    /// </summary>
    private readonly TechRadarContext _context = context;

    /// <summary>
    /// The telemetry
    /// </summary>
    private readonly TechRadarTelemetryMetrics _telemetry = telemetry;

    #endregion Private Properties
    #region Constructor

    #endregion Constructor

    #region IRadarDataItemService Implementation

    /// <summary>
    /// Loads the radar data item.
    /// </summary>
    /// <typeparam name="TRadarDataItem">The type of the t radar data item.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <returns>TRadarDataItem.</returns>
    public TRadarDataItem? GetRadarDataItem<TRadarDataItem>(int id) where TRadarDataItem : class, IRadarDataItem
    {
        using var activity = _telemetry.StartActivity("RadarDataItemService.GetRadarDataItem", ActivityKind.Server);
        activity?.SetTag("item.type", typeof(TRadarDataItem).Name);
        activity?.SetTag("item.id", id);

        try
        {
            return _context.Set<TRadarDataItem>().FirstOrDefault(rdi => rdi.Id == id);
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetRadarDataItem", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Saves the radar data item.
    /// </summary>
    /// <typeparam name="TRadarDataItem">The type of the t radar data item.</typeparam>
    /// <param name="item">The item.</param>
    /// <param name="userPrincipal">The user principal.</param>
    public void SaveRadarDataItem<TRadarDataItem>(TRadarDataItem item, ClaimsPrincipal userPrincipal) where TRadarDataItem : class, IRadarDataItem
    {
        using var activity = _telemetry.StartActivity("RadarDataItemService.SaveRadarDataItem", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();
        var itemType = typeof(TRadarDataItem).Name;
        var isCreate = item.Id == 0;

        activity?.SetTag("item.type", itemType);
        activity?.SetTag("item.id", item.Id);
        activity?.SetTag("operation.type", isCreate ? "create" : "update");
        activity?.SetTag("user.id", userPrincipal?.Identity?.Name);

        try
        {
            var existing = GetRadarDataItem<TRadarDataItem>(item.Id);
            if (existing != null)
            {
                if (existing is RadarItem existingRadarItem)
                {
                    var newRadarItem = item as RadarItem;
                    var oldArcId = existingRadarItem.ArcId;
                    var newArcId = newRadarItem?.ArcId ?? 0;
                    var movementDirection = GetMovementDirection(oldArcId, newArcId);
                    
                    existingRadarItem.MovementDirection = movementDirection;

                    // Record movement direction metric
                    if (movementDirection != 0)
                    {
                        _telemetry.RecordItemMovementDirection(movementDirection, TechRadarTelemetryMetrics.ToTags(new TagList
                        {
                            { "item.id", existingRadarItem.Id },
                            { "old.arc.id", oldArcId },
                            { "new.arc.id", newArcId }
                        }));
                        
                        activity?.SetTag("movement.direction", movementDirection);
                    }
                }
                
                foreach (var property in typeof(TRadarDataItem).GetProperties().Where(p => p.CanWrite))
                {
                    if (property.Name == nameof(item.Id)
                        || property.Name == nameof(RadarItem.DateCreated)
                        || property.Name == nameof(RadarItem.MovementDirection))
                    {
                        continue;
                    }

                    if (property.Name == nameof(RadarItem.DateUpdated))
                    {
                        property.SetValue(existing, DateTime.UtcNow, null);
                    }
                    else
                    {
                        property.SetValue(existing, property.GetValue(item, null), null);
                    }
                }

                _context.Set<TRadarDataItem>().Update(existing);
                
                // Record update metric
                _telemetry.RecordItemUpdated(TechRadarTelemetryMetrics.ToTags(new TagList
                {
                    { "item.type", itemType }
                }));
            }
            else
            {
                foreach (var property in typeof(TRadarDataItem).GetProperties().Where(p => p.CanWrite))
                {
                    if (property.Name == nameof(RadarItem.DateUpdated) || property.Name == nameof(RadarItem.DateCreated))
                    {
                        property.SetValue(item, DateTime.UtcNow, null);
                    }
                }
                _context.Set<TRadarDataItem>().Add(item);
                
                // Record create metric
                _telemetry.RecordItemCreated(TechRadarTelemetryMetrics.ToTags(new TagList
                {
                    { "item.type", itemType }
                }));
            }

            _context.SaveChangesWithAudit(userPrincipal?.Identity?.Name);

            if (item is Radar radar && existing == null)
            {
                using var defaultActivity = _telemetry.StartActivity("RadarDataItemService.AddDefaultRingsAndQuadrants", ActivityKind.Internal);
                defaultActivity?.SetTag("radar.id", radar.Id);
                
                AddDefaultRingsAndQuadrants(radar);
                _context.SaveChangesWithAudit(userPrincipal?.Identity?.Name);
                
                defaultActivity?.SetTag("quadrants.created", 4);
                defaultActivity?.SetTag("arcs.created", 4);
            }

            if (item is RadarItem radarItem && !string.IsNullOrWhiteSpace(radarItem.Note))
            {
                AddNote(radarItem.Id, radarItem.Note, userPrincipal?.Identity?.Name);
                _context.SaveChangesWithAudit(userPrincipal?.Identity?.Name);
                
                _telemetry.RecordNoteCreated(TechRadarTelemetryMetrics.ToTags(new TagList
                {
                    { "radar.item.id", radarItem.Id }
                }));
                
                activity?.SetTag("note.added", true);
            }
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("SaveRadarDataItem", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordServiceOperationDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "operation", "SaveRadarDataItem" },
                { "item.type", itemType }
            });
        }
    }

    /// <summary>
    /// Deletes the radar data item.
    /// </summary>
    /// <typeparam name="TRadarDataItem">The type of the t radar data item.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <param name="userPrincipal">The user principal.</param>
    public void DeleteRadarDataItem<TRadarDataItem>(int id, ClaimsPrincipal userPrincipal) where TRadarDataItem : class, IRadarDataItem
    {
        using var activity = _telemetry.StartActivity("RadarDataItemService.DeleteRadarDataItem", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();
        var itemType = typeof(TRadarDataItem).Name;

        activity?.SetTag("item.type", itemType);
        activity?.SetTag("item.id", id);
        activity?.SetTag("user.id", userPrincipal?.Identity?.Name);

        try
        {
            var existing = GetQueryableForDelete<TRadarDataItem>()?.FirstOrDefault(a => a.Id == id);
            if (existing == null)
            {
                _telemetry.RecordError("DeleteRadarDataItem.NotFound");
                activity?.SetTag("item.found", false);
                return;
            }

            activity?.SetTag("item.found", true);
            
            _context.Set<TRadarDataItem>().Remove(existing);
            _context.SaveChangesWithAudit(userPrincipal?.Identity?.Name);
            
            // Record delete metric
            _telemetry.RecordItemDeleted(TechRadarTelemetryMetrics.ToTags(new TagList
            {
                { "item.type", itemType }
            }));
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("DeleteRadarDataItem", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordServiceOperationDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "operation", "DeleteRadarDataItem" },
                { "item.type", itemType }
            });
        }
    }

    #endregion Generic Methods



    #region Note Functions

    /// <summary>
    /// Gets the notes.
    /// </summary>
    /// <param name="radarItemId">The radar item identifier.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>PagedList&lt;RadarItemNote&gt;.</returns>
    public PagedList<RadarItemNote> GetNotes(int radarItemId, QueryParameters parameters)
    {
        using var activity = _telemetry.StartActivity("RadarDataItemService.GetNotes", ActivityKind.Server);
        activity?.SetTag("radar.item.id", radarItemId);
        activity?.SetTag("page", parameters.Page);
        activity?.SetTag("page.size", parameters.PageSize);

        try
        {
            var result = PagedList<RadarItemNote>.ToPagedList(
                _context.RadarItemNotes.Where(note => note.RadarItemId == radarItemId)
                    .OrderByDescending(note => note.DateUpdated), parameters.Page, parameters.PageSize);
            
            // Record metrics
            _telemetry.RecordNotesRetrieved(result.Count, new TagList
            {
                { "radar.item.id", radarItemId }
            });

            _telemetry.RecordPaginationPageSize(parameters.PageSize, new TagList
            {
                { "operation", "GetNotes" }
            });

            _telemetry.RecordPaginationTotalRecords(result.TotalCount, new TagList
            {
                { "operation", "GetNotes" },
                { "radar.item.id", radarItemId }
            });

            activity?.SetTag("notes.count", result.Count);
            activity?.SetTag("total.count", result.TotalCount);
            activity?.SetTag("total.pages", result.TotalPages);

            return result;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetNotes", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    #endregion

    #region Private Support Functions

    private IQueryable<TRadarDataItem>? GetQueryableForDelete<TRadarDataItem>() where TRadarDataItem : class, IRadarDataItem
    {
        if (typeof(TRadarDataItem) == typeof(RadarItem))
        {
            return _context.RadarItems.Include(ri => ri.Tags) as IQueryable<TRadarDataItem>;
        }

        return _context.Set<TRadarDataItem>();
    }


    private int GetMovementDirection(int previousArcId, int newArcId)
    {
        if (previousArcId == newArcId)
        {
            return 0;
        }

        var prevArc = _context.RadarArcs.FirstOrDefault(arc => arc.Id == previousArcId);
        var newArc = _context.RadarArcs.FirstOrDefault(arc => arc.Id == newArcId);

        if (prevArc == null || newArc == null)
        {
            return 0;
        }

        // Negative numbers move towards the center, positive numbers move outside
        return (newArc.Position - prevArc.Position);
    }

    private void AddDefaultRingsAndQuadrants(Radar radar)
    {
        for (int quadIndex = 1; quadIndex <= 4; ++quadIndex)
        {
            var quad = new Quadrant()
            {
                Name = $"Quadrant {quadIndex}",
                Position = quadIndex,
                RadarId = radar.Id,
                Color = "#000000"
            };
            _context.Quadrants.Add(quad);
        }

        for (int arcIndex = 1; arcIndex <= 4; ++arcIndex)
        {
            var arc = new RadarArc()
            {
                Name = $"Ring {arcIndex}",
                Position = arcIndex,
                RadarId = radar.Id,
                Radius = 15,
                Color = "#000000"
            };
            _context.RadarArcs.Add(arc);
        }
    }

    private void AddNote(int radarItemId, string radarItemNote, string? identityName)
    {
        var note = new RadarItemNote
        {
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            RadarItemId = radarItemId,
            Notes = radarItemNote,
            UserId = identityName ?? "UnknownUser"
        };
        _context.RadarItemNotes.Add(note);
    }

    #endregion Private Support Functions
}