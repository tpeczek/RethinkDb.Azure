using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerValueBinder : IValueProvider
    {
        #region Fields
        private static readonly Type STRING_TYPE = typeof(string);

        private readonly ParameterInfo _parameter;
        private readonly object _value;
        private readonly bool _parameterTypeIsString;
        #endregion

        #region Properties
        public Type Type => _parameter.ParameterType;
        #endregion

        #region Constructor
        public RethinkDbTriggerValueBinder(ParameterInfo parameter, object value)
        {
            _parameter = parameter;
            _value = value;
            _parameterTypeIsString = parameter.ParameterType == STRING_TYPE;
        }
        #endregion

        #region Methods
        public Task<object> GetValueAsync()
        {
            if (_parameterTypeIsString)
            {
                return Task.FromResult((object)JsonConvert.SerializeObject(_value));
            }

            return Task.FromResult(_value);
        }

        public string ToInvokeString() => String.Empty;
        #endregion
    }
}
