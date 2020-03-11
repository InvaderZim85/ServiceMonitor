using System;
using Microsoft.Win32.TaskScheduler;

namespace ServiceMonitor.DataObjects
{
    /// <summary>
    /// Represents a task entry
    /// </summary>
    internal sealed class TaskEntry
    {
        /// <summary>
        /// Gets or sets the name of the task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates if the task is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the last run time
        /// </summary>
        public DateTime LastRun { get; set; }

        /// <summary>
        /// Gets or sets the next run time
        /// </summary>
        public DateTime NextRun { get; set; }

        /// <summary>
        /// Gets or sets the state of the task
        /// </summary>
        public TaskState State { get; set; }

        /// <summary>
        /// Converts a <see cref="Task"/> object into a <see cref="TaskEntry"/> object
        /// </summary>
        /// <param name="task">The <see cref="Task"/> object</param>
        public static explicit operator TaskEntry(Task task)
        {
            if (task == null)
                return new TaskEntry();

            return new TaskEntry
            {
                Name = task.Name,
                Enabled = task.Enabled,
                LastRun = task.LastRunTime,
                NextRun = task.NextRunTime,
                State = task.State
            };
        }
    }
}
