using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using RethinkDb.Azure.Functions.Worker.Extensions;
using Demo.Azure.Functions.Worker.RethinkDb.Model;

namespace Demo.Azure.Functions.Worker.RethinkDb
{
    public class ThreadStatsFunctions
    {
        private readonly ILogger<ThreadStatsFunctions> _logger;

        public ThreadStatsFunctions(ILogger<ThreadStatsFunctions> logger)
        {
            _logger = logger;
        }

        [Function("QueryThreadStatsByIdFromQueryString")]
        public IActionResult QueryThreadStatsByIdFromQueryString(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "QueryThreadStatsById")] HttpRequest request,
            [RethinkDbInput(
                databaseName: "Demo",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword",
                Id = "{Query.id}")] ThreadStats threadStats)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (threadStats == null)
            {
                _logger.LogInformation($"Thread stats not found (Query['id']: {request.Query["id"]}).");
                return new NotFoundResult();
            }

            _logger.LogInformation($"Thread stats found (Query['id']: {request.Query["id"]}).");

            return new OkObjectResult(threadStats);
        }

        [Function("QueryThreadStatsByIdFromRouteData")]
        public IActionResult QueryThreadStatsByIdFromRouteData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "QueryThreadStatsById/{id}")] HttpRequest request,
            [RethinkDbInput(
                databaseName: "Demo",
                tableName: "ThreadStats",
                HostnameSetting = "RethinkDbHostname",
                UserSetting = "RethinkDbUser",
                PasswordSetting = "RethinkDbPassword",
                Id = "{id}")] ThreadStats threadStats)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (threadStats == null)
            {
                _logger.LogInformation($"Thread stats not found.");
                return new NotFoundResult();
            }

            _logger.LogInformation($"Thread stats found.");

            return new OkObjectResult(threadStats);
        }

        [Function("StoreSingleThreadStats")]
        public RethinkDbDocumentAndHttpResponse StoreSingleThreadStats(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData request)
        {
            Guid id = Guid.NewGuid();
            ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
            ThreadPool.GetMinThreads(out var minThreads, out var _);
            ThreadPool.GetMaxThreads(out var maxThreads, out var _);

            var document = new
            {
                id,
                WorkerThreads = workerThreads,
                MinThreads = minThreads,
                MaxThreads = maxThreads,
                _source = nameof(ThreadStatsFunctions) + "." + nameof(StoreSingleThreadStats)
            };

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(document);

            _logger.LogInformation("C# HTTP trigger function stored single thread stats.");

            return new RethinkDbDocumentAndHttpResponse
            {
                RethinkDbDocument = document,
                HttpResponse = response
            };
        }

        [Function("StoreThreadStatsSeries")]
        public async Task<RethinkDbMultipleDocuments> StoreThreadStatsSeries(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            Int32.TryParse(request.Query["count"], out int count);
            Int32.TryParse(request.Query["delay"], out int delay);

            var documents = new List<ThreadStats>();

            for (int i = 0; i < count; i++)
            {
                ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
                ThreadPool.GetMinThreads(out var minThreads, out var _);
                ThreadPool.GetMaxThreads(out var maxThreads, out var _);

                documents.Add(new ThreadStats
                {
                    WorkerThreads = workerThreads,
                    MinThreads = minThreads,
                    MaxThreads = maxThreads
                });

                await Task.Delay(delay);
            }

            _logger.LogInformation($"C# HTTP trigger function stored {count} thread stats.");

            return new RethinkDbMultipleDocuments
            {
                RethinkDbDocuments = documents
            };
        }
    }
}
