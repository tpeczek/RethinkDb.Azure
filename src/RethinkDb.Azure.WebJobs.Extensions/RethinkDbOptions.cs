namespace RethinkDb.Azure.WebJobs.Extensions
{
    /// <summary>
    /// Options for RethinkDB binding extensions.
    /// </summary>
    public class RethinkDbOptions
    {
        /// <summary>
        /// The hostname or IP address of the server.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The TCP port of the server.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// The authorization key to the server.
        /// </summary>
        public string AuthorizationKey { get; set; }

        /// <summary>
        /// The user account to connect as to the server.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The user account password to connect as to the server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The value indicating if SSL/TLS encryption should be enabled for connection to the server.
        /// </summary>
        /// <remarks>The underlying driver (RethinkDb.Driver) requires a commercial license for SSL/TLS encryption, <see cref="LicenseTo"/> and <see cref="LicenseKey"/> must be set.</remarks>
        public bool EnableSsl { get; set; } = false;

        /// <summary>
        /// The "license to" of underlying driver (RethinkDb.Driver) commercial license.
        /// </summary>
        public string LicenseTo { get; set; }

        /// <summary>
        /// The "license key" of underlying driver (RethinkDb.Driver) commercial license.
        /// </summary>
        public string LicenseKey { get; set; }
    }
}
