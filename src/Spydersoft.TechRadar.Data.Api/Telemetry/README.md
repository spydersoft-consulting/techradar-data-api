# Tech Radar API Telemetry Instrumentation

## Overview
This document describes the comprehensive telemetry instrumentation implemented for the Spydersoft.TechRadar.Data.Api project using OpenTelemetry metrics and distributed tracing, integrated with **Spydersoft.Platform ITelemetryClient**.

## Implementation Summary

### 1. TechRadarTelemetryMetrics Helper Class
**Location:** `Spydersoft.TechRadar.Data.Api\Telemetry\TechRadarTelemetryMetrics.cs`

A centralized telemetry class that provides:
- **Integration with Spydersoft.Platform ITelemetryClient** - Uses the platform's telemetry infrastructure
- **Meter:** `Spydersoft.TechRadar.Data.Api` (v1.0.0)
- **ActivitySource:** `Spydersoft.TechRadar.Data.Api` (v1.0.0)
- **50+ Metrics** covering business, performance, and demonstrative scenarios
- Helper methods for starting activities and recording errors

**Platform Integration:**
The `TechRadarTelemetryMetrics` class depends on `ITelemetryClient` from `Spydersoft.Platform.Telemetry`, ensuring consistent telemetry patterns across all Spydersoft applications. The platform's telemetry client manages the lifecycle and configuration of OpenTelemetry exporters.

### 2. Dependency Injection
**Location:** `Spydersoft.TechRadar.Data.Api\Program.cs`

The `TechRadarTelemetryMetrics` class is registered as a scoped service in the DI container:
```csharp
builder.Services.AddScoped<Spydersoft.TechRadar.Data.Api.Telemetry.TechRadarTelemetryMetrics>();
```

This integrates with the existing platform telemetry setup:
```csharp
builder.AddSpydersoftTelemetry(typeof(Program).Assembly, new Spydersoft.Platform.Hosting.Telemetry.ConfigurationFunctions() {
    TraceConfiguration = (builder) => {
        builder.AddNpgsql();
    },
    MetricsConfiguration = (builder) => {
        builder.AddNpgsqlInstrumentation();
    }
});
```

## Metrics Implemented

### Business Metrics

#### Radar Metrics
- **`techradar.radars.retrieved.count`** - Counter for radar list retrievals
- **`techradar.radar.data.load.duration`** (ms) - Histogram for LoadRadarData operation duration
- **`techradar.radar.items.loaded.count`** - Counter for items loaded per radar
- **`techradar.radar.filter.applied.count`** - Counter when filters are applied (tags, date range)
- **`techradar.radar.entries.generated.count`** - Counter for radar entries generated
- **`techradar.radar.complexity.score`** - Histogram of radar complexity (items × quadrants × arcs)

#### Item Metrics
- **`techradar.items.created.count`** - Counter for items created, tagged by type (Radar, RadarItem, Quadrant, Arc)
- **`techradar.items.updated.count`** - Counter for items updated, tagged by type
- **`techradar.items.deleted.count`** - Counter for items deleted, tagged by type
- **`techradar.items.movement.direction`** - Histogram of movement direction changes (-3 to +3)

#### Note Metrics
- **`techradar.notes.created.count`** - Counter for notes created
- **`techradar.notes.retrieved.count`** - Counter for notes retrieved
- **`techradar.pagination.page.size`** - Histogram of requested page sizes
- **`techradar.pagination.total.records`** - Histogram of total records in paged results

#### Tag Metrics
- **`techradar.tags.created.count`** - Counter for tags created
- **`techradar.tags.assigned.count`** - Counter for tag assignments
- **`techradar.tags.removed.count`** - Counter for tag removals
- **`techradar.tags.retrieved.count`** - Counter for tag retrievals

### Performance Metrics
- **`techradar.service.operation.duration`** (ms) - Histogram of all service operation durations

### Error & Validation Metrics
- **`techradar.errors.count`** - Counter for errors by type
- **`techradar.validation.failures.count`** - Counter for validation failures with reason tags

### Demonstrative/Creative Metrics
These metrics showcase telemetry capabilities beyond typical use cases:

- **`techradar.item.name.length`** - Histogram of item name lengths
- **`techradar.legendkey.type.count`** - Counter for custom vs auto-generated legend keys
- **`techradar.color.diversity.count`** - Counter of unique colors used per radar
- **`techradar.url.presence.count`** - Counter of items with/without URLs
- **`techradar.item.age.days`** - Histogram of item ages (days since creation)
- **`techradar.item.staleness.days`** - Histogram of days since last update
- **`techradar.quadrant.distribution.count`** - Distribution of items across quadrants
- **`techradar.arc.distribution.count`** - Distribution of items across arcs/rings

## Distributed Tracing (Activities)

### Activity Names
Activities are created for all major service operations:

**RadarService:**
- `RadarService.GetRadarList`
- `RadarService.LoadRadarData`
- `RadarService.GetItems`
- `RadarService.GetArcs`
- `RadarService.GetQuadrants`

**RadarDataItemService:**
- `RadarDataItemService.GetRadarDataItem`
- `RadarDataItemService.SaveRadarDataItem`
- `RadarDataItemService.AddDefaultRingsAndQuadrants` (internal operation)
- `RadarDataItemService.DeleteRadarDataItem`
- `RadarDataItemService.GetNotes`

**TagService:**
- `TagService.GetAllRadarTags`
- `TagService.GetRadarTagsForItem`
- `TagService.SaveRadarItemTag`
- `TagService.RemoveRadarItemTag`

