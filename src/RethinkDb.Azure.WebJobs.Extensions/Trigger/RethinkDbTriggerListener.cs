using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Extensions.Logging;
using RethinkDb.Azure.WebJobs.Extensions.Model;

namespace RethinkDb.Azure.WebJobs.Extensions.Trigger
{
    internal class RethinkDbTriggerListener : IListener
    {
        #region Fields
        private const int LISTENER_NOT_REGISTERED = 0;
        private const int LISTENER_REGISTERING = 1;
        private const int LISTENER_REGISTERED = 2;

        private readonly ITriggeredFunctionExecutor _executor;
        private readonly Driver.Net.Connection _rethinkDbConnection;
        private readonly Driver.Ast.Table _rethinkDbTable;
        private readonly ILogger _logger;

        private int _listenerStatus = LISTENER_NOT_REGISTERED;
        private Task _listenerTask;
        private CancellationTokenSource _listenerStoppingTokenSource;

        private bool _disposed = false;
        #endregion

        #region Constructor
        public RethinkDbTriggerListener(ITriggeredFunctionExecutor executor, Driver.Net.Connection rethinkDbConnection, Driver.Ast.Table rethinkDbTable, ILogger logger)
        {
            _executor = executor;
            _rethinkDbConnection = rethinkDbConnection;
            _rethinkDbTable = rethinkDbTable;
            _logger = logger;
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
        {
            if (!_disposed)
            {
                _rethinkDbConnection.Dispose();

                GC.SuppressFinalize(this);

                _disposed = true;
            }
        }

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
            Driver.Net.Cursor<DocumentChange> changefeed = await _rethinkDbTable.Changes().RunCursorAsync<DocumentChange>(_rethinkDbConnection, listenerStoppingToken);

            while (!listenerStoppingToken.IsCancellationRequested && (await changefeed.MoveNextAsync(listenerStoppingToken)))
            {
                await _executor.TryExecuteAsync(new TriggeredFunctionData() { TriggerValue = changefeed.Current }, CancellationToken.None);
            }

            changefeed.Close();
        }
        #endregion
    }
}
