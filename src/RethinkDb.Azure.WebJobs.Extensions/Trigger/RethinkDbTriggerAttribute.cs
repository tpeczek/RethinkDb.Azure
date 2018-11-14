using System;
using Microsoft.Azure.WebJobs.Description;
using RethinkDb.Azure.WebJobs.Extensions.Model;

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
        /// The name of the database containing the table to monitor for changes.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// The name of the table to monitor for changes.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// The hostname or IP address of the server containing the database and table to monitor.
        /// </summary>
        [AppSetting]
        public string HostnameSetting { get; set; }

        /// <summary>
        /// The TCP port of the server containing the database and table to monitor.
        /// </summary>
        [AppSetting]
        public string PortSetting { get; set; }

        /// <summary>
        /// The authorization key to the server containing the database and table to monitor.
        /// </summary>
        [AppSetting]
        public string AuthorizationKeySetting { get; set; }

        /// <summary>
        /// The user account to connect as to the server containing the database and table to monitor.
        /// </summary>
        [AppSetting]
        public string UserSetting { get; set; }

        /// <summary>
        /// The user account password to connect as to the server containing the database and table to monitor.
        /// </summary>
        [AppSetting]
        public string PasswordSetting { get; set; }

        /// <summary>
        /// The value indicating if SSL/TLS encryption should be enabled for connection to the server.
        /// </summary>
        /// <remarks>The underlying driver (RethinkDb.Driver) requires a commercial license for SSL/TLS encryption.</remarks>
        [AppSetting]
        public string EnableSslSetting { get; set; }

        /// <summary>
        /// The "license to" of underlying driver (RethinkDb.Driver) commercial license.
        /// </summary>
        [AppSetting]
        public string LicenseToSetting { get; set; }

        /// <summary>
        /// The "license key" of underlying driver (RethinkDb.Driver) commercial license.
        /// </summary>
        [AppSetting]
        public string LicenseKeySetting { get; set; }

        /// <summary>
        /// The value indicating if <see cref="DocumentChange.Type"/> field should be included for <see cref="DocumentChange"/>.
        /// </summary>
        public bool IncludeTypes { get; set; } = false;

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
