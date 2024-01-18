namespace Spydersoft.TechRadar.Data.Api.Options
{
    /// <summary>
    /// Class OpenTelemetryOptions.
    /// </summary>
    public class OpenTelemetryOptions
    {
        /// <summary>
        /// The section name
        /// </summary>
        public const string SectionName = "OpenTelemetry";

        /// <summary>
        /// Gets or sets the tracing oltp endpoint.
        /// </summary>
        /// <value>The tracing oltp endpoint.</value>
        public string? TracingOltpEndpoint { get; set; } = null;
    }
}
