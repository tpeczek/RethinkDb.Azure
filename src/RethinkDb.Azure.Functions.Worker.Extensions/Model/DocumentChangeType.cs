using System.Text.Json.Serialization;

namespace RethinkDb.Azure.Functions.Worker.Extensions.Model
{
    /// <summary>
    /// The type of <see cref="DocumentChange"/>.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DocumentChangeType
    {
        /// <summary>
        /// Document was added.
        /// </summary>
        Add,
        /// <summary>
        /// Document was removed.
        /// </summary>
        Remove,
        /// <summary>
        /// Document was changed.
        /// </summary>
        Change,
        /// <summary>
        /// Initial document.
        /// </summary>
        Initial,
        /// <summary>
        /// If an initial result for a document has been sent and a change is made to that document that would move it to the unsent part of the result set 
        /// (e.g., a changefeed monitors the top 100 posters, the first 50 have been sent, and poster 48 has become poster 52),
        /// an 'uninitial' notification will be sent, with an old_val field but no new_val field.
        /// </summary>
        Uninitial,
        /// <summary>
        /// A state document.
        /// </summary>
        State
    }
}
