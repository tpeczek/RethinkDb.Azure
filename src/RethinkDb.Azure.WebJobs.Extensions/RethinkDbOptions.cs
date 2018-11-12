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
    }
}
