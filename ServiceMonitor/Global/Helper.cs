using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceMonitor.DataObjects;
using ZimLabs.CoreLib;

namespace ServiceMonitor.Global
{
    /// <summary>
    /// Provides several helper functions
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Backing field for <see cref="Settings"/>
        /// </summary>
        private static Settings _settings;

        /// <summary>
        /// Gets the settings of the service
        /// </summary>
        public static Settings Settings => _settings ?? (_settings = LoadSettings());

        /// <summary>
        /// Gets the base path of the application
        /// </summary>
        public static string BasePath => $"http://{Environment.MachineName.ToLower()}:{Settings.Rest.Port}/{Settings.Rest.ServiceName}/";

        /// <summary>
        /// Gets the host path
        /// </summary>
        public static string HostPath => $"http://localhost:{Settings.Rest.Port}/{Settings.Rest.ServiceName}/";

        /// <summary>
        /// Loads the settings of the service
        /// </summary>
        /// <returns>The settings</returns>
        private static Settings LoadSettings()
        {
            var path = Path.Combine(Core.GetBaseFolder(), "Settings.json");

            return LoadJsonFile<Settings>(path);
        }

        /// <summary>
        /// Loads the content of a JSON file
        /// </summary>
        /// <typeparam name="T">The type of the content</typeparam>
        /// <param name="path">The path of the file</param>
        /// <returns>The content</returns>
        public static T LoadJsonFile<T>(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException($"The desired file '{path}' is missing.");

            var content = File.ReadAllText(path, Encoding.UTF8);

            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// Writes an object as JSON formatted string into the specified file
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="content">The content</param>
        public static void WriteJsonFile(string path, object content)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var tmpContent = content == null ? "" : JsonConvert.SerializeObject(content, Formatting.Indented);

            File.WriteAllText(path, tmpContent);
        }

        /// <summary>
        /// Formats the given time span into a readable format
        /// </summary>
        /// <param name="timespan">The time span</param>
        /// <returns>The formatted value</returns>
        public static string FormatString(this TimeSpan timespan)
        {
            return $"{timespan.Hours:00}:{timespan.Minutes:00}:{timespan.Seconds:00}.{timespan.Milliseconds:000}";
        }
    }
}
