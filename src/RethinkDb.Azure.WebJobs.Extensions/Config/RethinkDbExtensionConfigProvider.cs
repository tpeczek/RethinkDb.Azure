using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using RethinkDb.Azure.WebJobs.Extensions.Trigger;
using RethinkDb.Azure.WebJobs.Extensions.Services;

namespace RethinkDb.Azure.WebJobs.Extensions.Config
{
    [Extension("RethinkDB")]
    internal class RethinkDbExtensionConfigProvider : IExtensionConfigProvider
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly RethinkDbOptions _options;
        private readonly IRethinkDBConnectionFactory _rethinkDBConnectionFactory;
        private readonly INameResolver _nameResolver;
        private readonly ILoggerFactory _loggerFactory;
        #endregion

        #region Constructor
        public RethinkDbExtensionConfigProvider(IConfiguration configuration, IOptions<RethinkDbOptions> options, IRethinkDBConnectionFactory rethinkDBConnectionFactory, INameResolver nameResolver, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _options = options.Value;
            _rethinkDBConnectionFactory = rethinkDBConnectionFactory;
            _nameResolver = nameResolver;
            _loggerFactory = loggerFactory;
        }
        #endregion

        #region Methods
        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // RethinkDB Trigger
            var triggerAttributeBindingRule = context.AddBindingRule<RethinkDbTriggerAttribute>();
            triggerAttributeBindingRule.BindToTrigger<DocumentChange>(new RethinkDbTriggerAttributeBindingProvider(_configuration, _options, _rethinkDBConnectionFactory, _nameResolver, _loggerFactory));
        }
        #endregion
    }
}
