namespace Spydersoft.TechRadar.Api.Options
{
    /// <summary>
    /// Class IdentityOptions.
    /// </summary>
    public class IdentityOptions
    {
        /// <summary>
        /// The section name
        /// </summary>
        public static string SectionName = "Identity";

        /// <summary>
        /// Gets or sets the authority.
        /// </summary>
        /// <value>The authority.</value>
        public string? Authority { get; set; } = null;

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string? ApplicationName { get; set; } = null;
    }
}
