namespace ServiceMonitor.DataObjects
{
    /// <summary>
    /// Provides the settings for the REST service and the topshelf service
    /// </summary>
    internal sealed class Settings
    {
        /// <summary>
        /// Gets or sets the REST settings
        /// </summary>
        public RestSettings Rest { get; set; }

        /// <summary>
        /// Gets or sets the service settings
        /// </summary>
        public ServiceSettings Service { get; set; }
    }
}
