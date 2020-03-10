using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;
using ZimLabs.Utility.Extensions;

namespace ServiceMonitor.Business
{
    /// <summary>
    /// Provides the functions to check the running services
    /// </summary>
    internal static class ServiceHelper
    {
        /// <summary>
        /// Loads the list with the desired services and their current state
        /// </summary>
        /// <returns>The list with the services</returns>
        public static List<ServiceEntry> LoadServices()
        {
            // Step 1: Get the desired services
            var path = Path.Combine(ZimLabs.Utility.Global.GetBaseFolder(), "Services.json");
            var services = Helper.LoadJsonFile<List<string>>(path);

            // Step 2: Get all services which are currently are running
            return LoadServiceController().Where(w => services.Any(a => a.ContainsIgnoreCase(w.ServiceName)))
                .Select(s => (ServiceEntry) s).ToList();
        }

        /// <summary>
        /// Loads the service controller
        /// </summary>
        /// <returns>The array with the services</returns>
        private static IEnumerable<ServiceController> LoadServiceController()
        {
            return ServiceController.GetServices(Environment.MachineName);
        }
    }
}
