using System;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;

namespace RethinkDb.Azure.Functions.Worker.Extensions
{
    /// <summary>
    /// Attribute used to define a trigger that binds to a RethinkDB table.
    /// </summary>
    public sealed class RethinkDbTriggerAttribute : TriggerBindingAttribute
    {
        /// <summary>
        /// The name of the database containing the table to which the parameter applies (may include binding parameters).
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// The name of the table to which the parameter applies (may include binding parameters).
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// The name of the app setting containing the hostname or IP address of the server to which the parameter applies.
        /// </summary>
        public string HostnameSetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the TCP port of the server to which the parameter applies.
        /// </summary>
        public string PortSetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the authorization key to the server to which the parameter applies.
        /// </summary>
        public string AuthorizationKeySetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the user account to connect as to the server to which the parameter applies.
        /// </summary>
        public string UserSetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the user account password to connect as to the server to which the parameter applies.
        /// </summary>
        public string PasswordSetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the value indicating if SSL/TLS encryption should be enabled for connection to the server.
        /// </summary>
        /// <remarks>The underlying driver (RethinkDb.Driver) requires a commercial license for SSL/TLS encryption.</remarks>
        public string EnableSslSetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the "license to" of underlying driver (RethinkDb.Driver) commercial license.
        /// </summary>
        public string LicenseToSetting { get; set; }

        /// <summary>
        /// The name of the app setting containing the "license key" of underlying driver (RethinkDb.Driver) commercial license.
        /// </summary>
        public string LicenseKeySetting { get; set; }

        /// <summary>
        /// The value indicating if <see cref="DocumentChange.Type"/> field should be included for <see cref="DocumentChange"/>.
        /// </summary>
        public bool IncludeTypes { get; set; } = false;

        /// <summary>
        /// Defines a trigger that binds to a RethinkDB table.
        /// </summary>
        /// <param name="databaseName">Name of the database of the table to which the parameter applies (may include binding parameters).</param>
        /// <param name="tableName">Name of the table to which the parameter applies (may include binding parameters).</param>
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
