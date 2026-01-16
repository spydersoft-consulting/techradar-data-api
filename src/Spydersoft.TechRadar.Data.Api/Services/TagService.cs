using Microsoft.EntityFrameworkCore;
using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Models.Dto;
using Spydersoft.TechRadar.Data.Api.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Spydersoft.TechRadar.Data.Api.Services;

/// <summary>
/// Class TagService.
/// Implements the <see cref="Spydersoft.TechRadar.Data.Api.Services.ITagService" />
/// </summary>
/// <seealso cref="Spydersoft.TechRadar.Data.Api.Services.ITagService" />
/// <remarks>
/// Initializes a new instance of the <see cref="TagService"/> class.
/// </remarks>
/// <param name="context">The context.</param>
/// <param name="telemetry">The telemetry.</param>
public class TagService(TechRadarContext context, TechRadarTelemetryMetrics telemetry) : ITagService
{
    private readonly TechRadarContext _context = context;
    private readonly TechRadarTelemetryMetrics _telemetry = telemetry;

    #region ITagService Functions

    /// <summary>
    /// Gets the tags for radar.
    /// </summary>
    /// <param name="radarId">The radar identifier.</param>
    /// <returns>List&lt;SimpleTag&gt;.</returns>
    public List<ItemTag> GetAllRadarTags(int radarId)
    {
        using var activity = _telemetry.StartActivity("TagService.GetAllRadarTags", ActivityKind.Server);
        activity?.SetTag("radar.id", radarId);

        try
        {
            var tags = new List<ItemTag>();

            foreach (var tag in _context.Tags
                .Where(t => t.RadarId == radarId))
            {
                if (tags.All(t => t.TagId != tag.Id))
                {
                    tags.Add(new ItemTag
                    {
                        Name = tag.Name,
                        TagId = tag.Id
                    });
                }
            }

            var result = tags.OrderBy(t => t.Name).ToList();
            
            // Record metrics
            _telemetry.RecordTagsRetrieved(result.Count, new TagList
            {
                { "radar.id", radarId },
                { "operation", "GetAllRadarTags" }
            });

            activity?.SetTag("tags.count", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetAllRadarTags", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the radar tags for item.
    /// </summary>
    /// <param name="radarItemId">The radar item identifier.</param>
    /// <returns>List&lt;RadarItemTag&gt;.</returns>
    public List<RadarItemTag> GetRadarTagsForItem(int radarItemId)
    {
        using var activity = _telemetry.StartActivity("TagService.GetRadarTagsForItem", ActivityKind.Server);
        activity?.SetTag("radar.item.id", radarItemId);

        try
        {
            var result = _context.RadarItemTags.Where(rit => rit.RadarItemId == radarItemId).ToList();
            
            // Record metrics
            _telemetry.RecordTagsRetrieved(result.Count, new TagList
            {
                { "radar.item.id", radarItemId },
                { "operation", "GetRadarTagsForItem" }
            });

            activity?.SetTag("tags.count", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetRadarTagsForItem", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Saves the tag.
    /// </summary>
    /// <param name="radarItemId">The radar item identifier.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="userPrincipal">The user principal.</param>
    public void SaveRadarItemTag(int radarItemId, ItemTag tag, ClaimsPrincipal userPrincipal)
    {
        using var activity = _telemetry.StartActivity("TagService.SaveRadarItemTag", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();
        var isNewTag = tag.TagId == 0;

        activity?.SetTag("radar.item.id", radarItemId);
        activity?.SetTag("tag.id", tag.TagId);
        activity?.SetTag("tag.name", tag.Name);
        activity?.SetTag("is.new.tag", isNewTag);
        activity?.SetTag("user.id", userPrincipal?.Identity?.Name);

        try
        {
            var item = _context.RadarItems
                .Include(radarItem => radarItem.Tags)
                .FirstOrDefault(ri => ri.Id == radarItemId);

            if (item == null)
            {
                _telemetry.RecordError("SaveRadarItemTag.ItemNotFound");
                _telemetry.RecordValidationFailure(TechRadarTelemetryMetrics.ToTags(new TagList
                {
                    { "reason", "ItemNotFound" },
                    { "radar.item.id", radarItemId }
                }));
                activity?.SetTag("item.found", false);
                return;
            }

            activity?.SetTag("item.found", true);
            activity?.SetTag("radar.id", item.RadarId);

            if (tag.TagId != 0)
            {
                // if the tag id passed in does not match a tag in the system
                if (_context.Tags.All(t => t.RadarId == item.RadarId && t.Id == tag.TagId))
                {
                    _telemetry.RecordError("SaveRadarItemTag.TagNotFound");
                    _telemetry.RecordValidationFailure(TechRadarTelemetryMetrics.ToTags(new TagList
                    {
                        { "reason", "TagNotFound" },
                        { "tag.id", tag.TagId }
                    }));
                    activity?.SetTag("validation.error", "TagNotFound");
                    return;
                }

                // verify the desired tag doesn't already exist on the item
                var dataTag = item.Tags?.FirstOrDefault(t => t.TagId == tag.TagId);
                if (dataTag != null)
                {
                    _telemetry.RecordError("SaveRadarItemTag.TagAlreadyExists");
                    _telemetry.RecordValidationFailure(TechRadarTelemetryMetrics.ToTags(new TagList
                    {
                        { "reason", "TagAlreadyExists" },
                        { "tag.id", tag.TagId },
                        { "radar.item.id", radarItemId }
                    }));
                    activity?.SetTag("validation.error", "TagAlreadyExists");
                    return;
                }
            }
            else
            {
                // The incoming tag needs added to the radar
                var newTag = new Tag { Name = tag.Name, Description = tag.Name, RadarId = item.RadarId };
                _context.Tags.Add(newTag);
                _context.SaveChangesWithAudit(userPrincipal?.Identity?.Name);
                tag.TagId = newTag.Id;

                // Record new tag creation
                _telemetry.RecordTagCreated(TechRadarTelemetryMetrics.ToTags(new TagList
                {
                    { "radar.id", item.RadarId },
                    { "tag.name", tag.Name }
                }));

                activity?.SetTag("new.tag.created", true);
                activity?.SetTag("new.tag.id", tag.TagId);
            }

            var radarItemTag = new RadarItemTag { TagId = tag.TagId, RadarItemId = radarItemId };
            _context.RadarItemTags.Add(radarItemTag);
            _context.SaveChangesWithAudit(userPrincipal?.Identity?.Name);

            // Record tag assignment
            _telemetry.RecordTagAssigned(TechRadarTelemetryMetrics.ToTags(new TagList
            {
                { "radar.item.id", radarItemId },
                { "tag.id", tag.TagId }
            }));

            activity?.SetTag("tag.assigned", true);
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("SaveRadarItemTag", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordServiceOperationDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "operation", "SaveRadarItemTag" }
            });
        }
    }

    /// <summary>
    /// Removes the radar item tag.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="tagId">The tag identifier.</param>
    /// <param name="user">The user.</param>
    public void RemoveRadarItemTag(int id, int tagId, ClaimsPrincipal user)
    {
        using var activity = _telemetry.StartActivity("TagService.RemoveRadarItemTag", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();

        activity?.SetTag("radar.item.id", id);
        activity?.SetTag("tag.id", tagId);
        activity?.SetTag("user.id", user?.Identity?.Name);

        try
        {
            var item = _context.RadarItems
                .Include(radarItem => radarItem.Tags)
                .FirstOrDefault(ri => ri.Id == id);

            var tag = item?.Tags?.FirstOrDefault(t => t.TagId == tagId);
            if (tag == null)
            {
                _telemetry.RecordError("RemoveRadarItemTag.TagNotFound");
                activity?.SetTag("tag.found", false);
                return;
            }

            activity?.SetTag("tag.found", true);
            
            _context.RadarItemTags.Remove(tag);
            _context.SaveChangesWithAudit(user.Identity?.Name);

            // Record tag removal
            _telemetry.RecordTagRemoved(TechRadarTelemetryMetrics.ToTags(new TagList
            {
                { "radar.item.id", id },
                { "tag.id", tagId }
            }));

            activity?.SetTag("tag.removed", true);
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("RemoveRadarItemTag", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordServiceOperationDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "operation", "RemoveRadarItemTag" }
            });
        }
    }

    #endregion Tag Functions
}
