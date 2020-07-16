using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using Microsoft.Win32.TaskScheduler;
using ServiceMonitor.DataObjects;
using ServiceMonitor.Global;
using ZimLabs.CoreLib;
using ZimLabs.CoreLib.Extensions;

namespace ServiceMonitor.Business
{
    /// <summary>
    /// Provides the functions to check the windows tasks
    /// </summary>
    internal class TaskHelper
    {
        /// <summary>
        /// Contains the path of the file which contains the list with the tasks
        /// </summary>
        private readonly string _filePath = Path.Combine(Core.GetBaseFolder(), "TaskList.json");

        /// <summary>
        /// Backing field for <see cref="TaskList"/>
        /// </summary>
        private List<TaskEntry> _taskList;

        /// <summary>
        /// Gets the list with the tasks
        /// </summary>
        public List<TaskEntry> TaskList
        {
            get
            {
                if (_taskList == null)
                    LoadTasks();

                return _taskList;
            }
        }

        /// <summary>
        /// Loads all tasks
        /// </summary>
        /// <returns>The list with all available tasks</returns>
        private IEnumerable<TaskEntry> LoadAllTasks()
        {
            using var taskService = new TaskService();
            return taskService.AllTasks.Select(s => (TaskEntry) s).ToList();
        }

        /// <summary>
        /// Loads the list with the desired windows tasks and their current sate
        /// </summary>
        /// <returns>The list with the tasks</returns>
        private void LoadTasks()
        {
            // Step 1: Get the desired tasks
            var tasks = Helper.LoadJsonFile<List<string>>(_filePath) ?? new List<string>();

            // Step 2: Get all tasks and return the specified 
            var allTasks = LoadAllTasks();

            _taskList = allTasks.Where(w => tasks.Any(a => a.EqualsIgnoreCase(w.Name))).ToList();
        }

        /// <summary>
        /// Adds a new task to the task list
        /// </summary>
        /// <param name="task">The task which should be added</param>
        public void AddTask(string task)
        {
            if (string.IsNullOrEmpty(task))
                throw new ArgumentNullException(nameof(task));

            // Step 1: Check if the task exists
            var tasks = LoadAllTasks();
            if (!tasks.Any(a => a.Name.EqualsIgnoreCase(task)))
                throw new EndpointNotFoundException();

            // Step 2: Load the tasks
            if (_taskList.Any(a => a.Name.EqualsIgnoreCase(task)))
                return;

            var tmpList = _taskList.Select(s => s.Name).ToList();
            tmpList.Add(task);

            // Save the current task list
            Helper.WriteJsonFile(_filePath, tmpList);
        }

        /// <summary>
        /// Deletes a task from the list
        /// </summary>
        /// <param name="task">The name of the task</param>
        public void DeleteTask(string task)
        {
            if (string.IsNullOrEmpty(task))
                throw new ArgumentNullException(nameof(task));

            var tmpTask = TaskList.FirstOrDefault(f => f.Name.EqualsIgnoreCase(task));
            if (tmpTask == null)
                return; // No task available, so everything is fine...

            TaskList.Remove(tmpTask);

            Helper.WriteJsonFile(_filePath, TaskList.Select(s => s.Name));
        }
    }
}
