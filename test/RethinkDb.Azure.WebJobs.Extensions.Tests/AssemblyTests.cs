using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs.Hosting;
using Xunit;

namespace RethinkDb.Azure.WebJobs.Extensions.Tests
{
    public class AssemblyTests
    {
        #region Prepare SUT
        private IEnumerable<Type> PrepareWebJobsStartupTypes()
        {
            return typeof(RethinkDbOptions).Assembly.GetCustomAttributes<WebJobsStartupAttribute>()
                .Select(startupAttribute => startupAttribute.WebJobsStartupType);
        }
        #endregion

        #region Tests
        [Fact]
        public void Assembly_RegistersSingleWebJobsStartup()
        {
            IEnumerable<Type> webJobsStartupTypes = PrepareWebJobsStartupTypes();

            Assert.True(webJobsStartupTypes.Count() == 1);
        }

        [Fact]
        public void Assembly_RegistersRethinkDbWebJobsStartup()
        {
            IEnumerable<Type> webJobsStartupTypes = PrepareWebJobsStartupTypes();

            Assert.Contains(webJobsStartupTypes, webJobsStartupType => webJobsStartupType == typeof(RethinkDbWebJobsStartup));
        }
        #endregion
    }
}
