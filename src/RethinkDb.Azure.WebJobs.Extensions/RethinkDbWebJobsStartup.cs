using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using RethinkDb.Azure.WebJobs.Extensions;

[assembly: WebJobsStartup(typeof(RethinkDbWebJobsStartup))]

namespace RethinkDb.Azure.WebJobs.Extensions
{
    /// <summary>
    /// Class defining a startup configuration action for RethinkDB binding extensions, which will be performed as part of host startup.
    /// </summary>
    public class RethinkDbWebJobsStartup : IWebJobsStartup
    {
        /// <summary>
        /// Performs the startup configuration action for RethinkDB binding extensions.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> that can be used to configure the host.</param>
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddRethinkDb();
        }
    }
}
