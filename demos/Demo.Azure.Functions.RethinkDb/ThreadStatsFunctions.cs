using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using Demo.Azure.Functions.RethinkDb.Model;

namespace Demo.Azure.Functions.RethinkDb
{
    public static class ThreadStatsFunctions
    {
        [FunctionName("QueryThreadStatsByIdFromQueryString")]
        public static IActionResult QueryThreadStatsByIdFromQueryString(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "QueryThreadStatsById")] HttpRequest request,
            [RethinkDb(
                databaseName: "Demo",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword",
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

        [FunctionName("QueryThreadStatsByIdFromRouteData")]
        public static IActionResult QueryThreadStatsByIdFromRouteData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "QueryThreadStatsById/{id}")] HttpRequest request,
            [RethinkDb(
                databaseName: "Demo",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword",
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

        [FunctionName("StoreSingleThreadStats")]
        public static IActionResult StoreSingleThreadStats(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
            [RethinkDb(
                databaseName: "Demo",
                tableName: "ThreadStats",
                CreateIfNotExists = true,
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword")] out dynamic document,
            ILogger log)
        {
            Guid id = Guid.NewGuid();
            ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
            ThreadPool.GetMinThreads(out var minThreads, out var _);
            ThreadPool.GetMaxThreads(out var maxThreads, out var _);

            document = new
            {
                id,
                WorkerThreads = workerThreads,
                MinThreads = minThreads,
                MaxThreads = maxThreads,
                _source = nameof(ThreadStatsFunctions) + "." + nameof(StoreSingleThreadStats)
            };

            log.LogInformation("C# HTTP trigger function stored single thread stats.");

            return new ObjectResult(document);
        }

        [FunctionName("StoreThreadStatsSeries")]
        public static async Task<IActionResult> StoreThreadStatsSeries(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
            [RethinkDb(
                databaseName: "Demo",
                tableName: "ThreadStats",
                CreateIfNotExists = true,
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword")] IAsyncCollector<ThreadStats> threadStatsCollector,
            ILogger log)
        {
            Int32.TryParse(request.Query["count"], out int count);
            Int32.TryParse(request.Query["delay"], out int delay);

            for (int i = 0; i < count; i++)
            {
                ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
                ThreadPool.GetMinThreads(out var minThreads, out var _);
                ThreadPool.GetMaxThreads(out var maxThreads, out var _);

                await threadStatsCollector.AddAsync(new ThreadStats
                {
                    WorkerThreads = workerThreads,
                    MinThreads = minThreads,
                    MaxThreads = maxThreads
                });

                await Task.Delay(delay);
            }

            log.LogInformation($"C# HTTP trigger function stored {count} thread stats.");

            return new OkResult();
        }

        [FunctionName("HandleThreadStatsChange")]
        public static void HandleThreadStatsChange(
            [RethinkDbTrigger(
                databaseName: "Demo",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword")]DocumentChange change,
            ILogger log)
        {
            log.LogInformation("[ThreadStats Change Received] " + change.GetNewValue<ThreadStats>());
        }
    }
}
