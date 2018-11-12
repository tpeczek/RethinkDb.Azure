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
        private readonly ConcurrentDictionary<ConnectionOptions, Task<Connection>> _connectionCache = new ConcurrentDictionary<ConnectionOptions, Task<Connection>>();
        #endregion

        #region Methods
        public Task<Connection> GetConnectionAsync(ConnectionOptions options)
        {
            return _connectionCache.GetOrAdd(options, CreateConnectionAsync);
        }

        public void Dispose()
        {
            foreach(KeyValuePair<ConnectionOptions, Task<Connection>> cachedConnection in _connectionCache)
            {
                cachedConnection.Value.Result.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        private Task<Connection> CreateConnectionAsync(ConnectionOptions options)
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

            return connectionBuilder.ConnectAsync();
        }
        #endregion
    }
}
