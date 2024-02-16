using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Scale;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerListener : IListener, IScaleMonitorProvider, ITargetScalerProvider
    {
        #region Fields
        private const int LISTENER_NOT_REGISTERED = 0;
        private const int LISTENER_REGISTERING = 1;
        private const int LISTENER_REGISTERED = 2;

        private readonly string _functionId;
        private readonly ITriggeredFunctionExecutor _executor;
        private readonly Task<IConnection> _rethinkDbConnectionTask;
        private readonly Table _rethinkDbTable;
        private readonly bool _includeTypes;

        private readonly RethinkDbMetricsProvider _rethinkDbMetricsProvider;
        private readonly IScaleMonitor<RethinkDbTriggerMetrics> _rethinkDbScaleMonitor;
        private readonly ITargetScaler _rethinkDbTargetScaler;

        private int _listenerStatus = LISTENER_NOT_REGISTERED;
        private Task _listenerTask;
        private CancellationTokenSource _listenerStoppingTokenSource;
        #endregion

        #region Constructor
        public RethinkDbTriggerListener(string functionId, ITriggeredFunctionExecutor executor, Task<IConnection> rethinkDbConnectionTask, Table rethinkDbTable, bool includeTypes)
        {
            _functionId = functionId;
            _executor = executor;
            _rethinkDbConnectionTask = rethinkDbConnectionTask;
            _rethinkDbTable = rethinkDbTable;
            _includeTypes = includeTypes;

            _rethinkDbMetricsProvider = new RethinkDbMetricsProvider();
            _rethinkDbScaleMonitor = new RethinkDbScaleMonitor(_functionId, _rethinkDbMetricsProvider);
            _rethinkDbTargetScaler = new RethinkDbTargetScaler();
        }
        #endregion

        #region Methods
        public void Cancel()
        {
            StopAsync(CancellationToken.None).Wait();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ValidateListenerStatus();

            try
            {
                _listenerStoppingTokenSource = new CancellationTokenSource();
                _listenerTask = ListenAsync(_listenerStoppingTokenSource.Token);

                Interlocked.CompareExchange(ref _listenerStatus, LISTENER_REGISTERED, LISTENER_REGISTERING);
            }
            catch (Exception)
            {
                _listenerStatus = LISTENER_NOT_REGISTERED;

                throw;
            }

            return _listenerTask.IsCompleted ? _listenerTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_listenerTask == null)
            {
                return;
            }

            try
            {
                _listenerStoppingTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_listenerTask, Task.Delay(Timeout.Infinite, cancellationToken));

                _listenerStatus = LISTENER_NOT_REGISTERED;
            }

        }

        public void Dispose()
        { }

        private void ValidateListenerStatus()
        {
            int previousListenerStatus = Interlocked.CompareExchange(ref _listenerStatus, LISTENER_REGISTERING, LISTENER_NOT_REGISTERED);
            if (previousListenerStatus == LISTENER_REGISTERING)
            {
                throw new InvalidOperationException($"The {nameof(RethinkDbTriggerListener)} is already starting.");
            }
            else if (previousListenerStatus == LISTENER_REGISTERED)
            {
                throw new InvalidOperationException("The {nameof(RethinkDbTriggerListener)}  has already started.");
            }
        }

        private async Task ListenAsync(CancellationToken listenerStoppingToken)
        {
            IConnection rethinkDbConnection = (_rethinkDbConnectionTask.Status == TaskStatus.RanToCompletion) ? _rethinkDbConnectionTask.Result : (await _rethinkDbConnectionTask);

            Cursor<DocumentChange> changefeed = await _rethinkDbTable.Changes()
                .OptArg("include_types", _includeTypes)
                .RunCursorAsync<DocumentChange>(rethinkDbConnection, listenerStoppingToken);

            while (!listenerStoppingToken.IsCancellationRequested && (await changefeed.MoveNextAsync(listenerStoppingToken)))
            {
                _rethinkDbMetricsProvider.CurrentBufferedItemsCount = changefeed.BufferedItems.Count;
                await _executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = changefeed.Current }, CancellationToken.None);
            }

            changefeed.Close();
        }

        public IScaleMonitor GetMonitor()
        {
            return _rethinkDbScaleMonitor;
        }

        public ITargetScaler GetTargetScaler()
        {
            return _rethinkDbTargetScaler;
        }
        #endregion
    }
}
