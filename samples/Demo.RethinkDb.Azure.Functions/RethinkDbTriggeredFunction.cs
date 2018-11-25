using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbTriggeredFunction
    {
        [FunctionName("RethinkDbTriggeredFunction")]
        public static void Run([RethinkDbTrigger(
            databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
            tableName: "ThreadStats",
            HostnameSetting = "RethinkDbHostname")]DocumentChange change,
            ILogger log)
        {
            log.LogInformation("[ThreadStats Change Received] " + change.GetNewValue<ThreadStats>());
        }
    }
}
