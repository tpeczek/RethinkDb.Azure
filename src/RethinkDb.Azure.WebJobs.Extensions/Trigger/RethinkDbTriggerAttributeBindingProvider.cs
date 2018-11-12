using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using RethinkDb.Azure.WebJobs.Extensions.Services;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        #region Fields
        private const string UNABLE_TO_RESOLVE_APP_SETTING_FORMAT = "Unable to resolve app setting for property '{0}.{1}'. Make sure the app setting exists and has a valid value.";

        private readonly IConfiguration _configuration;
        private readonly RethinkDbOptions _options;
        private readonly IRethinkDBConnectionFactory _rethinkDBConnectionFactory;
        private readonly INameResolver _nameResolver;
        private readonly ILogger _logger;

        private readonly Task<ITriggerBinding> _nullTriggerBindingTask = Task.FromResult<ITriggerBinding>(null);
        #endregion

        #region Constructor
        public RethinkDbTriggerAttributeBindingProvider(IConfiguration configuration, RethinkDbOptions options, IRethinkDBConnectionFactory rethinkDBConnectionFactory, INameResolver nameResolver, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _options = options;
            _rethinkDBConnectionFactory = rethinkDBConnectionFactory;
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

            ConnectionOptions triggerConnectionOptions = ResolveTriggerConnectionOptions(triggerAttribute);
            Task<Connection> triggerConnectionTask = _rethinkDBConnectionFactory.GetConnectionAsync(triggerConnectionOptions);

            TableOptions triggerTableOptions = ResolveTriggerTableOptions(triggerAttribute);

            return Task.FromResult<ITriggerBinding>(new RethinkDbTriggerBinding(parameter, triggerConnectionTask, triggerTableOptions, triggerAttribute.IncludeTypes));
        }

        private ConnectionOptions ResolveTriggerConnectionOptions(RethinkDbTriggerAttribute triggerAttribute)
        {
            return new ConnectionOptions(
                ResolveTriggerAttributeHostname(triggerAttribute),
                ResolveTriggerAttributePort(triggerAttribute),
                ResolveTriggerAttributeSetting(triggerAttribute.AuthorizationKeySetting, _options.AuthorizationKey),
                ResolveTriggerAttributeSetting(triggerAttribute.UserSetting, _options.User),
                ResolveTriggerAttributeSetting(triggerAttribute.PasswordSetting, _options.Password)
            );
        }

        private string ResolveTriggerAttributeHostname(RethinkDbTriggerAttribute triggerAttribute)
        {
            string hostname = null;
            if (!String.IsNullOrEmpty(triggerAttribute.HostnameSetting))
            {
                hostname = _configuration.GetConnectionStringOrSetting(triggerAttribute.HostnameSetting);

                if (String.IsNullOrEmpty(hostname))
                {
                    throw new InvalidOperationException(String.Format(UNABLE_TO_RESOLVE_APP_SETTING_FORMAT, nameof(RethinkDbTriggerAttribute), nameof(RethinkDbTriggerAttribute.HostnameSetting)));
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

        private int? ResolveTriggerAttributePort(RethinkDbTriggerAttribute triggerAttribute)
        {
            if (!String.IsNullOrEmpty(triggerAttribute.PortSetting))
            {
                string portString = _configuration.GetConnectionStringOrSetting(triggerAttribute.PortSetting);

                if (String.IsNullOrEmpty(portString) || !Int32.TryParse(portString, out int port))
                {
                    throw new InvalidOperationException(String.Format(UNABLE_TO_RESOLVE_APP_SETTING_FORMAT, nameof(RethinkDbTriggerAttribute), nameof(RethinkDbTriggerAttribute.PortSetting)));
                }

                return port;
            }

            return _options.Port;
        }

        private string ResolveTriggerAttributeSetting(string settingName, string optionsValue)
        {
            if (!String.IsNullOrEmpty(settingName))
            {
                return _configuration.GetConnectionStringOrSetting(settingName);
            }

            return optionsValue;
        }

        private TableOptions ResolveTriggerTableOptions(RethinkDbTriggerAttribute triggerAttribute)
        {
            return new TableOptions(
                ResolveAttributeValue(triggerAttribute.DatabaseName),
                ResolveAttributeValue(triggerAttribute.TableName)
            );
        }

        private string ResolveAttributeValue(string attributeValue)
        {
            return _nameResolver.Resolve(attributeValue) ?? attributeValue;
        }
        #endregion
    }
}
