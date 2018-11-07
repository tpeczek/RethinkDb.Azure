using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerBinding : ITriggerBinding
    {
        #region Fields
        private readonly ParameterInfo _parameter;
        private readonly Driver.Net.Connection _rethinkDbConnection;
        private readonly string _rethinkDbTableName;
        private readonly Driver.Ast.Table _rethinkDbTable;
        private readonly ILogger _logger;
        private readonly Task<ITriggerData> _emptyBindingDataTask = Task.FromResult<ITriggerData>(new TriggerData(null, new Dictionary<string, object>()));
        #endregion

        #region Properties
        public Type TriggerValueType => typeof(DocumentChange);

        public IReadOnlyDictionary<string, Type> BindingDataContract { get; } = new Dictionary<string, Type>();
        #endregion

        #region Constructor
        public RethinkDbTriggerBinding(ParameterInfo parameter, Driver.Net.Connection rethinkDbConnection, string rethinkDbDatabaseName, string rethinkDbTableName, ILogger logger)
        {
            _parameter = parameter;
            _rethinkDbConnection = rethinkDbConnection;
            _rethinkDbTableName = rethinkDbTableName;
            _rethinkDbTable = Driver.RethinkDB.R.Db(rethinkDbDatabaseName).Table(rethinkDbTableName);
            _logger = logger;
        }
        #endregion

        #region Methods
        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            // ValueProvider is via binding rules. 
            return _emptyBindingDataTask;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return Task.FromResult<IListener>(new RethinkDbTriggerListener(context.Executor, _rethinkDbConnection, _rethinkDbTable, _logger));

        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new RethinkDbTriggerParameterDescriptor
            {
                Name = _parameter.Name,
                Type = RethinkDbTriggerParameterDescriptor.TRIGGER_NAME,
                TableName = _rethinkDbTableName
            };
        }
        #endregion
    }
}
