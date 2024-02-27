using RethinkDb.Azure.Functions.Worker.Extensions;

namespace Demo.Azure.Functions.Worker.RethinkDb
{
    public class RethinkDbMultipleDocuments
    {
        [RethinkDbOutput(databaseName: "Demo", tableName: "ThreadStats", CreateIfNotExists = true, HostnameSetting = "RethinkDbHostname", UserSetting = "RethinkDbUser", PasswordSetting = "RethinkDbPassword")]
        public IEnumerable<object> RethinkDbDocuments { get; set; }
    }
}
