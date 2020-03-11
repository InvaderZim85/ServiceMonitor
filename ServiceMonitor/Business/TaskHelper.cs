using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;
using ZimLabs.Utility.Extensions;

namespace ServiceMonitor.Business
{
    /// <summary>
    /// Provides the functions to check the windows tasks
    /// </summary>
    internal static class TaskHelper
    {
        /// <summary>
        /// Loads the list with the desired windows tasks and their current sate
        /// </summary>
        /// <returns>The list with the tasks</returns>
        public static List<TaskEntry> LoadTasks()
        {
            // Step 1: Get the desired tasks
            var path = Path.Combine(ZimLabs.Utility.Global.GetBaseFolder(), "TaskList.json");
            var tasks = Helper.LoadJsonFile<List<string>>(path);

            // Step 2: Get all tasks and return the specified 
            using (var taskService = new TaskService())
            {
                var allTasks = taskService.AllTasks;

                return allTasks.Where(w => tasks.Any(a => a.EqualsIgnoreCase(w.Name))).Select(s => (TaskEntry) s)
                    .ToList();
            }
        }
    }
}
