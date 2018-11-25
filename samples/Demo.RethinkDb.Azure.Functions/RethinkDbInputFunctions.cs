using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbInputFunctions
    {
        [FunctionName("QueryByIdFromQueryString")]
        public static IActionResult QueryByIdFromQueryString(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                Id = "{Query.id}")] ThreadStats threadStats,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (threadStats == null)
            {
                log.LogInformation($"Thread stats not found (Query['id']: {request.Query["id"]}).");
                return new NotFoundResult();
            }

            log.LogInformation($"Thread stats found (Query['id']: {request.Query["id"]}).");
            return new ObjectResult(threadStats);
        }

        [FunctionName("QueryByIdFromRouteData")]
        public static IActionResult QueryByIdFromRouteData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "QueryByIdFromRouteData/{id}")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                Id = "{id}")] ThreadStats threadStats,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (threadStats == null)
            {
                log.LogInformation($"Thread stats not found.");
                return new NotFoundResult();
            }

            log.LogInformation($"Thread stats found.");
            return new ObjectResult(threadStats);
        }
    }
}
