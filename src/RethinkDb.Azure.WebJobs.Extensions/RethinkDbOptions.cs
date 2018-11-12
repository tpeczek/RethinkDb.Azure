namespace RethinkDb.Azure.WebJobs.Extensions
{
    /// <summary>
    /// Options for RethinkDB binding extensions.
    /// </summary>
    public class RethinkDbOptions
    {
        /// <summary>
        /// The hostname or IP address of the server containing the table to monitor.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The TCP port of the server containing the database and table to monitor.
        /// </summary>
        public int? Port { get; set; }
    }
}
