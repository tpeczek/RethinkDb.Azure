using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTargetScaler : ITargetScaler
    {
        public TargetScalerDescriptor TargetScalerDescriptor => throw new NotImplementedException();

        public Task<TargetScalerResult> GetScaleResultAsync(TargetScalerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
