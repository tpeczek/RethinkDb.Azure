using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Demo.Azure.Functions.Worker.RethinkDb
{
    public class ThreadStatsFunctions
    {
        private readonly ILogger<ThreadStatsFunctions> _logger;

        public ThreadStatsFunctions(ILogger<ThreadStatsFunctions> logger)
        {
            _logger = logger;
        }

        [Function("StoreSingleThreadStats")]
        public IActionResult StoreSingleThreadStats(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            Guid id = Guid.NewGuid();
            ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
            ThreadPool.GetMinThreads(out var minThreads, out var _);
            ThreadPool.GetMaxThreads(out var maxThreads, out var _);

            _logger.LogInformation("C# HTTP trigger function stored single thread stats.");

            return new OkResult();
        }
    }
}
