using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly RethinkDbOptions _options;
        private readonly INameResolver _nameResolver;
        private readonly ILogger _logger;
        private readonly Task<ITriggerBinding> _nullTriggerBindingTask = Task.FromResult<ITriggerBinding>(null);
        #endregion

        #region Constructor
        public RethinkDbTriggerAttributeBindingProvider(IConfiguration configuration, RethinkDbOptions options, INameResolver nameResolver, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _options = options;
            _nameResolver = nameResolver;
            _logger = loggerFactory.CreateLogger(LogCategories.CreateTriggerCategory("RethinkDB"));
        }
        #endregion

        #region Methods
        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            ParameterInfo parameter = context.Parameter;

            RethinkDbTriggerAttribute triggerAttribute = parameter.GetCustomAttribute<RethinkDbTriggerAttribute>(inherit: false);
            if (triggerAttribute is null)
            {
                return _nullTriggerBindingTask;
            }

            Driver.Net.Connection rethinkDbConnection = Driver.RethinkDB.R.Connection()
                .Hostname(ResolveTriggerAttributeHostname(triggerAttribute))
                .Connect();

            string triggerDatabaseName = ResolveAttributeValue(triggerAttribute.DatabaseName);
            string triggerTableName = ResolveAttributeValue(triggerAttribute.TableName);

            return Task.FromResult<ITriggerBinding>(new RethinkDbTriggerBinding(parameter, rethinkDbConnection, triggerDatabaseName, triggerTableName, _logger));
        }

        private string ResolveTriggerAttributeHostname(RethinkDbTriggerAttribute triggerAttribute)
        {
            string hostname = null;
            if (!String.IsNullOrEmpty(triggerAttribute.HostnameSetting))
            {
                hostname = _configuration.GetConnectionStringOrSetting(triggerAttribute.HostnameSetting);

                if (String.IsNullOrEmpty(hostname))
                {
                    throw new InvalidOperationException($"Unable to resolve app setting for property '{nameof(RethinkDbTriggerAttribute)}.{nameof(RethinkDbTriggerAttribute.HostnameSetting)}'. Make sure the app setting exists and has a valid value.");
                }

                return hostname;
            }

            hostname = _options.Hostname;

            if (String.IsNullOrEmpty(hostname))
            {
                throw new InvalidOperationException($"The RethinkDbTriggerAttribute hostname must be set either via a '{nameof(RethinkDbOptions.Hostname)}' configuration setting, via the '{nameof(RethinkDbTriggerAttribute)}.{nameof(RethinkDbTriggerAttribute.HostnameSetting)}' property or via '{nameof(RethinkDbOptions)}.{nameof(RethinkDbOptions.Hostname)}'.");
            }

            return hostname;
        }

        private string ResolveAttributeValue(string attributeValue)
        {
            return _nameResolver.Resolve(attributeValue) ?? attributeValue;
        }
        #endregion
    }
}
