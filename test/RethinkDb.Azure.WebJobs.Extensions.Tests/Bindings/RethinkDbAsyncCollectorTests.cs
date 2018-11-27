using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Bindings;
using Moq;
using Xunit;
using RethinkDb.Azure.WebJobs.Extensions.Tests.Bindings.Infrastructure;

namespace RethinkDb.Azure.WebJobs.Extensions.Tests.Bindings
{
    public class RethinkDbAsyncCollectorTests
    {
        #region Fields
        private const string DATABASE_NAME = "TESTDB";
        private const string TABLE_NAME = "ITEMS";
        private const string DOCUMENT_ID = "084D0625-1765-4d49-9702-CE87B44B2582";
        private const string DOCUMENT_PROPERTY_VALUE = "Value";

        private const string DB_LIST_REQL_RAW = "$reql_reqlraw$[59,[]]";
        private const string TABLE_LIST_REQL_RAW = "$reql_reqlraw$[62,[[14,[\"" + DATABASE_NAME + "\"]]]]";
        private const string DB_CREATE_REQL_RAW = "$reql_reqlraw$[57,[\"" + DATABASE_NAME + "\"]]";
        private const string TABLE_CREATE_REQL_RAW = "$reql_reqlraw$[60,[[14,[\"" + DATABASE_NAME + "\"]],\"" + TABLE_NAME + "\"]]";
        private const string UPSERT_REQL_RAW = "$reql_reqlraw$[56,[[15,[[14,[\"" + DATABASE_NAME + "\"]],\"" + TABLE_NAME + "\"]],{\"id\":\"" + DOCUMENT_ID + "\",\"Property\":\"" + DOCUMENT_PROPERTY_VALUE + "\"}],{\"conflict\":\"update\"}]";

        private static readonly Func<ReqlAst, bool> DB_OR_TABLE_LIST_REQL_MATCH = reql => DB_LIST_REQL_RAW.Equals(ReqlRaw.ToRawString(reql)) || TABLE_LIST_REQL_RAW.Equals(ReqlRaw.ToRawString(reql));
        private static readonly Func<ReqlAst, bool> DB_CREATE_REQL_MATCH = reql => DB_CREATE_REQL_RAW.Equals(ReqlRaw.ToRawString(reql));
        private static readonly Func<ReqlAst, bool> TABLE_CREATE_REQL_MATCH = reql => TABLE_CREATE_REQL_RAW.Equals(ReqlRaw.ToRawString(reql));
        private static readonly Func<ReqlAst, bool> UPSERT_REQL_MATCH = reql => UPSERT_REQL_RAW.Equals(ReqlRaw.ToRawString(reql));

        private const string NOT_EXISTS_AND_NO_CREATE_INVALID_OPERATION_MESSAGE = "The table '" + TABLE_NAME + "' (in database '" + DATABASE_NAME + "') does not exist. To automatically create the table, set 'CreateIfNotExists' to 'true'.";
        #endregion

        #region Prepare SUT
        private Mock<IConnection> PrepareRethinkDbConnectionMock(bool databaseExists, bool tableExists)
        {
            Mock<IConnection> rethinkDbConnectionMock = new Mock<IConnection>();

            rethinkDbConnectionMock
                .Setup(m => m.RunResultAsync<string[]>(It.Is<ReqlAst>(reql => DB_OR_TABLE_LIST_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReqlAst reql, Object globalOpts, CancellationToken cancelToken) =>
                {
                    string reqlRaw = ReqlRaw.ToRawString(reql);

                    if (databaseExists && DB_LIST_REQL_RAW.Equals(reqlRaw))
                    {
                        return new string[] { DATABASE_NAME };
                    }

                    if (tableExists && TABLE_LIST_REQL_RAW.Equals(reqlRaw))
                    {
                        return new string[] { TABLE_NAME };
                    }

                    return new string[0];
                });

            return rethinkDbConnectionMock;
        }

        private RethinkDbAsyncCollector<T> PrepareRethinkDbAsyncCollector<T>(IConnection rethinkDbConnection, bool createIfNotExists = false) where T : class
        {
            return new RethinkDbAsyncCollector<T>(
                new RethinkDbAttribute(DATABASE_NAME, TABLE_NAME) { CreateIfNotExists = createIfNotExists },
                Task.FromResult(rethinkDbConnection)
            );
        }
        #endregion

        #region Tests
        [Fact]
        public async Task AddAsync_DatabaseExistsTableExists_RunsUpsertOnTable()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(true, true);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object);

            await rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE });

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => UPSERT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DatabaseDoesNotExistsTableDoesNotExistsNoCreateIfNotExists_ThrowsInvalidOperationException()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(false, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: false);

            InvalidOperationException invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(() => rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE }));
            Assert.Equal(NOT_EXISTS_AND_NO_CREATE_INVALID_OPERATION_MESSAGE, invalidOperationException.Message);
        }

        [Fact]
        public async Task AddAsync_DatabaseDoesNotExistsTableDoesNotExistsCreateIfNotExists_RunsDbCreate()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(false, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: true);

            await rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE });

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => DB_CREATE_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DatabaseDoesNotExistsTableDoesNotExistsCreateIfNotExists_RunsTableCreate()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(false, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: true);

            await rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE });

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => TABLE_CREATE_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DatabaseDoesNotExistsTableDoesNotExistsCreateIfNotExists_RunsUpsertOnTable()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(false, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: true);

            await rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE });

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => UPSERT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DatabaseExistsTableDoesNotExistsNoCreateIfNotExists_ThrowsInvalidOperationException()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(true, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: false);

            InvalidOperationException invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(() => rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE }));
            Assert.Equal(NOT_EXISTS_AND_NO_CREATE_INVALID_OPERATION_MESSAGE, invalidOperationException.Message);
        }

        [Fact]
        public async Task AddAsync_DatabaseExistsTableDoesNotExistsCreateIfNotExists_RunsTableCreate()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(true, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: true);

            await rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE });

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => TABLE_CREATE_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DatabaseExistsTableDoesNotExistsCreateIfNotExists_RunsUpsertOnTable()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock(true, false);
            RethinkDbAsyncCollector<Poco> rethinkDbAsyncCollector = PrepareRethinkDbAsyncCollector<Poco>(rethinkDbConnectionMock.Object, createIfNotExists: true);

            await rethinkDbAsyncCollector.AddAsync(new Poco { Id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE });

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => UPSERT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
