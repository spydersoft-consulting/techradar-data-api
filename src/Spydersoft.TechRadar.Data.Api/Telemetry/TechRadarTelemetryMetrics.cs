using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Spydersoft.Platform.Telemetry;

namespace Spydersoft.TechRadar.Data.Api.Telemetry;

/// <summary>
/// Provides telemetry instrumentation helpers for the Tech Radar API.
/// This class provides convenient wrapper methods around ITelemetryClient for recording metrics.
/// The ITelemetryClient manages meter lifecycle and metric instances internally by name.
/// </summary>
public class TechRadarTelemetryMetrics
{
    private readonly ITelemetryClient _telemetryClient;

    // Metric names as constants for consistency
    private const string RadarsRetrievedCounterName = "techradar.radars.retrieved.count";
    private const string RadarDataLoadDurationName = "techradar.radar.data.load.duration";
    private const string RadarItemsLoadedCounterName = "techradar.radar.items.loaded.count";
    private const string RadarFilterAppliedCounterName = "techradar.radar.filter.applied.count";
    private const string RadarEntriesGeneratedCounterName = "techradar.radar.entries.generated.count";
    private const string RadarComplexityScoreName = "techradar.radar.complexity.score";
    
    private const string ItemsCreatedCounterName = "techradar.items.created.count";
    private const string ItemsUpdatedCounterName = "techradar.items.updated.count";
    private const string ItemsDeletedCounterName = "techradar.items.deleted.count";
    private const string ItemMovementDirectionName = "techradar.items.movement.direction";
    private const string ServiceOperationDurationName = "techradar.service.operation.duration";
    
    private const string NotesCreatedCounterName = "techradar.notes.created.count";
    private const string NotesRetrievedCounterName = "techradar.notes.retrieved.count";
    private const string PaginationPageSizeName = "techradar.pagination.page.size";
    private const string PaginationTotalRecordsName = "techradar.pagination.total.records";
    
    private const string TagsCreatedCounterName = "techradar.tags.created.count";
    private const string TagsAssignedCounterName = "techradar.tags.assigned.count";
    private const string TagsRemovedCounterName = "techradar.tags.removed.count";
    private const string TagsRetrievedCounterName = "techradar.tags.retrieved.count";
    
    private const string ItemNameLengthName = "techradar.item.name.length";
    private const string LegendKeyTypeCounterName = "techradar.legendkey.type.count";
    private const string ColorDiversityCounterName = "techradar.color.diversity.count";
    private const string UrlPresenceCounterName = "techradar.url.presence.count";
    private const string ItemAgeDaysName = "techradar.item.age.days";
    private const string ItemStalenessDaysName = "techradar.item.staleness.days";
    private const string QuadrantDistributionCounterName = "techradar.quadrant.distribution.count";
    private const string ArcDistributionCounterName = "techradar.arc.distribution.count";
    
    private const string ErrorCounterName = "techradar.errors.count";
    private const string ValidationFailureCounterName = "techradar.validation.failures.count";

    /// <summary>
    /// Initializes a new instance of the <see cref="TechRadarTelemetryMetrics"/> class.
    /// </summary>
    /// <param name="telemetryClient">The platform telemetry client (singleton).</param>
    public TechRadarTelemetryMetrics(ITelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
    }

    #region Radar Metrics

    /// <summary>
    /// Records that a radar list was retrieved.
    /// </summary>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordRadarRetrieved(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(RadarsRetrievedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that a radar list was retrieved.
    /// </summary>
    /// <param name="tagList">Optional tag list for the metric.</param>
    public void RecordRadarRetrieved(TagList tagList)
    {
        RecordRadarRetrieved(ToTags(tagList));
    }

