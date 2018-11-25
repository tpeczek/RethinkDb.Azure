using System.Threading.Tasks;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Services
{
    internal interface IRethinkDBConnectionFactory
    {
        Task<IConnection> GetConnectionAsync(ConnectionOptions options);
    }
}
