using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;

namespace RethinkDb.Azure.WebJobs.Extensions.Bindings
{
    internal class RethinkDbValueBinder<T> : IValueBinder where T : class
    {
        #region Fields
        private const string RETHINKDB_ID_PROPERTY = "id";
        private static readonly Type T_TYPE = typeof(T);
        private static readonly bool T_IS_JOBJECT = T_TYPE == typeof(JObject);
        private static readonly bool T_IS_STRING = T_TYPE == typeof(String);        

        private readonly RethinkDbAttribute _attribute;
        private readonly Task<IConnection> _rethinkDbConnectionTask;
        private readonly Table _rethinkDbTable;

        private IConnection _rethinkDbConnection;
        private JObject _originalValue;
        #endregion

        #region Properties
        public Type Type => T_TYPE;
        #endregion

        #region Constructor
        public RethinkDbValueBinder(RethinkDbAttribute attribute, Task<IConnection> rethinkDbConnectionTask)
        {
            _attribute = attribute;
            _rethinkDbConnectionTask = rethinkDbConnectionTask;
            _rethinkDbTable = Driver.RethinkDB.R.Db(attribute.DatabaseName).Table(attribute.TableName);
        }
        #endregion

        #region Methods
        public async Task<object> GetValueAsync()
        {
            _rethinkDbConnection = (_rethinkDbConnectionTask.Status == TaskStatus.RanToCompletion) ? _rethinkDbConnectionTask.Result : (await _rethinkDbConnectionTask);

            _originalValue = await _rethinkDbTable.Get(_attribute.Id).RunResultAsync<JObject>(_rethinkDbConnection);

            if(_originalValue == null)
            {
                return _originalValue;
            }
            
            return T_IS_STRING ? (_originalValue.ToString(Formatting.None) as T) : _originalValue.ToObject<T>();
        }

        public Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            if ((value == null) || (_originalValue == null) || T_IS_STRING)
            {
                return Task.CompletedTask;
            }

            JObject currentValue = T_IS_JOBJECT ? (JObject)value : JObject.FromObject(value);
            if (JToken.DeepEquals(_originalValue, currentValue))
            {
                return Task.CompletedTask;
            }

            if (!TryGetId(_originalValue, out string originalId))
            {
                throw new InvalidOperationException($"Cannot update a document which originally didn't had '{RETHINKDB_ID_PROPERTY}' property.");
            }

            if (TryGetId(currentValue, out string currentId))
            {
                if ((currentId != null) && !String.Equals(originalId, currentId, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException($"Cannot update the '{RETHINKDB_ID_PROPERTY}' property.");
                }

                currentValue.Remove(RETHINKDB_ID_PROPERTY);
            }

            
            return _rethinkDbTable.Get(originalId).Update(currentValue).RunAsync(_rethinkDbConnection);
        }

        public string ToInvokeString()
        {
            return String.Empty;
        }

        private static bool TryGetId(JObject value, out string id)
        {
            id = null;

            if (value.TryGetValue(RETHINKDB_ID_PROPERTY, StringComparison.Ordinal, out JToken idToken))
            {
                id = idToken.ToString();
                return true;
            }

            return false;
        }
        #endregion
    }
}
