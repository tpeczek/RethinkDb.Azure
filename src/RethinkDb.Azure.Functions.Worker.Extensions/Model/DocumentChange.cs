using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RethinkDb.Azure.Functions.Worker.Extensions.Model
{
    /// <summary>
    /// Represents a document change in RethinkDB table.
    /// </summary>
    public class DocumentChange
    {
        #region Properties
        /// <summary>
        /// Gets or sets the old value. When a document is inserted, old value will be null.
        /// </summary>
        [JsonPropertyName("old_val")]
        public dynamic OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value. When a document is deleted, new value will be null.
        /// </summary>
        [JsonPropertyName("new_val")]
        public dynamic NewValue { get; set; }

        /// <summary>
        /// If <see cref="RethinkDbTriggerAttribute.IncludeTypes"/> is set to true, this property will indicate the type of change.
        /// </summary>
        [JsonPropertyName("type")]
        public DocumentChangeType? Type { get; set; }
        #endregion
    }

    /// <summary>
    /// Represents a document change in RethinkDB table.
    /// </summary>
    public class DocumentChange<T>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the old value. When a document is inserted, old value will be null.
        /// </summary>
        [JsonPropertyName("old_val")]
        public T OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value. When a document is deleted, new value will be null.
        /// </summary>
        [JsonPropertyName("new_val")]
        public T NewValue { get; set; }

        /// <summary>
        /// If <see cref="RethinkDbTriggerAttribute.IncludeTypes"/> is set to true, this property will indicate the type of change.
        /// </summary>
        [JsonPropertyName("type")]
        public DocumentChangeType? Type { get; set; }
        #endregion
    }
}
