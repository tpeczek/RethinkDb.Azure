using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerBinding : ITriggerBinding
    {
        #region Fields
        private static readonly Type DOCUMENTCHANGE_TYPE = typeof(DocumentChange);
        private static readonly IReadOnlyDictionary<string, object> EMPTY_BINDING_DATA = new Dictionary<string, object>();

        private readonly ParameterInfo _parameter;
        private readonly Task<IConnection> _rethinkDbConnectionTask;
        private readonly TableOptions _rethinkDbTableOptions;
        private readonly Driver.Ast.Table _rethinkDbTable;
        private readonly bool _includeTypes;
        #endregion

        #region Properties
        public Type TriggerValueType => DOCUMENTCHANGE_TYPE;

        public IReadOnlyDictionary<string, Type> BindingDataContract { get; } = new Dictionary<string, Type>();
        #endregion

        #region Constructor
        public RethinkDbTriggerBinding(ParameterInfo parameter, Task<IConnection> rethinkDbConnectionTask, TableOptions rethinkDbTableOptions, bool includeTypes)
        {
            _parameter = parameter;
            _rethinkDbConnectionTask = rethinkDbConnectionTask;
            _rethinkDbTableOptions = rethinkDbTableOptions;
            _rethinkDbTable = Driver.RethinkDB.R.Db(_rethinkDbTableOptions.DatabaseName).Table(_rethinkDbTableOptions.TableName);
            _includeTypes = includeTypes;
        }
        #endregion

        #region Methods
        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            IValueProvider valueBinder = new RethinkDbTriggerValueBinder(_parameter, value);

            return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, EMPTY_BINDING_DATA));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Task.FromResult<IListener>(new RethinkDbTriggerListener(context.Descriptor.Id, context.Executor, _rethinkDbConnectionTask, _rethinkDbTable, _includeTypes));

        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RethinkDbTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                Type = RethinkDbTriggerParameterDescriptor.TRIGGER_NAME,
                TableName = _rethinkDbTableOptions.TableName
            };
        }
        #endregion
    }
}
