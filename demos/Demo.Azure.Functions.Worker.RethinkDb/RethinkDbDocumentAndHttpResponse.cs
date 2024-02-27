using Microsoft.Azure.Functions.Worker.Http;
using RethinkDb.Azure.Functions.Worker.Extensions;

namespace Demo.Azure.Functions.Worker.RethinkDb
{
    public sealed class RethinkDbDocumentAndHttpResponse
    {
        [RethinkDbOutput(databaseName: "Demo", tableName: "ThreadStats", CreateIfNotExists = true, HostnameSetting = "RethinkDbHostname", UserSetting = "RethinkDbUser", PasswordSetting = "RethinkDbPassword")]
        public object RethinkDbDocument { get; set;  }

        public HttpResponseData HttpResponse { get; set;  }
    }
}
