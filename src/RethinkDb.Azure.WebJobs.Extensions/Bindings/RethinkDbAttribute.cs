using System;
using Microsoft.Azure.WebJobs.Description;

namespace Microsoft.Azure.WebJobs
{
    /// <summary>
    /// Attribute used to bind to a RethinkDB table.
    /// </summary>
    /// <remarks>
    /// The method parameter type can be one of the following:
    /// <list type="bullet">
    /// <item><description>T</description></item>
    /// <item><description>out T</description></item>
    /// <item><description><see cref="ICollector{T}"/></description></item>
    /// <item><description><see cref="IAsyncCollector{T}"/></description></item>
    /// </list>
    /// </remarks>
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class RethinkDbAttribute : Attribute
    {
        /// <summary>
        /// The name of the database containing the table to which the parameter applies (may include binding parameters).
        /// </summary>
        [AutoResolve]
        public string DatabaseName { get; private set; }

        /// <summary>
        /// The name of the table to which the parameter applies (may include binding parameters).
        /// </summary>
        [AutoResolve]
        public string TableName { get; private set; }

        /// <summary>
        /// The hostname or IP address of the server to which the parameter applies.
        /// </summary>
        [AppSetting]
        public string HostnameSetting { get; set; }

        /// <summary>
        /// The TCP port of the server to which the parameter applies.
        /// </summary>
        [AppSetting]
        public string PortSetting { get; set; }

        /// <summary>
        /// The authorization key to the server to which the parameter applies.
        /// </summary>
        [AppSetting]
        public string AuthorizationKeySetting { get; set; }

        /// <summary>
        /// The user account to connect as to the server to which the parameter applies.
        /// </summary>
        [AppSetting]
        public string UserSetting { get; set; }

        /// <summary>
        /// The user account password to connect as to the server to which the parameter applies.
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
        /// The Id of the document to retrieve from the table (may include binding parameters).
        /// </summary>
        [AutoResolve]
        public string Id { get; set; }

        /// <summary>
        /// The value indicating if database and table should be automatically created if they do not exist (only applies to output bindings).
        /// </summary>
        public bool CreateIfNotExists { get; set; } = false;

        /// <summary>
        /// Binds parameter to a RethinkDB table.
        /// </summary>
        /// <param name="databaseName">Name of the database of the table to which the parameter applies (may include binding parameters).</param>
        /// <param name="tableName">Name of the table to which the parameter applies (may include binding parameters).</param>
        public RethinkDbAttribute(string databaseName, string tableName)
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
