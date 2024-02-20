using System.Linq;
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
            return GetScaleStatus(context.WorkerCount, context.Metrics?.ToArray());
        }

        public ScaleStatus GetScaleStatus(ScaleStatusContext context)
        {
            return GetScaleStatus(context.WorkerCount, context.Metrics?.Cast<RethinkDbTriggerMetrics>().ToArray());
        }

        Task<ScaleMetrics> IScaleMonitor.GetMetricsAsync()
        {
            return Task.FromResult((ScaleMetrics)_rethinkDbMetricsProvider.GetMetrics());
        }

        private ScaleStatus GetScaleStatus(int workerCount, RethinkDbTriggerMetrics[] metrics)
        {
            ScaleStatus status = new ScaleStatus
            {
                Vote = ScaleVote.None
            };

            // RethinkDB change feed is not meant to be processed in paraller.
            if (workerCount > 1)
            {
                status.Vote = ScaleVote.ScaleIn;

                return status;
            }

            return status;
        }
        #endregion
    }
}
