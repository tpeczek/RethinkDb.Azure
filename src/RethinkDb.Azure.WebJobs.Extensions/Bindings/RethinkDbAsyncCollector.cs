using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;

namespace RethinkDb.Azure.WebJobs.Extensions.Bindings
{
    internal class RethinkDbAsyncCollector<T> : IAsyncCollector<T>
    {
        #region Fields
        private const string NOT_EXISTS_AND_NO_CREATE_INVALID_OPERATION_MESSAGE_FORMAT = "The table '{0}' (in database '{1}') does not exist. To automatically create the table, set '{2}' to 'true'.";

        private readonly RethinkDbAttribute _attribute;
        private readonly Task<IConnection> _rethinkDbConnectionTask;
        private readonly Table _rethinkDbTable;

        private IConnection _rethinkDbConnection;
        private bool _alreadyExistsOrCreated = false;
        #endregion

        #region Constructor
        public RethinkDbAsyncCollector(RethinkDbAttribute attribute, Task<IConnection> rethinkDbConnectionTask)
        {
            _attribute = attribute;
            _rethinkDbConnectionTask = rethinkDbConnectionTask;
            _rethinkDbTable = Driver.RethinkDB.R.Db(attribute.DatabaseName).Table(attribute.TableName);
        }
        #endregion

        #region Methods
        public async Task AddAsync(T item, CancellationToken cancellationToken = default)
        {
            if (_rethinkDbConnection is null)
            {
                _rethinkDbConnection = (_rethinkDbConnectionTask.Status == TaskStatus.RanToCompletion) ? _rethinkDbConnectionTask.Result : (await _rethinkDbConnectionTask);
            }

            await EnsureExistsOrCreateAsync(cancellationToken);

            await UpsertAsync(item, cancellationToken);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private async Task EnsureExistsOrCreateAsync(CancellationToken cancellationToken)
        {
            if (_alreadyExistsOrCreated)
            {
                return;
            }

            if (!(await Driver.RethinkDB.R.DbList().RunResultAsync<string[]>(_rethinkDbConnection, cancellationToken)).Contains(_attribute.DatabaseName))
            {
                if (_attribute.CreateIfNotExists)
                {
                    await Driver.RethinkDB.R.DbCreate(_attribute.DatabaseName).RunAsync(_rethinkDbConnection, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException(String.Format(NOT_EXISTS_AND_NO_CREATE_INVALID_OPERATION_MESSAGE_FORMAT, _attribute.TableName, _attribute.DatabaseName, nameof(RethinkDbAttribute.CreateIfNotExists)));
                }
            }

            if (!(await Driver.RethinkDB.R.Db(_attribute.DatabaseName).TableList().RunResultAsync<string[]>(_rethinkDbConnection, cancellationToken)).Contains(_attribute.TableName))
            {
                if (_attribute.CreateIfNotExists)
                {
                    await Driver.RethinkDB.R.Db(_attribute.DatabaseName).TableCreate(_attribute.TableName).RunAsync(_rethinkDbConnection, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException(String.Format(NOT_EXISTS_AND_NO_CREATE_INVALID_OPERATION_MESSAGE_FORMAT, _attribute.TableName, _attribute.DatabaseName, nameof(RethinkDbAttribute.CreateIfNotExists)));
                }
            }

            _alreadyExistsOrCreated = true;
        }

        private Task UpsertAsync(T item, CancellationToken cancellationToken)
        {
            return _rethinkDbTable.Insert(item)
                .OptArg("conflict", "update")
                .RunAsync(_rethinkDbConnection, cancellationToken);
        }
        #endregion
    }
}
