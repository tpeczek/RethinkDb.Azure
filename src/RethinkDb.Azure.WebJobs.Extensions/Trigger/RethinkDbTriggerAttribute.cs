using System;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs
{
    /// <summary>
    /// Defines the [RethinkDbTrigger] attribute.
    /// </summary>
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RethinkDbTriggerAttribute : Attribute
    {
        /// <summary>
        /// Name of the database containing the table to monitor for changes.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Name of the table to monitor for changes.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// The hostname or IP address of the server containing the database and table to monitor.
        /// </summary>
        [AppSetting]
        public string HostnameSetting { get; set; }

        /// <summary>
        /// Triggers an event when changes occur on a monitored table
        /// </summary>
        /// <param name="databaseName">Name of the database of the table to monitor for changes.</param>
        /// <param name="tableName">Name of the table to monitor for changes.</param>
        public RethinkDbTriggerAttribute(string databaseName, string tableName)
        {
            if (String.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            if (String.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            DatabaseName = databaseName;
            TableName = tableName;
        }
    }
}
