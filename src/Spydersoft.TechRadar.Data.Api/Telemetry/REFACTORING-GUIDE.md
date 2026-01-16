# Tech Radar Telemetry Refactoring Guide

## Overview
This document outlines the refactoring needed to properly use `Spydersoft.Platform.Telemetry.ITelemetryClient` in the Tech Radar API.

## Key Architectural Understanding

### How ITelemetryClient Works
Based on the `MeterTelemetryClient` implementation:

1. **ITelemetryClient is a Singleton** - Registered for the application lifetime
2. **Metrics are cached by name** - Internal dictionaries store Counter<T> and Histogram<T> by name
3. **GetOrCreate Pattern** - Calling `RecordCounter("metric.name")` will:
   - Check if a counter with that name exists
   - If not, create it and cache it  
   - Use the cached counter to record the value

### What This Means
- ? **No need to create Meter instances** - ITelemetryClient manages them
- ? **No need to store Counter/Histogram instances** - ITelemetryClient caches them by name
- ? **Just call methods with consistent names** - The platform handles the rest

## Refactoring Changes

### Old Pattern (INCORRECT)
```csharp
public class TechRadarTelemetryMetrics
{
    private readonly Meter _meter;
    public Counter<long> RadarsRetrievedCounter { get; }
    
    public TechRadarTelemetryMetrics()
    {
        _meter = new Meter("name");
        RadarsRetrievedCounter = _meter.CreateCounter<long>("techradar.radars.retrieved.count");
    }
}

// Service usage:
_telemetry.RadarsRetrievedCounter.Add(1, new TagList { { "radar.count", count } });
```

### New Pattern (CORRECT)
```csharp
public class TechRadarTelemetryMetrics
{
    private readonly ITelemetryClient _telemetryClient;
    
    public TechRadarTelemetryMetrics(ITelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }
    
    public void RecordRadarRetrieved(IDictionary<string, object>? tags = null)
    {
        _telemetryClient.RecordCounter("techradar.radars.retrieved.count", 1, tags);
    }
}

// Service usage:
_telemetry.RecordRadarRetrieved(new Dictionary<string, object> { {"radar.count", count} });
```

## Required Changes

###  1. TechRadarTelemetryMetrics Class
**Status:** ? COMPLETED

The class now:
- Takes `ITelemetryClient` as a dependency
- Defines metric names as constants
- Provides wrapper methods that call `ITelemetryClient.RecordCounter/RecordHistogram`
- Includes TagList overloads for convenience

### 2. Service Layer Updates  
**Status:** ? **NEEDS COMPLETION**

All services need to be updated to use the new method-based API:

#### RadarService
Replace:
```csharp
_telemetry.RadarsRetrievedCounter.Add(1, new TagList { { "radar.count", radars.Count } });
```

With:
```csharp
_telemetry.RecordRadarRetrieved(new TagList { { "radar.count", radars.Count } });
```

**Files to update:**
- `RadarService.cs` - ~30 metric calls
- `RadarDataItemService.cs` - ~20 metric calls  
- `TagService.cs` - ~15 metric calls

#### Detailed Service Updates Needed:

**RadarService.cs:**
| Old Call | New Method |
|---------|------------|
| `RadarsRetrievedCounter.Add(...)` | `RecordRadarRetrieved(TagList)` |
| `RadarDataLoadDuration.Record(...)` | `RecordRadarDataLoadDuration(ms, TagList)` |
| `RadarItemsLoadedCounter.Add(...)` | `RecordRadarItemsLoaded(count, TagList)` |
| `RadarFilterAppliedCounter.Add(...)` | `RecordRadarFilterApplied(TagList)` |
| `RadarEntriesGeneratedCounter.Add(...)` | `RecordRadarEntriesGenerated(count, TagList)` |
| `RadarComplexityScore.Record(...)` | `RecordRadarComplexityScore(score, TagList)` |
| `LegendKeyTypeCounter.Add(...)` | `RecordLegendKeyType(count, TagList)` |
| `ColorDiversityCounter.Add(...)` | `RecordColorDiversity(count, TagList)` |
| `UrlPresenceCounter.Add(...)` | `RecordUrlPresence(count, TagList)` |
| `ItemNameLength.Record(...)` | `RecordItemNameLength(length, TagList)` |
| `ItemAgeDays.Record(...)` | `RecordItemAge(days, TagList)` |
| `ItemStalenessDays.Record(...)` | `RecordItemStaleness(days, TagList)` |
| `QuadrantDistributionCounter.Add(...)` | `RecordQuadrantDistribution(count, TagList)` |
| `ArcDistributionCounter.Add(...)` | `RecordArcDistribution(count, TagList)` |
| `ServiceOperationDuration.Record(...)` | `RecordServiceOperationDuration(ms, TagList)` |

