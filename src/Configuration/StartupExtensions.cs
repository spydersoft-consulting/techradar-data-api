using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Spydersoft.TechRadar.Api.Options;
using System;

namespace Spydersoft.TechRadar.Api.Configuration
{
    /// <summary>
    /// Class StartupExtensions.
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Configures the open telemetry.
        /// </summary>
        /// <param name="otel">The otel.</param>
        /// <param name="options">The options.</param>
        public static void ConfigureOpenTelemetry(this OpenTelemetryBuilder otel, OpenTelemetryOptions options)
        {
            otel.WithMetrics(metrics => metrics
                // Metrics provider from OpenTelemetry
                .AddAspNetCoreInstrumentation()
                // Metrics provides by ASP.NET Core in .NET 8
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddPrometheusExporter()
            );

            // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
            otel.WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddSource("Spydersoft.TechRadar");
                if (options.TracingOltpEndpoint != null)
                {
                    tracing.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.TracingOltpEndpoint);
                    });
                }
                else
                {
                    tracing.AddConsoleExporter();
                }
            });
        }
    }
}
