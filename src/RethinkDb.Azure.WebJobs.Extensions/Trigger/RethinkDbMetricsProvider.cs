namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbMetricsProvider
    {
        public int CurrentBufferedItemsCount { get; set; }

        public RethinkDbTriggerMetrics GetMetrics()
        {
            return new RethinkDbTriggerMetrics { BufferedItemsCount = CurrentBufferedItemsCount };
        }
    }
}
