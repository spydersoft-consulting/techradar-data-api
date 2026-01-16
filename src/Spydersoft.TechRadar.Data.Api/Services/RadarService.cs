using Microsoft.EntityFrameworkCore;
using Spydersoft.TechRadar.Data.Api.Data;
using Spydersoft.TechRadar.Data.Api.Models;
using Spydersoft.TechRadar.Data.Api.Models.RadarViewObjects;
using Spydersoft.TechRadar.Data.Api.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Spydersoft.TechRadar.Data.Api.Services;

/// <summary>
/// Class RadarService.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RadarService"/> class.
/// </remarks>
/// <param name="context">The context.</param>
/// <param name="telemetry">The telemetry.</param>
public class RadarService(TechRadarContext context, TechRadarTelemetryMetrics telemetry) : IRadarService
{
    /// <summary>
    /// The context
    /// </summary>
    private readonly TechRadarContext _context = context;
    
    /// <summary>
    /// The telemetry
    /// </summary>
    private readonly TechRadarTelemetryMetrics _telemetry = telemetry;

    /// <summary>
    /// Gets the radar list.
    /// </summary>
    /// <returns>List&lt;Radar&gt;.</returns>
    public async Task<List<Radar>> GetRadarList()
    {
        using var activity = _telemetry.StartActivity("RadarService.GetRadarList", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var radars = await _context.Radars.ToListAsync();
            
            // Record metrics
            _telemetry.RecordRadarRetrieved(new TagList
            {
                { "radar.count", radars.Count }
            });

            activity?.SetTag("radar.count", radars.Count);
            
            return radars;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetRadarList", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordServiceOperationDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "operation", "GetRadarList" }
            });
        }
    }

    /// <summary>
    /// Loads the radar data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="filterParameters">The filter parameters.</param>
    /// <returns>RadarData.</returns>
    public RadarData? LoadRadarData(int id, FilterParameters filterParameters)
    {
        using var activity = _telemetry.StartActivity("RadarService.LoadRadarData", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();

        activity?.SetTag("radar.id", id);
        activity?.SetTag("filter.tags", filterParameters.Tags?.Length ?? 0);
        activity?.SetTag("filter.days", filterParameters.UpdatedWithinDays);

        try
        {
            var radar = _context.Radars.FirstOrDefault(r => r.Id == id);

            if (radar == null)
            {
                _telemetry.RecordError("RadarNotFound");
                activity?.SetStatus(ActivityStatusCode.Error, "Radar not found");
                return null;
            }

            var data = new RadarData
            {
                Height = 1400,
                Width = 1400,
                Title = radar.Title,
                Colors = new ColorSettings
                {
                    Background = radar.BackgroundColor,
                    Grid = radar.GridlineColor,
                    Inactive = radar.InactiveColor
                }
            };

            // Track unique colors for color diversity metric
            var uniqueColors = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(radar.BackgroundColor)) uniqueColors.Add(radar.BackgroundColor);
            if (!string.IsNullOrWhiteSpace(radar.GridlineColor)) uniqueColors.Add(radar.GridlineColor);
            if (!string.IsNullOrWhiteSpace(radar.InactiveColor)) uniqueColors.Add(radar.InactiveColor);

            var ringIndex = new Dictionary<int, int>();
            var index = 0;
            foreach (var arc in _context.RadarArcs.Where(ra => ra.RadarId == id).OrderBy(ra => ra.Position))
            {
                data.Rings.Add(new RadarRing { Name = arc.Name, Color = arc.Color, Radius = arc.Radius });
                ringIndex[arc.Id] = index++;
                if (!string.IsNullOrWhiteSpace(arc.Color)) uniqueColors.Add(arc.Color);
            }

            var quadrantIndex = new Dictionary<int, int>();
            index = 0;
            foreach (var quad in _context.Quadrants.Where(qd => qd.RadarId == id).OrderBy(qd => qd.Position))
            {
                data.Quadrants.Add(new RadarQuadrant { Name = quad.Name, Color = quad.Color });
                quadrantIndex[quad.Id] = index++;
                if (!string.IsNullOrWhiteSpace(quad.Color)) uniqueColors.Add(quad.Color);
            }

            // Track filter application
            if (filterParameters.Tags?.Length > 0 || filterParameters.UpdatedWithinDays > 0)
            {
                var filterTags = new TagList { { "radar.id", id } };
                if (filterParameters.Tags?.Length > 0)
                {
                    filterTags.Add("filter.type", "tags");
                    filterTags.Add("tag.count", filterParameters.Tags.Length);
                }
                if (filterParameters.UpdatedWithinDays > 0)
                {
                    filterTags.Add("filter.type", "date");
                    filterTags.Add("days", filterParameters.UpdatedWithinDays);
                }
                
                
                _telemetry.RecordRadarFilterApplied(filterTags);
            }

            var cutoffDate = DateTime.MinValue;
            if (filterParameters.UpdatedWithinDays > 0)
            {
                cutoffDate = DateTime.UtcNow.AddDays(-1 * filterParameters.UpdatedWithinDays);
            }

            var itemCount = 0;
            var customLegendKeyCount = 0;
            var autoLegendKeyCount = 0;
            var itemsWithUrl = 0;
            var itemsWithoutUrl = 0;
            var quadrantDistribution = new Dictionary<int, int>();
            var arcDistribution = new Dictionary<int, int>();

            foreach (var radarItem in _context.RadarItems
                .Include(radarItem => radarItem.Tags)
                .Where(ri => ri.RadarId == id && (filterParameters.Tags == null || ri.Tags.Any(t => filterParameters.Tags.Contains(t.TagId)))
                             && (ri.DateCreated > cutoffDate || ri.DateUpdated > cutoffDate)))
            {
                data.Entries.Add(new RadarEntry
                {
                    Active = true,
                    LegendKey = string.IsNullOrWhiteSpace(radarItem.LegendKey) ? (radarItem.Name.Length > 15 ? radarItem.Name.Substring(0, 15) : radarItem.Name) : radarItem.LegendKey,
                    Label = radarItem.Name,
                    Link = radarItem.Url,
                    Moved = radarItem.DateUpdated > DateTime.UtcNow.AddDays(-90) ? radarItem.MovementDirection : 0,
                    Quadrant = quadrantIndex[radarItem.QuadrantId],
                    Ring = ringIndex[radarItem.ArcId],
                    IsNew = radarItem.DateCreated > DateTime.UtcNow.AddDays(-90)
                });

                itemCount++;

                // Demonstrative metrics collection
                _telemetry.RecordItemNameLength(radarItem.Name.Length, new TagList { { "radar.id", id } });

                if (string.IsNullOrWhiteSpace(radarItem.LegendKey))
                {
                    autoLegendKeyCount++;
                }
                else
                {
                    customLegendKeyCount++;
                }

                if (string.IsNullOrWhiteSpace(radarItem.Url))
                {
                    itemsWithoutUrl++;
                }
                else
                {
                    itemsWithUrl++;
                }

                // Track item age and staleness
                var ageDays = (DateTime.UtcNow - radarItem.DateCreated).Days;
                var stalenessDays = (DateTime.UtcNow - radarItem.DateUpdated).Days;
                
                _telemetry.RecordItemAge(ageDays, new TagList { { "radar.id", id } });
                _telemetry.RecordItemStaleness(stalenessDays, new TagList { { "radar.id", id } });

                // Track distribution
                quadrantDistribution.TryGetValue(radarItem.QuadrantId, out var qCount);
                quadrantDistribution[radarItem.QuadrantId] = qCount + 1;

                arcDistribution.TryGetValue(radarItem.ArcId, out var aCount);
                arcDistribution[radarItem.ArcId] = aCount + 1;
            }

            // Record comprehensive metrics
            _telemetry.RecordRadarItemsLoaded(itemCount, new TagList { { "radar.id", id } });
            _telemetry.RecordRadarEntriesGenerated(data.Entries.Count, new TagList { { "radar.id", id } });

            // Calculate and record complexity score
            var complexityScore = data.Entries.Count * data.Quadrants.Count * data.Rings.Count;
            _telemetry.RecordRadarComplexityScore(complexityScore, new TagList
            {
                { "radar.id", id },
                { "items", data.Entries.Count },
                { "quadrants", data.Quadrants.Count },
                { "rings", data.Rings.Count }
            });

            // Record demonstrative metrics
            _telemetry.RecordLegendKeyType(customLegendKeyCount, new TagList
            {
                { "radar.id", id },
                { "type", "custom" }
            });

            _telemetry.RecordLegendKeyType(autoLegendKeyCount, new TagList
            {
                { "radar.id", id },
                { "type", "auto" }
            });

            _telemetry.RecordColorDiversity(uniqueColors.Count, new TagList
            {
                { "radar.id", id }
            });

            _telemetry.RecordUrlPresence(itemsWithUrl, new TagList
            {
                { "radar.id", id },
                { "has.url", "true" }
            });

            _telemetry.RecordUrlPresence(itemsWithoutUrl, new TagList
            {
                { "radar.id", id },
                { "has.url", "false" }
            });

            // Record distribution metrics
            foreach (var kvp in quadrantDistribution)
            {
                _telemetry.RecordQuadrantDistribution(kvp.Value, new TagList
                {
                    { "radar.id", id },
                    { "quadrant.id", kvp.Key }
                });
            }

            foreach (var kvp in arcDistribution)
            {
                _telemetry.RecordArcDistribution(kvp.Value, new TagList
                {
                    { "radar.id", id },
                    { "arc.id", kvp.Key }
                });
            }

            activity?.SetTag("entries.count", data.Entries.Count);
            activity?.SetTag("complexity.score", complexityScore);
            activity?.SetTag("color.diversity", uniqueColors.Count);

            return data;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("LoadRadarData", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordRadarDataLoadDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "radar.id", id }
            });
        }
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>ActionResult&lt;List&lt;RadarItem&gt;&gt;.</returns>
    public List<RadarItem> GetItems(int id, RadarQueryParameters parameters)
    {
        using var activity = _telemetry.StartActivity("RadarService.GetItems", ActivityKind.Server);
        var stopwatch = Stopwatch.StartNew();

        activity?.SetTag("radar.id", id);
        activity?.SetTag("filter.arc.id", parameters.ArcId);
        activity?.SetTag("filter.quadrant.id", parameters.QuadrantId);
        activity?.SetTag("filter.tag.id", parameters.TagId);

        try
        {
            var query = _context.RadarItems.Where(ri => ri.RadarId == id);

            if (parameters.ArcId.HasValue && parameters.ArcId > 0)
            {
                query = query.Where(q => q.ArcId == parameters.ArcId);
                _telemetry.RecordRadarFilterApplied(new TagList
                {
                    { "radar.id", id },
                    { "filter.type", "arc" }
                });
            }

            if (parameters.QuadrantId.HasValue && parameters.QuadrantId > 0)
            {
                query = query.Where(q => q.QuadrantId == parameters.QuadrantId);
                _telemetry.RecordRadarFilterApplied(new TagList
                {
                    { "radar.id", id },
                    { "filter.type", "quadrant" }
                });
            }

            if (parameters.TagId.HasValue && parameters.TagId > 0)
            {
                query = query.Where(ri => ri.Tags.Any(t => t.TagId == parameters.TagId));
                _telemetry.RecordRadarFilterApplied(new TagList
                {
                    { "radar.id", id },
                    { "filter.type", "tag" }
                });
            }

            var items = query.OrderBy(ri => ri.Name).ToList();
            
            activity?.SetTag("items.count", items.Count);
            _telemetry.RecordRadarItemsLoaded(items.Count, new TagList { { "radar.id", id } });

            return items;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetItems", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _telemetry.RecordServiceOperationDuration(stopwatch.ElapsedMilliseconds, new TagList
            {
                { "operation", "GetItems" }
            });
        }
    }

    /// <summary>
    /// Gets the arcs.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>List&lt;RadarArc&gt;.</returns>
    public List<RadarArc> GetArcs(int id)
    {
        using var activity = _telemetry.StartActivity("RadarService.GetArcs", ActivityKind.Server);
        activity?.SetTag("radar.id", id);

        try
        {
            var arcs = _context.RadarArcs.Where(ra => ra.RadarId == id).OrderBy(ra => ra.Position).ToList();
            activity?.SetTag("arcs.count", arcs.Count);
            return arcs;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetArcs", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the quadrants.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>List&lt;Quadrant&gt;.</returns>
    public List<Quadrant> GetQuadrants(int id)
    {
        using var activity = _telemetry.StartActivity("RadarService.GetQuadrants", ActivityKind.Server);
        activity?.SetTag("radar.id", id);

        try
        {
            var quadrants = _context.Quadrants.Where(q => q.RadarId == id).OrderBy(q => q.Position).ToList();
            activity?.SetTag("quadrants.count", quadrants.Count);
            return quadrants;
        }
        catch (Exception ex)
        {
            _telemetry.RecordError("GetQuadrants", ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
