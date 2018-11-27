using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Demo.RethinkDb.Azure.Functions.Model;

namespace Demo.RethinkDb.Azure.Functions
{
    public static class RethinkDbOutputFunctions
    {
        [FunctionName("OutputSingleDoc")]
        public static IActionResult OutputSingleDoc(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname")] out dynamic document,
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
                _source = nameof(RethinkDbOutputFunctions) + "." + nameof(OutputSingleDoc)
            };

            log.LogInformation("C# HTTP trigger function inserted single document");

            return new ObjectResult(document);
        }

        [FunctionName("OutputMultipleDocs")]
        public static async Task<IActionResult> OutputMultipleDocs(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest request,
            [RethinkDb(
                databaseName: "Demo_AspNetCore_Changefeed_RethinkDB",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname")] IAsyncCollector<ThreadStats> threadStatsCollector,
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

            log.LogInformation($"C# HTTP trigger function inserted {count} documents");

            return new OkResult();
        }
    }
}