    /// <summary>
    /// Records the duration of a radar data load operation.
    /// </summary>
    /// <param name="durationMs">Duration in milliseconds.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordRadarDataLoadDuration(double durationMs, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(RadarDataLoadDurationName, durationMs, tags);
    }

    /// <summary>
    /// Records the duration of a radar data load operation.
    /// </summary>
    /// <param name="durationMs">Duration in milliseconds.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordRadarDataLoadDuration(double durationMs, TagList tagList)
    {
        RecordRadarDataLoadDuration(durationMs, ToTags(tagList));
    }

    /// <summary>
    /// Records the number of items loaded for a radar.
    /// </summary>
    /// <param name="count">Number of items loaded.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordRadarItemsLoaded(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(RadarItemsLoadedCounterName, count, tags);
    }

    /// <summary>
    /// Records the number of items loaded for a radar.
    /// </summary>
    /// <param name="count">Number of items loaded.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordRadarItemsLoaded(long count, TagList tagList)
    {
        RecordRadarItemsLoaded(count, ToTags(tagList));
    }

    /// <summary>
    /// Records that a filter was applied to radar data.
    /// </summary>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordRadarFilterApplied(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(RadarFilterAppliedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that a filter was applied to radar data.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordRadarFilterApplied(TagList tagList)
    {
        RecordRadarFilterApplied(ToTags(tagList));
    }

    /// <summary>
    /// Records the number of radar entries generated.
    /// </summary>
    /// <param name="count">Number of entries generated.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordRadarEntriesGenerated(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(RadarEntriesGeneratedCounterName, count, tags);
    }

    /// <summary>
    /// Records the number of radar entries generated.
    /// </summary>
    /// <param name="count">Number of entries generated.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordRadarEntriesGenerated(long count, TagList tagList)
    {
        RecordRadarEntriesGenerated(count, ToTags(tagList));
    }

    /// <summary>
    /// Records a radar complexity score.
    /// </summary>
    /// <param name="score">The complexity score.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordRadarComplexityScore(double score, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(RadarComplexityScoreName, score, tags);
    }

    /// <summary>
    /// Records a radar complexity score.
    /// </summary>
    /// <param name="score">The complexity score.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordRadarComplexityScore(double score, TagList tagList)
    {
        RecordRadarComplexityScore(score, ToTags(tagList));
    }

    #endregion

    #region RadarDataItem Metrics

    /// <summary>
    /// Records that an item was created.
    /// </summary>
    /// <param name="tags">Optional tags for the metric (should include item.type).</param>
    public void RecordItemCreated(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(ItemsCreatedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that an item was created.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemCreated(TagList tagList)
    {
        RecordItemCreated(ToTags(tagList));
    }

    /// <summary>
    /// Records that an item was updated.
    /// </summary>
    /// <param name="tags">Optional tags for the metric (should include item.type).</param>
    public void RecordItemUpdated(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(ItemsUpdatedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that an item was updated.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemUpdated(TagList tagList)
    {
        RecordItemUpdated(ToTags(tagList));
    }

    /// <summary>
    /// Records that an item was deleted.
    /// </summary>
    /// <param name="tags">Optional tags for the metric (should include item.type).</param>
    public void RecordItemDeleted(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(ItemsDeletedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that an item was deleted.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemDeleted(TagList tagList)
    {
        RecordItemDeleted(ToTags(tagList));
    }

    /// <summary>
    /// Records an item movement direction.
    /// </summary>
    /// <param name="direction">Movement direction (-3 to +3).</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordItemMovementDirection(int direction, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(ItemMovementDirectionName, direction, tags);
    }

    /// <summary>
    /// Records an item movement direction.
    /// </summary>
    /// <param name="direction">Movement direction (-3 to +3).</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemMovementDirection(int direction, TagList tagList)
    {
        RecordItemMovementDirection(direction, ToTags(tagList));
    }

    /// <summary>
    /// Records the duration of a service operation.
    /// </summary>
    /// <param name="durationMs">Duration in milliseconds.</param>
    /// <param name="tags">Optional tags for the metric (should include operation name).</param>
    public void RecordServiceOperationDuration(double durationMs, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(ServiceOperationDurationName, durationMs, tags);
    }

    /// <summary>
    /// Records the duration of a service operation.
    /// </summary>
    /// <param name="durationMs">Duration in milliseconds.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordServiceOperationDuration(double durationMs, TagList tagList)
    {
        RecordServiceOperationDuration(durationMs, ToTags(tagList));
    }

    #endregion

    #region Note Metrics

    /// <summary>
    /// Records that a note was created.
    /// </summary>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordNoteCreated(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(NotesCreatedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that a note was created.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordNoteCreated(TagList tagList)
    {
        RecordNoteCreated(ToTags(tagList));
    }

    /// <summary>
    /// Records that notes were retrieved.
    /// </summary>
    /// <param name="count">Number of notes retrieved.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordNotesRetrieved(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(NotesRetrievedCounterName, count, tags);
    }

    /// <summary>
    /// Records that notes were retrieved.
    /// </summary>
    /// <param name="count">Number of notes retrieved.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordNotesRetrieved(long count, TagList tagList)
    {
        RecordNotesRetrieved(count, ToTags(tagList));
    }

    /// <summary>
    /// Records a pagination page size.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordPaginationPageSize(int pageSize, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(PaginationPageSizeName, pageSize, tags);
    }

    /// <summary>
    /// Records a pagination page size.
    /// </summary>
    /// <param name="pageSize">The page size.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordPaginationPageSize(int pageSize, TagList tagList)
    {
        RecordPaginationPageSize(pageSize, ToTags(tagList));
    }

    /// <summary>
    /// Records the total records in a paged result.
    /// </summary>
    /// <param name="totalRecords">Total number of records.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordPaginationTotalRecords(int totalRecords, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(PaginationTotalRecordsName, totalRecords, tags);
    }

    /// <summary>
    /// Records the total records in a paged result.
    /// </summary>
    /// <param name="totalRecords">Total number of records.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordPaginationTotalRecords(int totalRecords, TagList tagList)
    {
        RecordPaginationTotalRecords(totalRecords, ToTags(tagList));
    }

    #endregion

    #region Tag Metrics

    /// <summary>
    /// Records that a tag was created.
    /// </summary>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordTagCreated(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(TagsCreatedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that a tag was created.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordTagCreated(TagList tagList)
    {
        RecordTagCreated(ToTags(tagList));
    }

    /// <summary>
    /// Records that a tag was assigned to an item.
    /// </summary>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordTagAssigned(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(TagsAssignedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that a tag was assigned to an item.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordTagAssigned(TagList tagList)
    {
        RecordTagAssigned(ToTags(tagList));
    }

    /// <summary>
    /// Records that a tag was removed from an item.
    /// </summary>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordTagRemoved(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(TagsRemovedCounterName, 1, tags);
    }

    /// <summary>
    /// Records that a tag was removed from an item.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordTagRemoved(TagList tagList)
    {
        RecordTagRemoved(ToTags(tagList));
    }

    /// <summary>
    /// Records that tags were retrieved.
    /// </summary>
    /// <param name="count">Number of tags retrieved.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordTagsRetrieved(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(TagsRetrievedCounterName, count, tags);
    }

    /// <summary>
    /// Records that tags were retrieved.
    /// </summary>
    /// <param name="count">Number of tags retrieved.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordTagsRetrieved(long count, TagList tagList)
    {
        RecordTagsRetrieved(count, ToTags(tagList));
    }

    #endregion

    #region Demonstrative/Creative Metrics

    /// <summary>
    /// Records an item name length (demonstrative metric).
    /// </summary>
    /// <param name="length">Length of the item name.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordItemNameLength(int length, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(ItemNameLengthName, length, tags);
    }

    /// <summary>
    /// Records an item name length (demonstrative metric).
    /// </summary>
    /// <param name="length">Length of the item name.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemNameLength(int length, TagList tagList)
    {
        RecordItemNameLength(length, ToTags(tagList));
    }

    /// <summary>
    /// Records a legend key type count.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="tags">Optional tags for the metric (should include type: custom/auto).</param>
    public void RecordLegendKeyType(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(LegendKeyTypeCounterName, count, tags);
    }

    /// <summary>
    /// Records a legend key type count.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordLegendKeyType(long count, TagList tagList)
    {
        RecordLegendKeyType(count, ToTags(tagList));
    }

    /// <summary>
    /// Records color diversity.
    /// </summary>
    /// <param name="count">Number of unique colors.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordColorDiversity(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(ColorDiversityCounterName, count, tags);
    }

    /// <summary>
    /// Records color diversity.
    /// </summary>
    /// <param name="count">Number of unique colors.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordColorDiversity(long count, TagList tagList)
    {
        RecordColorDiversity(count, ToTags(tagList));
    }

    /// <summary>
    /// Records URL presence.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="tags">Optional tags for the metric (should include has.url: true/false).</param>
    public void RecordUrlPresence(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(UrlPresenceCounterName, count, tags);
    }

    /// <summary>
    /// Records URL presence.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordUrlPresence(long count, TagList tagList)
    {
        RecordUrlPresence(count, ToTags(tagList));
    }

    /// <summary>
    /// Records an item's age in days.
    /// </summary>
    /// <param name="ageDays">Age in days since creation.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordItemAge(int ageDays, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(ItemAgeDaysName, ageDays, tags);
    }

    /// <summary>
    /// Records an item's age in days.
    /// </summary>
    /// <param name="ageDays">Age in days since creation.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemAge(int ageDays, TagList tagList)
    {
        RecordItemAge(ageDays, ToTags(tagList));
    }

    /// <summary>
    /// Records an item's staleness in days.
    /// </summary>
    /// <param name="stalenessDays">Days since last update.</param>
    /// <param name="tags">Optional tags for the metric.</param>
    public void RecordItemStaleness(int stalenessDays, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordHistogram(ItemStalenessDaysName, stalenessDays, tags);
    }

    /// <summary>
    /// Records an item's staleness in days.
    /// </summary>
    /// <param name="stalenessDays">Days since last update.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordItemStaleness(int stalenessDays, TagList tagList)
    {
        RecordItemStaleness(stalenessDays, ToTags(tagList));
    }

    /// <summary>
    /// Records quadrant distribution.
    /// </summary>
    /// <param name="count">Number of items in the quadrant.</param>
    /// <param name="tags">Optional tags for the metric (should include quadrant.id).</param>
    public void RecordQuadrantDistribution(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(QuadrantDistributionCounterName, count, tags);
    }

    /// <summary>
    /// Records quadrant distribution.
    /// </summary>
    /// <param name="count">Number of items in the quadrant.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordQuadrantDistribution(long count, TagList tagList)
    {
        RecordQuadrantDistribution(count, ToTags(tagList));
    }

    /// <summary>
    /// Records arc/ring distribution.
    /// </summary>
    /// <param name="count">Number of items in the arc.</param>
    /// <param name="tags">Optional tags for the metric (should include arc.id).</param>
    public void RecordArcDistribution(long count, IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(ArcDistributionCounterName, count, tags);
    }

    /// <summary>
    /// Records arc/ring distribution.
    /// </summary>
    /// <param name="count">Number of items in the arc.</param>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordArcDistribution(long count, TagList tagList)
    {
        RecordArcDistribution(count, ToTags(tagList));
    }

    #endregion

    #region Error Metrics

    /// <summary>
    /// Records an error.
    /// </summary>
    /// <param name="errorType">Type of error.</param>
    /// <param name="exception">Optional exception.</param>
    public void RecordError(string errorType, Exception? exception = null)
    {
        var tags = new Dictionary<string, object> { { "error.type", errorType } };
        
        if (exception != null)
        {
            tags["exception.type"] = exception.GetType().Name;
            tags["exception.message"] = exception.Message;
        }

        _telemetryClient.RecordCounter(ErrorCounterName, 1, tags);
    }

    /// <summary>
    /// Records a validation failure.
    /// </summary>
    /// <param name="tags">Optional tags for the metric (should include reason).</param>
    public void RecordValidationFailure(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter(ValidationFailureCounterName, 1, tags);
    }

    /// <summary>
    /// Records a validation failure.
    /// </summary>
    /// <param name="tagList">Tag list for the metric.</param>
    public void RecordValidationFailure(TagList tagList)
    {
        RecordValidationFailure(ToTags(tagList));
    }

    #endregion

    #region Activity/Tracing

    /// <summary>
    /// Starts a new activity for the specified operation using ITelemetryClient.
    /// </summary>
    /// <param name="operationName">Name of the operation</param>
    /// <param name="kind">The activity kind</param>
    /// <returns>Activity instance or null if not enabled</returns>
    public Activity? StartActivity(string operationName, ActivityKind kind = ActivityKind.Internal)
    {
        // Use TrackEvent to create an activity through ITelemetryClient
        // Or we could maintain our own ActivitySource - but let's delegate to the platform
        return Activity.Current?.Source.StartActivity(operationName, kind);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Converts a TagList to a Dictionary for use with ITelemetryClient.
    /// </summary>
    public static IDictionary<string, object>? ToTags(TagList tagList)
    {
        if (tagList.Count == 0) return null;
        
        var dict = new Dictionary<string, object>();
        foreach (var tag in tagList)
        {
            if (tag.Value != null)
            {
                dict[tag.Key] = tag.Value;
            }
        }
        return dict.Count > 0 ? dict : null;
    }

    #endregion
}
