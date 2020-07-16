using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Nancy;
using Nancy.ModelBinding;
using ServiceMonitor.Business;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;

namespace ServiceMonitor.Modules
{
    /// <summary>
    /// Represents the main model of the REST service
    /// </summary>
    public sealed class MainModule : NancyModule
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MainModule"/>
        /// </summary>
        public MainModule()
        {
            Get("/", _ => ProcessGetRequest());

            Post("/service", _ => AddService());

            Post("/task", _ => AddTask());
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
        /// Adds a new service to the list
        /// </summary>
        /// <returns>The response</returns>
        private object AddService()
        {
            try
            {
                var data = this.Bind<ServiceEntry>();

                var result = ServiceHelper.AddService(data);

                if (result.successful)
                    return Response.AsJson(new
                    {
                        Code = 200,
                        Message = "Service saved"
                    });
                else

            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Adds a new task to the list
        /// </summary>
        /// <returns>The response</returns>
        private object AddTask()
        {
            try
            {

            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
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

        /// <summary>
        /// Returns an error response
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The response</returns>
        private object ErrorResponse(Exception ex)
        {
            var data = new
            {
                StautsCode = 500,
                Message = "An error has occured. Please check 'ErrorMessage' for more details.",
                ErrorMessage = ex.Message
            };

            return Response.AsJson(data, HttpStatusCode.InternalServerError);
        }
    }
}
