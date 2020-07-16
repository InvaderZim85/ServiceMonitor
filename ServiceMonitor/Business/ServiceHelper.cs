using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Newtonsoft.Json;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;
using ZimLabs.CoreLib;
using ZimLabs.CoreLib.Extensions;

namespace ServiceMonitor.Business
{
    /// <summary>
    /// Provides the functions to check the running services
    /// </summary>
    internal static class ServiceHelper
    {
        /// <summary>
        /// Contains the dictionary with the services
        /// </summary>
        private static List<ServiceEntry> _servicesInfoList = new List<ServiceEntry>();

        /// <summary>
        /// Loads the list with the desired services and their current state
        /// </summary>
        /// <returns>The list with the services</returns>
        public static List<ServiceEntry> LoadServices()
        {
            // Step 1: Get the desired services
            LoadServiceSettings();

            // Step 2: Get all services which are currently are running
            var tmpServices = LoadServiceController();

            var result = new List<ServiceEntry>();

            foreach (var service in tmpServices)
            {
                var info = _servicesInfoList.FirstOrDefault(f => f.Name.EqualsIgnoreCase(service.ServiceName));
                if (info == null)
                    continue;

                var tmpResult = (ServiceEntry) service;
                tmpResult.Port = info.Port ?? "/";
                tmpResult.Description = info.Description ?? "/";

                result.Add(tmpResult);
            }

            return result;
        }

        /// <summary>
        /// Loads the service controller
        /// </summary>
        /// <returns>The array with the services</returns>
        private static IEnumerable<ServiceController> LoadServiceController()
        {
            return ServiceController.GetServices(Environment.MachineName);
        }

        /// <summary>
        /// Loads all service settings and saves them into the service dictionary
        /// </summary>
        public static void LoadServiceSettings()
        {
            try
            {
                var path = Path.Combine(Core.GetBaseFolder(), "Services.json");
                _servicesInfoList = Helper.LoadJsonFile<List<ServiceEntry>>(path);
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(nameof(LoadServiceSettings), ex);
            }
        }

        /// <summary>
        /// Adds a new service to the service list
        /// </summary>
        /// <param name="service">The service which should be added</param>
        /// <returns>true when successful, otherwise false</returns>
        public static (bool successful, string message) AddService(ServiceEntry service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            // Step 1: Check if the service exist
            var services = LoadServiceController();
            if (!services.Any(a => a.ServiceName.EqualsIgnoreCase(service.Name)))
                return (false, $"The service '{service.Name}' doesn't exist.");

            // Step 2: Add the service
            LoadServiceSettings();

            _servicesInfoList.Add(service);

            var path = Path.Combine(Core.GetBaseFolder(), "Services.json");
            var content = JsonConvert.SerializeObject(_servicesInfoList, Formatting.Indented);
            File.WriteAllText(path, content);

            return File.Exists(path) ? (true, "") : (false, "Can't save service file.");
        }
    }
}
