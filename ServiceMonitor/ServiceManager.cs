using System;
using Nancy.Hosting.Self;
using ServiceMonitor.Global;

namespace ServiceMonitor
{
    /// <summary>
    /// Provides the functions to interact with the nancy REST service
    /// </summary>
    internal sealed class ServiceManager : IDisposable
    {
        /// <summary>
        /// Contains the value which indicates if the class was already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Contains the instance of the nancy host
        /// </summary>
        private NancyHost _host;

        public void Start()
        {
            var hostConfig = new HostConfiguration
            {
                UrlReservations = new UrlReservations { CreateAutomatically = true }
            };

            _host = new NancyHost(hostConfig, new Uri(Helper.HostPath));
            _host.Start();

            ServiceLogger.Info($"Service started and listening on: {Helper.HostPath}");
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        public void Stop()
        {
            _host?.Stop();
            ServiceLogger.Info("Service stopped.");
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        /// <param name="dispose">true to dispose the class</param>
        private void Dispose(bool dispose)
        {
            if (_disposed)
                return;

            if (dispose)
            {
                _host?.Dispose();
            }

            _disposed = true;
        }
    }
}
