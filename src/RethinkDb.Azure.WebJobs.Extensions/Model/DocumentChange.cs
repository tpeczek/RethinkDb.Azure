using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace RethinkDb.Azure.WebJobs.Extensions.Model
{
    /// <summary>
    /// Represents a document change in RethinkDB table.
    /// </summary>
    public class DocumentChange
    {
        #region Fields
        [JsonProperty("old_val")]
        private JObject _oldValue;

        [JsonProperty("new_val")]
        private JObject _newValue;
        #endregion

        #region Properties
        /// <summary>
        /// If <see cref="RethinkDbTriggerAttribute.IncludeTypes"/> is set to true, this property will indicate the type of change.
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentChangeType? Type { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the old value. When a document is inserted, old value will be null.
        /// </summary>
        /// <returns>The old value.</returns>
        public JObject GetOldValue()
        {
            return _oldValue;
        }

        /// <summary>
        /// Gets the old value as specified .NET type. When a document is inserted, old value will be null.
        /// </summary>
        /// <typeparam name="T">Specified .NET type.</typeparam>
        /// <returns>The old value as specified .NET type.</returns>
        public T GetOldValue<T>()
        {
            return (_oldValue == null) ? default : _oldValue.ToObject<T>();
        }

        /// <summary>
        /// Gets the new value. When a document is deleted, new value will be null.
        /// </summary>
        /// <returns>The new value.</returns>
        public JObject GetNewValue()
        {
            return _newValue;
        }

        /// <summary>
        /// Gets the new value as specified .NET type. When a document is deleted, new value will be null.
        /// </summary>
        /// <typeparam name="T">Specified .NET type.</typeparam>
        /// <returns>The new value as specified .NET type.</returns>
        public T GetNewValue<T>()
        {
            return (_newValue == null) ? default : _newValue.ToObject<T>();
        }
        #endregion
    }
}
