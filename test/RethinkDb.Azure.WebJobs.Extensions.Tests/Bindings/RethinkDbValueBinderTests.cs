using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;
using RethinkDb.Azure.WebJobs.Extensions.Bindings;
using Moq;
using Xunit;
using RethinkDb.Azure.WebJobs.Extensions.Tests.Bindings.Infrastructure;

namespace RethinkDb.Azure.WebJobs.Extensions.Tests.Bindings
{
    public class RethinkDbValueBinderTests
    {
        #region Fields
        private const string DATABASE_NAME = "TESTDB";
        private const string TABLE_NAME = "ITEMS";
        private const string DOCUMENT_ID = "084D0625-1765-4d49-9702-CE87B44B2582";
        private const string OTHER_DOCUMENT_ID = "084D0625-1765-4d49-9702-CE87B44B2583";
        private const string DOCUMENT_PROPERTY_VALUE = "Value";
        private const string DOCUMENT_PROPERTY_NEW_VALUE = "New value";
        private const string GET_DOCUMENT_REQL_RAW = "$reql_reqlraw$[16,[[15,[[14,[\"" + DATABASE_NAME + "\"]],\"" + TABLE_NAME + "\"]],\"" + DOCUMENT_ID + "\"]]";
        private const string UPDATE_DOCUMENT_REQL_RAW = "$reql_reqlraw$[53,[[16,[[15,[[14,[\"" + DATABASE_NAME + "\"]],\"" + TABLE_NAME + "\"]],\"" + DOCUMENT_ID + "\"]],{\"Property\":\"" + DOCUMENT_PROPERTY_NEW_VALUE + "\"}]]";
        private const string UPDATE_ID_INVALID_OPERATION_EXCEPTION_MESSAGE = "Cannot update the 'id' property.";

        private static readonly Func<ReqlAst, bool> GET_DOCUMENT_REQL_MATCH = reql => GET_DOCUMENT_REQL_RAW.Equals(ReqlRaw.ToRawString(reql));
        private static readonly Func<ReqlAst, bool> UPDATE_DOCUMENT_REQL_MATCH = reql => UPDATE_DOCUMENT_REQL_RAW.Equals(ReqlRaw.ToRawString(reql));
        #endregion

