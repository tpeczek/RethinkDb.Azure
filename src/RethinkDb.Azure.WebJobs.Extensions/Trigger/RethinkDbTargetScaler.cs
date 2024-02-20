using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTargetScaler : ITargetScaler
    {
        #region Fields
        private readonly string _functionId;
        #endregion

        #region Properties
        public TargetScalerDescriptor TargetScalerDescriptor { get; }
        #endregion

        #region Constructor
        public RethinkDbTargetScaler(string functionId)
        {
            _functionId = functionId;

            TargetScalerDescriptor = new TargetScalerDescriptor(_functionId);
        }
        #endregion

        #region Methods
        public Task<TargetScalerResult> GetScaleResultAsync(TargetScalerContext context)
        {
            return Task.FromResult(new TargetScalerResult
            {
                TargetWorkerCount = 1
            });
        }
        #endregion
    }
}
