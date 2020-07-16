using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Routing;
using ServiceMonitor.Business;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;
using ZimLabs.NancyFx.RestHelper.DataObjects;
using ResponseMessage = ServiceMonitor.DataObjects.ResponseMessage;

namespace ServiceMonitor.Modules
{
    /// <summary>
    /// Represents the main model of the REST service
    /// </summary>
    public sealed class MainModule : NancyModule
    {
        /// <summary>
        /// The instance for the interaction with the services
        /// </summary>
        private readonly ServiceHelper _serviceHelper = new ServiceHelper();

        /// <summary>
        /// The instance for the interaction with the tasks
        /// </summary>
        private readonly TaskHelper _taskHelper = new TaskHelper();

        /// <summary>
        /// Creates a new instance of the <see cref="MainModule"/>
        /// </summary>
        [RouteDescription("main", "Returns a view with the service and task information", "HTML")]
        [RouteDescription("postService", "Adds a new service to the service list", "JSON", 
            new [] { HttpStatusCode.OK}, 
            new [] { HttpStatusCode.BadRequest, HttpStatusCode.NotFound })]
        [RouteDescription("postTask", "Adds a new task to the task list", "JSON",
            new[] { HttpStatusCode.OK },
            new[] { HttpStatusCode.BadRequest, HttpStatusCode.NotFound })]
        [RouteDescription("deleteService", "Removes a service from the service list according to the given name.", "JSON",
            new[] { HttpStatusCode.OK },
            new[] { HttpStatusCode.BadRequest })]
        [RouteDescription("deleteTask", "Removes a task from the task list according to the given name.", "JSON",
            new[] { HttpStatusCode.OK },
            new[] { HttpStatusCode.BadRequest })]
        [RouteDescription("docRoute", "Returns this information page.", "HTML")]
        public MainModule(IRouteCacheProvider cacheProvider)
        {
            Get("/", _ => ProcessGetRequest(), name: "main");

            Post("/service/", _ => AddService(), name: "postService");

            Post("/task/{name}", x => AddTask(x.Name), name: "postTask");

            Delete("/service/{name}", x => DeleteService(x.Name), name: "deleteService");

            Delete("/task/{name}", x => DeleteTask(x.Name), name: "deleteTask");

            Get("/docs", _ => ShowInfo(cacheProvider), name: "docRoute");
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
                stopWatch.Stop();
                var data = new
                {
                    BaseData = GetBaseData(),
                    Services = _serviceHelper.ServiceList.OrderBy(o => o.Name),
                    ServiceCount = _serviceHelper.ServiceList.Count,
                    TaskList = _taskHelper.TaskList.OrderBy(o => o.Name),
                    TaskListCount = _taskHelper.TaskList.Count,
                    Duration = stopWatch.Elapsed.FormatString(),
                    TimeStamp = DateTime.Now
                };

                return View["entryList", data];
            }
            catch (Exception ex)
            {
                return ErrorView(ex);
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
        /// <param name="name">The name of the new service</param>
        /// <returns>The response</returns>
        private object AddService()
        {
            try
            {
                var payload = this.Bind<ServiceEntry>();
                if (payload == null)
                    return ResponseMessage("Missing payload.", HttpStatusCode.BadRequest);

                _serviceHelper.AddService(payload);

                return ResponseMessage("Service added.");
            }
            catch (ModelBindingException ex)
            {
                return ErrorResponse(ex);
            }
            catch (EndpointNotFoundException)
            {
                return ResponseMessage("The specified service doesn't exist.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Deletes a service
        /// </summary>
        /// <param name="name">The name of the service</param>
        /// <returns>The response</returns>
        private object DeleteService(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return ResponseMessage("The name of the desired service is missing.", HttpStatusCode.BadRequest);

                _serviceHelper.DeleteService(name);

                return ResponseMessage("Service deleted.");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Adds a new task to the list
        /// </summary>
        /// <param name="name">The name of the task</param>
        /// <returns>The response</returns>
        private object AddTask(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return ResponseMessage("The name of the desired task is missing.", HttpStatusCode.BadRequest);

                _taskHelper.AddTask(name);

                return ResponseMessage("Task added");
            }
            catch (EndpointNotFoundException)
            {
                return ResponseMessage("The specified task doesn't exist.", HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="name">The name of the task</param>
        /// <returns>The response</returns>
        private object DeleteTask(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    return ResponseMessage("The name of the desired task is missing.", HttpStatusCode.BadRequest);

                _taskHelper.DeleteTask(name);

                return ResponseMessage("Task deleted");
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex);
            }
        }

        /// <summary>
        /// Returns the information about the services
        /// </summary>
        /// <param name="cacheProvider">The cache provider</param>
        /// <returns>The response</returns>
        private object ShowInfo(IRouteCacheProvider cacheProvider)
        {
            var metaData = cacheProvider.GetCache().RetrieveMetadata<CustomMetadata>().Where(w => w != null).ToList();

            var data = new
            {
                BaseData = GetBaseData(),
                Metadata = metaData
            };

            return View["info.html", data];
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
        /// Returns a response message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="code">The status code (optional)</param>
        /// <returns>The response</returns>
        private object ResponseMessage(string message, HttpStatusCode code = HttpStatusCode.OK)
        {
            return Response.AsJson(new ResponseMessage(message, code), code);
        }

        /// <summary>
        /// Returns the error view
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="caller">The caller (optional, auto fill)</param>
        /// <returns>The error message</returns>
        private object ErrorView(Exception ex, [CallerMemberName] string caller = "")
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
            return Response.AsJson(new ResponseMessage(ex), HttpStatusCode.InternalServerError);
        }
    }
}
