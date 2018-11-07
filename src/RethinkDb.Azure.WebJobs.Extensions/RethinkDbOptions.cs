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
    }
}