        #region Prepare SUT
        private Mock<IConnection> PrepareRethinkDbConnectionMock()
        {
            Mock<IConnection> rethinkDbConnectionMock = new Mock<IConnection>();

            rethinkDbConnectionMock
                .Setup(m => m.RunResultAsync<JObject>(It.Is<ReqlAst>(reql => GET_DOCUMENT_REQL_MATCH(reql)),It.IsAny<Object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(JObject.FromObject(new { id = DOCUMENT_ID, Property = DOCUMENT_PROPERTY_VALUE }));

            return rethinkDbConnectionMock;
        }

        private RethinkDbValueBinder<T> PrepareRethinkDbValueBinder<T>(IConnection rethinkDbConnection) where T: class
        {
            return new RethinkDbValueBinder<T>(
                new RethinkDbAttribute(DATABASE_NAME, TABLE_NAME) { Id = DOCUMENT_ID },
                Task.FromResult(rethinkDbConnection)
            );
        }
        #endregion

        #region Tests
        [Fact]
        public async Task GetValueAsync_JObject_RunsGetOnTable()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<JObject> rethinkDbValueBinder = PrepareRethinkDbValueBinder<JObject>(rethinkDbConnectionMock.Object);

            JObject value = (await rethinkDbValueBinder.GetValueAsync()) as JObject;

            rethinkDbConnectionMock.Verify(m => m.RunResultAsync<JObject>(It.Is<ReqlAst>(reql => GET_DOCUMENT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetValueAsync_JObject_ReturnsValue()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<JObject> rethinkDbValueBinder = PrepareRethinkDbValueBinder<JObject>(rethinkDbConnectionMock.Object);

            JObject value = (await rethinkDbValueBinder.GetValueAsync()) as JObject;

            Assert.NotNull(value);
        }

        [Fact]
        public async Task GetValueAsync_Poco_RunsGetOnTable()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<Poco> rethinkDbValueBinder = PrepareRethinkDbValueBinder<Poco>(rethinkDbConnectionMock.Object);

            Poco value = (await rethinkDbValueBinder.GetValueAsync()) as Poco;

            rethinkDbConnectionMock.Verify(m => m.RunResultAsync<JObject>(It.Is<ReqlAst>(reql => GET_DOCUMENT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetValueAsync_Poco_ReturnsValue()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<Poco> rethinkDbValueBinder = PrepareRethinkDbValueBinder<Poco>(rethinkDbConnectionMock.Object);

            Poco value = (await rethinkDbValueBinder.GetValueAsync()) as Poco;

            Assert.NotNull(value);
        }

        [Fact]
        public async Task GetValueAsync_String_RunsGetOnTable()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<String> rethinkDbValueBinder = PrepareRethinkDbValueBinder<String>(rethinkDbConnectionMock.Object);

            string value = (await rethinkDbValueBinder.GetValueAsync()) as String;

            rethinkDbConnectionMock.Verify(m => m.RunResultAsync<JObject>(It.Is<ReqlAst>(reql => GET_DOCUMENT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetValueAsync_String_ReturnsValue()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<String> rethinkDbValueBinder = PrepareRethinkDbValueBinder<String>(rethinkDbConnectionMock.Object);

            string value = (await rethinkDbValueBinder.GetValueAsync()) as String;

            Assert.NotNull(value);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_JObject_NoPropertyChange_DoesNotUpdate()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<JObject> rethinkDbValueBinder = PrepareRethinkDbValueBinder<JObject>(rethinkDbConnectionMock.Object);

            JObject value = (await rethinkDbValueBinder.GetValueAsync()) as JObject;
            await rethinkDbValueBinder.SetValueAsync(value, default(CancellationToken));

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.IsAny<ReqlAst>(), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_JObject_PropertyChange_Updates()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<JObject> rethinkDbValueBinder = PrepareRethinkDbValueBinder<JObject>(rethinkDbConnectionMock.Object);

            JObject value = (await rethinkDbValueBinder.GetValueAsync()) as JObject;
            value["Property"] = DOCUMENT_PROPERTY_NEW_VALUE;
            await rethinkDbValueBinder.SetValueAsync(value, default(CancellationToken));

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => UPDATE_DOCUMENT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_JObject_IdChange_ThrowsInvalidOperationException()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<JObject> rethinkDbValueBinder = PrepareRethinkDbValueBinder<JObject>(rethinkDbConnectionMock.Object);

            JObject value = (await rethinkDbValueBinder.GetValueAsync()) as JObject;
            value["id"] = OTHER_DOCUMENT_ID;

            InvalidOperationException invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(() => rethinkDbValueBinder.SetValueAsync(value, default(CancellationToken)));
            Assert.Equal(UPDATE_ID_INVALID_OPERATION_EXCEPTION_MESSAGE, invalidOperationException.Message);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_Poco_NoPropertyChange_DoesNotUpdate()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<Poco> rethinkDbValueBinder = PrepareRethinkDbValueBinder<Poco>(rethinkDbConnectionMock.Object);

            Poco value = (await rethinkDbValueBinder.GetValueAsync()) as Poco;
            await rethinkDbValueBinder.SetValueAsync(value, default(CancellationToken));

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.IsAny<ReqlAst>(), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_Poco_PropertyChange_Updates()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<Poco> rethinkDbValueBinder = PrepareRethinkDbValueBinder<Poco>(rethinkDbConnectionMock.Object);

            Poco value = (await rethinkDbValueBinder.GetValueAsync()) as Poco;
            value.Property = DOCUMENT_PROPERTY_NEW_VALUE;
            await rethinkDbValueBinder.SetValueAsync(value, default(CancellationToken));

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.Is<ReqlAst>(reql => UPDATE_DOCUMENT_REQL_MATCH(reql)), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_Poco_IdChange_ThrowsInvalidOperationException()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<Poco> rethinkDbValueBinder = PrepareRethinkDbValueBinder<Poco>(rethinkDbConnectionMock.Object);

            Poco value = (await rethinkDbValueBinder.GetValueAsync()) as Poco;
            value.Id = OTHER_DOCUMENT_ID;

            InvalidOperationException invalidOperationException = await Assert.ThrowsAsync<InvalidOperationException>(() => rethinkDbValueBinder.SetValueAsync(value, default(CancellationToken)));
            Assert.Equal(UPDATE_ID_INVALID_OPERATION_EXCEPTION_MESSAGE, invalidOperationException.Message);
        }

        [Fact]
        public async Task GetValueAsyncSetValueAsync_String_DoesNotUpdate()
        {
            Mock<IConnection> rethinkDbConnectionMock = PrepareRethinkDbConnectionMock();
            RethinkDbValueBinder<String> rethinkDbValueBinder = PrepareRethinkDbValueBinder<String>(rethinkDbConnectionMock.Object);

            String value = (await rethinkDbValueBinder.GetValueAsync()) as String;
            await rethinkDbValueBinder.SetValueAsync(DOCUMENT_PROPERTY_NEW_VALUE, default(CancellationToken));

            rethinkDbConnectionMock.Verify(m => m.RunAsync<Object>(It.IsAny<ReqlAst>(), It.IsAny<Object>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        #endregion
    }
}
