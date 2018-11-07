using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using RethinkDb.Azure.WebJobs.Extensions.Config;

namespace RethinkDb.Azure.WebJobs.Extensions.Tests
{
    public class RethinkDbWebJobsStartupTests
    {
        #region Prepare SUT
        private IHost PrepareHost()
        {
            return new HostBuilder()
                .ConfigureWebJobs(builder => builder.UseWebJobsStartup<RethinkDbWebJobsStartup>())
                .Build();
        }
        #endregion

        #region Tests
        [Fact]
        public void RethinkDbWebJobsStartup_RegistersSingleExtensionConfigProvider()
        {
            IHost host = PrepareHost();

            IEnumerable<IExtensionConfigProvider> extensionConfigProviders = host.Services.GetServices<IExtensionConfigProvider>();

            Assert.True(extensionConfigProviders.Count() == 1);
        }

        [Fact]
        public void RethinkDbWebJobsStartup_RegistersRethinkDbExtensionConfigProvider()
        {
            IHost host = PrepareHost();

            IEnumerable<IExtensionConfigProvider> extensionConfigProviders = host.Services.GetServices<IExtensionConfigProvider>();

            Assert.Contains(extensionConfigProviders, extensionConfigProvider => (extensionConfigProvider.GetType() == typeof(RethinkDbExtensionConfigProvider)));
        }
        #endregion
    }
}
