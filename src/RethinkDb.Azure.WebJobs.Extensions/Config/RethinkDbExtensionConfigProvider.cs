using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RethinkDb.Azure.WebJobs.Extensions.Model;
using RethinkDb.Azure.WebJobs.Extensions.Trigger;
using RethinkDb.Azure.WebJobs.Extensions.Bindings;
using RethinkDb.Azure.WebJobs.Extensions.Services;

namespace RethinkDb.Azure.WebJobs.Extensions.Config
{
    [Extension("RethinkDB")]
    internal class RethinkDbExtensionConfigProvider : IExtensionConfigProvider
    {
        #region Classes
        private class RethinkDbOpenType : OpenType.Poco
        {
            public override bool IsMatch(Type type, OpenTypeMatchContext context)
            {
                if (type.FullName == "System.Object")
                {
                    return true;
                }

                return base.IsMatch(type, context);
            }
        }
        #endregion

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

            // RethinkDB Bindings
            var bindingAttributeBindingRule = context.AddBindingRule<RethinkDbAttribute>();
            bindingAttributeBindingRule.AddValidator(ValidateHost);
            bindingAttributeBindingRule.BindToCollector<RethinkDbOpenType>(typeof(RethinkDbCollectorConverter<>), _options, _rethinkDBConnectionFactory);
            bindingAttributeBindingRule.WhenIsNotNull(nameof(RethinkDbAttribute.Id))
                .BindToValueProvider(CreateValueBinderAsync);

            // RethinkDB Trigger
            var triggerAttributeBindingRule = context.AddBindingRule<RethinkDbTriggerAttribute>();
            triggerAttributeBindingRule.BindToTrigger(new RethinkDbTriggerAttributeBindingProvider(_configuration, _options, _rethinkDBConnectionFactory, _nameResolver, _loggerFactory));
        }

        private void ValidateHost(RethinkDbAttribute attribute, Type paramType)
        {
            if (String.IsNullOrEmpty(_options.Hostname) && String.IsNullOrEmpty(attribute.HostnameSetting))
            {
                string attributeProperty = $"{nameof(RethinkDbAttribute)}.{nameof(RethinkDbAttribute.HostnameSetting)}";
                string optionsProperty = $"{nameof(RethinkDbOptions)}.{nameof(RethinkDbOptions.Hostname)}";
                throw new InvalidOperationException($"The RethinkDB hostname must be set either via the {attributeProperty} property or via {optionsProperty}.");
            }
        }

        private Task<IValueBinder> CreateValueBinderAsync(RethinkDbAttribute attribute, Type type)
        {
            if (String.IsNullOrEmpty(attribute.Id))
            {
                throw new InvalidOperationException($"The '{nameof(RethinkDbAttribute.Id)}' property of a RethinkDB single-item input binding cannot be null or empty.");
            }

            ConnectionOptions connectionOptions = ConnectionOptionsBuilder.Build(attribute, _options);
            Type valyeBinderType = typeof(RethinkDbValueBinder<>).MakeGenericType(type);
            IValueBinder valueBinder = (IValueBinder)Activator.CreateInstance(valyeBinderType, attribute, _rethinkDBConnectionFactory.GetConnectionAsync(connectionOptions));

            return Task.FromResult(valueBinder);
        }
        #endregion
    }
}
