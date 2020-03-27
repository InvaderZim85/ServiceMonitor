using System.ServiceProcess;

namespace ServiceMonitor.DataObjects
{
    /// <summary>
    /// Represents a service entry
    /// </summary>
    internal sealed class ServiceEntry
    {
        /// <summary>
        /// The name of the service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the name without any dots
        /// </summary>
        public string CleanName => Name.Replace(".", "").Replace(" ", "");

        /// <summary>
        /// Gets or sets the display name of the service
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the state of the service
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the start type of the service
        /// </summary>
        public string StartType { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates if the service is currently running or not
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the port of the service
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Gets or sets the description of the service
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Converts the <see cref="ServiceController"/> object into a <see cref="ServiceEntry"/> object
        /// </summary>
        /// <param name="service">The <see cref="ServiceController"/> object</param>
        public static explicit operator ServiceEntry(ServiceController service)
        {
            if (service == null)
                return new ServiceEntry();

            return new ServiceEntry
            {
                Name = service.ServiceName,
                DisplayName = service.DisplayName,
                State = service.Status.ToString(),
                StartType = service.StartType.ToString(),
                IsRunning = service.Status == ServiceControllerStatus.Running || service.Status == ServiceControllerStatus.StartPending
            };
        }
    }
}
