using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Nancy;
using ServiceMonitor.Business;
using ServiceMonitor.Global;
using ZimLabs.Utility.Extensions;

namespace ServiceMonitor.Modules
{
    /// <summary>
    /// Represents the main model of the REST service
    /// </summary>
    public sealed class MainModel : NancyModule
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MainModel"/>
        /// </summary>
        public MainModel()
        {
            Get("/", _ => ProcessGetRequest());
        }

        /// <summary>
        /// Processes the main GET request
        /// </summary>
        /// <returns>The view</returns>
        private object ProcessGetRequest()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var services = ServiceHelper.LoadServices();
                var taskList = TaskHelper.LoadTasks();

                stopWatch.Stop();
                var data = new
                {
                    BaseData = GetBaseData(),
                    Services = services.OrderBy(o => o.Name),
                    ServiceCount = services.Count,
                    TaskList = taskList.OrderBy(o => o.Name),
                    TaskListCount = taskList.Count,
                    Duration = stopWatch.Elapsed.FormatString(),
                    TimeStamp = DateTime.Now
                };

                return View["entryList", data];
            }
            catch (Exception ex)
            {
                return ErrorMessage(ex);
            }
            finally
            {
                if (stopWatch.IsRunning)
                    stopWatch.Stop();
            }
        }

        /// <summary>
        /// Gets the object with the main link data like home, add, etc.
        /// </summary>
        /// <returns>The object with the data</returns>
        private object GetBaseData()
        {
            return new
            {
                Helper.Settings.Service.ServiceName,
                Helper.BasePath
            };
        }

        /// <summary>
        /// Returns the error view
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="caller">The caller (optional, auto fill)</param>
        /// <returns>The error message</returns>
        private object ErrorMessage(Exception ex, [CallerMemberName] string caller = "")
        {
            ServiceLogger.Error(caller, ex);
            var data = new
            {
                BaseData = GetBaseData(), 
                ex.Message
            };

            return View["error.html", data];
        }
    }
}
