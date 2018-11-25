using Newtonsoft.Json;

namespace RethinkDb.Azure.WebJobs.Extensions.Tests.Bindings.Infrastructure
{
    internal class Poco
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Property { get; set; }
    }
}
