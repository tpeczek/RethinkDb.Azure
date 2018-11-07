using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host.Protocols;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerParameterDescriptor : TriggerParameterDescriptor
    {
        #region Fields
        internal const string TRIGGER_NAME = "RethinkDBTrigger";
        private const string TRIGGER_DESCRIPTION = "New changes on table {0} at {1}";
        #endregion

        #region Properties
        internal string TableName { get; set; }
        #endregion

        #region Methods
        public override string GetTriggerReason(IDictionary<string, string> arguments)
        {
            return String.Format(TRIGGER_DESCRIPTION, TableName, DateTime.UtcNow.ToString("o"));
        }
        #endregion
    }
}
