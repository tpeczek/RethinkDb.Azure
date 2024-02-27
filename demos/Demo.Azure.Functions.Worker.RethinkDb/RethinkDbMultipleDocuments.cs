using RethinkDb.Azure.Functions.Worker.Extensions;
using Demo.Azure.Functions.Worker.RethinkDb.Model;

namespace Demo.Azure.Functions.Worker.RethinkDb
{
    public class RethinkDbMultipleDocuments
    {
        [RethinkDbOutput(databaseName: "Demo", tableName: "ThreadStats", CreateIfNotExists = true, HostnameSetting = "RethinkDbHostname", UserSetting = "RethinkDbUser", PasswordSetting = "RethinkDbPassword")]
        public IEnumerable<ThreadStats> RethinkDbDocuments { get; set; }
    }
}
