using System;
using Microsoft.Azure.WebJobs;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using RethinkDb.Azure.WebJobs.Extensions.Services;

namespace RethinkDb.Azure.WebJobs.Extensions.Bindings
{
    internal class RethinkDbCollectorConverter<T> : IConverter<RethinkDbAttribute, IAsyncCollector<T>>
    {
        #region Fields
        private readonly RethinkDbOptions _options;
        private readonly IRethinkDBConnectionFactory _rethinkDBConnectionFactory;
        #endregion

        #region Constructor
        public RethinkDbCollectorConverter(RethinkDbOptions options, IRethinkDBConnectionFactory rethinkDBConnectionFactory)
        {
            _options = options;
            _rethinkDBConnectionFactory = rethinkDBConnectionFactory ?? throw new ArgumentNullException(nameof(rethinkDBConnectionFactory));
        }
        #endregion

        #region Methods
        public IAsyncCollector<T> Convert(RethinkDbAttribute attribute)
        {
            ConnectionOptions connectionOptions = ConnectionOptionsBuilder.Build(attribute, _options);

            return new RethinkDbAsyncCollector<T>(attribute, _rethinkDBConnectionFactory.GetConnectionAsync(connectionOptions));
        }
        #endregion
    }
}
