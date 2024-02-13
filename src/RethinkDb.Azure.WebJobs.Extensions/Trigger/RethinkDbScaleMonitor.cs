using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbScaleMonitor : IScaleMonitor<RethinkDbTriggerMetrics>
    {
        public ScaleMonitorDescriptor Descriptor => throw new NotImplementedException();

        public Task<RethinkDbTriggerMetrics> GetMetricsAsync()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