**RadarDataItemService.cs:**
| Old Call | New Method |
|---------|------------|
| `ItemsCreatedCounter.Add(...)` | `RecordItemCreated(TagList)` |
| `ItemsUpdatedCounter.Add(...)` | `RecordItemUpdated(TagList)` |
| `ItemsDeletedCounter.Add(...)` | `RecordItemDeleted(TagList)` |
| `ItemMovementDirection.Record(...)` | `RecordItemMovementDirection(direction, TagList)` |
| `NotesCreatedCounter.Add(...)` | `RecordNoteCreated(TagList)` |
| `NotesRetrievedCounter.Add(...)` | `RecordNotesRetrieved(count, TagList)` |
| `PaginationPageSize.Record(...)` | `RecordPaginationPageSize(size, TagList)` |
| `PaginationTotalRecords.Record(...)` | `RecordPaginationTotalRecords(total, TagList)` |
| `ServiceOperationDuration.Record(...)` | `RecordServiceOperationDuration(ms, TagList)` |

**TagService.cs:**
| Old Call | New Method |
|---------|------------|
| `TagsCreatedCounter.Add(...)` | `RecordTagCreated(TagList)` |
| `TagsAssignedCounter.Add(...)` | `RecordTagAssigned(TagList)` |
| `TagsRemovedCounter.Add(...)` | `RecordTagRemoved(TagList)` |
| `TagsRetrievedCounter.Add(...)` | `RecordTagsRetrieved(count, TagList)` |
| `ValidationFailureCounter.Add(...)` | `RecordValidationFailure(TagList)` |
| `ServiceOperationDuration.Record(...)` | `RecordServiceOperationDuration(ms, TagList)` |

### 3. Test Updates
**Status:** ? COMPLETED  

Tests now properly mock `ITelemetryClient` without needing to return real Meter/ActivitySource instances.

## Implementation Strategy

### Option 1: Manual Update (Recommended for Understanding)
1. Open each service file
2. Find all `_telemetry.` calls
3. Replace with the corresponding `Record*` method from the mapping table above
4. Build and fix any remaining issues

### Option 2: Find & Replace
Use IDE find/replace with regex to bulk update:
```regex
Find: _telemetry\.(\w+)Counter\.Add\((.*?)\)
Replace: _telemetry.Record$1($2)
```

*Note: This may need manual adjustment*

### Option 3: Automated Refactoring
Create a Roslyn analyzer or script to automatically update all calls.

## Benefits of This Approach

? **Platform Consistency** - Uses the same telemetry patterns as other Spydersoft apps  
? **Memory Efficient** - No duplicate metric instances  
? **Centralized Management** - ITelemetryClient handles all metric lifecycle  
? **Simpler Code** - No need to manage Meter/Counter/Histogram instances  
? **Better Testability** - Mock ITelemetryClient instead of individual metrics  

## Next Steps

1. [ ] Complete service layer updates (RadarService, RadarDataItemService, TagService)
2. [ ] Build and verify no compilation errors
3. [ ] Run tests to ensure functionality is preserved
4. [ ] Update documentation if needed
5. [ ] Consider adding integration tests for telemetry

## Reference

- Platform Source: `https://github.com/spydersoft-consulting/platform/blob/main/src/Spydersoft.Platform/Spydersoft.Platform/Telemetry/MeterTelemetryClient.cs`
- ITelemetryClient caches metrics by name using internal dictionaries
- The singleton lifetime ensures metrics persist for the application lifetime