### Activity Tags
Each activity includes relevant tags:
- **`radar.id`** - Radar identifier
- **`item.type`** - Type of item (Radar, RadarItem, Quadrant, etc.)
- **`item.id`** - Item identifier
- **`user.id`** - User performing the operation
- **`operation.type`** - create/update/delete
- **`filter.*`** - Filter parameters (tags, arc.id, quadrant.id, days)
- **`page`, `page.size`** - Pagination information
- **`*.count`** - Result counts (items, tags, notes, etc.)
- **`movement.direction`** - Movement direction for radar items
- **`validation.error`** - Validation error type
- **`note.added`, `tag.assigned`, etc.** - Operation flags

## Instrumented Services

### RadarService
**Comprehensive instrumentation includes:**
- Timed operations with duration histograms
- Filter application tracking
- Complex metric calculation (color diversity, legend key types, etc.)
- Distribution tracking (items per quadrant/arc)
- Age and staleness tracking
- Error handling with telemetry

**Special Features:**
- Calculates radar complexity score: `items × quadrants × arcs`
- Tracks color diversity across radars
- Differentiates custom vs auto-generated legend keys
- Records item age and staleness distributions

### RadarDataItemService
**Instrumentation includes:**
- Create/Update/Delete counters with type tags
- Movement direction tracking for radar items
- Note creation metrics
- Default data generation tracking
- Comprehensive error handling
- Performance timing for all operations

**Special Features:**
- Tracks movement direction changes (-3 to +3 range)
- Records note creation with radar item context
- Monitors default quadrant/arc creation

### TagService
**Instrumentation includes:**
- Tag retrieval counts
- Tag creation tracking
- Tag assignment/removal counters
- Validation failure tracking with reasons
- Error handling for all operations
- Performance timing

**Special Features:**
- Tracks new tag creation vs existing tag assignment
- Records validation failures (ItemNotFound, TagNotFound, TagAlreadyExists)
- Monitors tag usage patterns

## Usage Examples

### Querying Metrics

#### Get radar retrieval count:
```promql
techradar_radars_retrieved_count_total
```

#### Average radar data load duration:
```promql
rate(techradar_radar_data_load_duration_sum[5m]) / rate(techradar_radar_data_load_duration_count[5m])
```

#### P95 service operation duration by operation type:
```promql
histogram_quantile(0.95, sum(rate(techradar_service_operation_duration_bucket[5m])) by (operation, le))
```

#### Items created per type:
```promql
sum by (item_type) (techradar_items_created_count_total)
```

#### Error rate by type:
```promql
rate(techradar_errors_count_total[5m])
```

#### Movement direction distribution:
```promql
histogram_quantile(0.5, sum(rate(techradar_items_movement_direction_bucket[5m])) by (le))
```

### Viewing Distributed Traces

Traces include:
- Full request path through the service layer
- Timing for each operation
- Contextual tags (radar.id, user.id, etc.)
- Error status and messages
- Nested activities for complex operations (e.g., default data creation)

## Dashboard Recommendations

### 1. Business Health Dashboard
- Total radars, items, tags (current counts)
- Items created/updated/deleted over time
- Most active radars (by entry count)
- Tag usage trends

### 2. Performance Dashboard
- P50, P90, P99 for operation durations
- Service operation duration by operation type
- Radar data load duration trends
- Database query performance (via Npgsql instrumentation)

### 3. User Engagement Dashboard
- Item age distribution
- Update frequency (staleness metrics)
- Tag usage patterns
- Note creation trends
- Movement direction heatmap

### 4. Data Quality Dashboard
- Legend key type distribution (custom vs auto)
- URL presence ratio
- Color diversity trends
- Quadrant/arc distribution balance
- Item name length distribution

### 5. Technical Health Dashboard
- Error rates by type
- Validation failure reasons
- Service operation success rates
- Concurrent request handling (from ASP.NET Core metrics)

## Testing

All unit tests have been updated to include the `TechRadarTelemetry` dependency:
- **RadarServiceTests** - Updated to inject telemetry
- **RadarDataItemServiceTests** - Updated to inject telemetry
- All tests pass successfully

## Benefits

### Observability
? Complete visibility into service operations  
? Distributed tracing across all layers  
? Performance monitoring at granular level  
? Error tracking with full context  

### Business Insights
? Understand radar complexity and usage patterns  
? Track user engagement through item updates and notes  
? Monitor tag adoption and usage  
? Identify stale or outdated content  

### Demonstrative Value
? Showcases creative use of metrics (color diversity, name length, etc.)  
? Demonstrates comprehensive instrumentation patterns  
? Provides examples of both business and technical metrics  
? Shows integration with OpenTelemetry standards  

### Production Readiness
? Error handling and validation tracking  
? Performance monitoring for optimization  
? User activity tracking for auditing  
? Scalable metric collection  

## Future Enhancements

Potential additions:
1. **Controller-level middleware** for HTTP request metrics
2. **Custom exporters** for specialized dashboards
3. **Alerting rules** based on thresholds
4. **SLI/SLO tracking** for service reliability
5. **Correlation with logs** for unified observability
6. **Real-time dashboards** using the collected metrics

## Configuration

The telemetry system automatically integrates with the existing OpenTelemetry configuration in `Program.cs`:
- **Traces:** Npgsql tracing already configured
- **Metrics:** Npgsql instrumentation already configured
- **Custom metrics:** Automatically exported through the configured exporters

No additional configuration required - metrics and traces will be exported to whatever backend is configured (e.g., OTLP, Prometheus, Jaeger, etc.).
