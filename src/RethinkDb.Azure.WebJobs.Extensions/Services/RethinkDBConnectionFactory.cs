using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Services
{
    internal class RethinkDBConnectionFactory : IRethinkDBConnectionFactory, IDisposable
    {
        #region Fields
        private readonly ConcurrentDictionary<ConnectionOptions, Task<IConnection>> _connectionCache = new ConcurrentDictionary<ConnectionOptions, Task<IConnection>>();
        #endregion

        #region Methods
        public Task<IConnection> GetConnectionAsync(ConnectionOptions options)
        {
            return _connectionCache.GetOrAdd(options, CreateConnectionAsync);
        }

        public void Dispose()
        {
            foreach(KeyValuePair<ConnectionOptions, Task<IConnection>> cachedConnection in _connectionCache)
            {
                cachedConnection.Value.Result.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        private Task<IConnection> CreateConnectionAsync(ConnectionOptions options)
        {
            Connection.Builder connectionBuilder = Driver.RethinkDB.R.Connection()
                .Hostname(options.Hostname);

            if (options.Port.HasValue)
            {
                connectionBuilder.Port(options.Port.Value);
            }

            if (!(options.AuthorizationKey is null))
            {
                connectionBuilder.AuthKey(options.AuthorizationKey);
            }

            if (!(options.User is null))
            {
                connectionBuilder.User(options.User, options.Password);
            }

            if (options.EnableSsl)
            {
                connectionBuilder.EnableSsl(new SslContext(), options.LicenseTo, options.LicenseKey);
            }

            return connectionBuilder.ConnectAsync().ContinueWith(t => (IConnection)t.Result);
        }
        #endregion
    }
}
