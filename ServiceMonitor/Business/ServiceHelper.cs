using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;
using ZimLabs.CoreLib;
using ZimLabs.CoreLib.Extensions;

namespace ServiceMonitor.Business
{
    /// <summary>
    /// Provides the functions to check the running services
    /// </summary>
    internal sealed class ServiceHelper
    {
        /// <summary>
        /// Contains the path of the settings file
        /// </summary>
        private readonly string _filePath = Path.Combine(Core.GetBaseFolder(), "Services.json");

        /// <summary>
        /// Backing field for <see cref="ServiceList"/>
        /// </summary>
        private List<ServiceEntry> _serviceList;

        /// <summary>
        /// Gets the list with the available services
        /// </summary>
        public List<ServiceEntry> ServiceList
        {
            get
            {
                if (_serviceList == null || !_serviceList.Any())
                    LoadServices();

                return _serviceList;
            }
        }

        /// <summary>
        /// Loads the list with the desired services and their current state
        /// </summary>
        /// <returns>The list with the services</returns>
        private void LoadServices()
        {
            // Step 1: Get the desired services
            var tmpServiceList = Helper.LoadJsonFile<List<ServiceEntry>>(_filePath) ?? new List<ServiceEntry>();

            // Step 2: Get all services which are currently are running
            var tmpServices = LoadServiceController();

            _serviceList = new List<ServiceEntry>();

            foreach (var service in tmpServices)
            {
                var info = tmpServiceList.FirstOrDefault(f => f.Name.EqualsIgnoreCase(service.ServiceName));
                if (info == null)
                    continue;

                var tmpResult = (ServiceEntry) service;
                tmpResult.Port = info.Port ?? "/";
                tmpResult.Description = info.Description ?? "/";

                _serviceList.Add(tmpResult);
            }
        }

        /// <summary>
        /// Loads the service controller
        /// </summary>
        /// <returns>The array with the services</returns>
        private IEnumerable<ServiceController> LoadServiceController()
        {
            return ServiceController.GetServices(Environment.MachineName);
        }

        /// <summary>
        /// Adds a new service to the service list
        /// </summary>
        /// <param name="service">The service which should be added</param>
        /// <exception cref="EndpointNotFoundException">Will be thrown when the specified service was not found</exception>
        public void AddService(ServiceEntry service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            // Step 1: Check if the given service is an existing service
            var services = LoadServiceController();
            var tmpService = services?.FirstOrDefault(f => f.ServiceName.EqualsIgnoreCase(service.Name));
            if (tmpService == null)
                throw new EndpointNotFoundException();

            var tmpServiceEntry = (ServiceEntry) tmpService;
            tmpServiceEntry.Port = service.Port;
            tmpServiceEntry.Description = service.Description;

            // Check if the service already exists
            if (ServiceList.Any(a => a.Name.EqualsIgnoreCase(service.Name)))
                return;

            ServiceList.Add(tmpServiceEntry);

            Helper.WriteJsonFile(_filePath, ServiceList);
        }

        /// <summary>
        /// Deletes the desired service
        /// </summary>
        /// <param name="service">The name of the service which should be deleted</param>
        public void DeleteService(string service)
        {
            if (string.IsNullOrEmpty(service))
                throw new ArgumentNullException(nameof(service));

            var tmpService = ServiceList.FirstOrDefault(f => f.Name.EqualsIgnoreCase(service));
            if (tmpService == null)
                return; // No service available, so everything is fine...

            ServiceList.Remove(tmpService);

            Helper.WriteJsonFile(_filePath, ServiceList);
        }
    }
}
