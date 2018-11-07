using System;
using Microsoft.Azure.WebJobs;
using Xunit;

namespace RethinkDb.Azure.WebJobs.Extensions.Tests.Trigger
{
    public class RethinkDbTriggerAttributeTests
    {
        #region Fields
        private const string SOME_DATABASE_NAME = "RethinkDBDatabase";
        private const string SOME_TABLE_NAME = "RethinkDbTableName";
        #endregion

        #region Tests
        [Fact]
        public void Constructor_InvalidDatabaseNameValidTableName_ThrowsArgumentNullExceptionForTableName()
        {
            ArgumentNullException databaseNameNullException = Assert.Throws<ArgumentNullException>(() => new RethinkDbTriggerAttribute(null, SOME_TABLE_NAME));
            Assert.Equal("databaseName", databaseNameNullException.ParamName);
        }

        [Fact]
        public void Constructor_ValidDatabaseNameInvalidTableName_ThrowsArgumentNullExceptionForTableName()
        {
            ArgumentNullException tableNameNullException = Assert.Throws<ArgumentNullException>(() => new RethinkDbTriggerAttribute(SOME_DATABASE_NAME, null));
            Assert.Equal("tableName", tableNameNullException.ParamName);
        }

        [Fact]
        public void Constructor_ValidDatabaseNameValidTableName_SetsDatabaseName()
        {
            RethinkDbTriggerAttribute attribute = new RethinkDbTriggerAttribute(SOME_DATABASE_NAME, SOME_TABLE_NAME);

            Assert.Equal(SOME_DATABASE_NAME, attribute.DatabaseName);
        }

        [Fact]
        public void Constructor_ValidDatabaseNameValidTableName_SetsTableName()
        {
            RethinkDbTriggerAttribute attribute = new RethinkDbTriggerAttribute(SOME_DATABASE_NAME, SOME_TABLE_NAME);

            Assert.Equal(SOME_TABLE_NAME, attribute.TableName);
        }
        #endregion
    }
}
