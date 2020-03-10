namespace ServiceMonitor.DataObjects
{
    /// <summary>
    /// Provides the settings for the REST service
    /// </summary>
    internal class RestSettings
    {
        /// <summary>
        /// Gets or sets the name of the service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the port of the REST service
        /// </summary>
        public int Port { get; set; }
    }
}
