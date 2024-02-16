using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbScaleMonitor : IScaleMonitor<RethinkDbTriggerMetrics>
    {
        #region Fields
        private readonly string _functionId;
        private readonly RethinkDbMetricsProvider _rethinkDbMetricsProvider;
        #endregion

        #region Properties
        public ScaleMonitorDescriptor Descriptor { get; }
        #endregion

        #region Constructor
        public RethinkDbScaleMonitor(string functionId, RethinkDbMetricsProvider rethinkDbMetricsProvider)
        {
            _functionId = functionId;
            _rethinkDbMetricsProvider = rethinkDbMetricsProvider;

            Descriptor = new ScaleMonitorDescriptor($"{_functionId}-{RethinkDbTriggerParameterDescriptor.TRIGGER_NAME}".ToLower(), _functionId);
        }
        #endregion

        #region Methods
        public Task<RethinkDbTriggerMetrics> GetMetricsAsync()
        {
            return Task.FromResult(_rethinkDbMetricsProvider.GetMetrics());
        }

        public ScaleStatus GetScaleStatus(ScaleStatusContext<RethinkDbTriggerMetrics> context)
        {
            throw new NotImplementedException();
        }

        public ScaleStatus GetScaleStatus(ScaleStatusContext context)
        {
            throw new NotImplementedException();
        }

        Task<ScaleMetrics> IScaleMonitor.GetMetricsAsync()
        {
            return Task.FromResult((ScaleMetrics)_rethinkDbMetricsProvider.GetMetrics());
        }
        #endregion
    }
}
