using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RethinkDb.Azure.WebJobs.Extensions.Config;

namespace RethinkDb.Azure.WebJobs.Extensions
{
    /// <summary>
    /// The <see cref="IWebJobsBuilder"/> extension methods for RethinkDB binding extensions.
    /// </summary>
    public static class RethinkDbWebJobsBuilderExtensions
    {
        /// <summary>
        /// Adds the RethinkDb binding extensions to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        /// <param name="configure">An <see cref="Action{RethinkDbOptions}"/> to configure the provided <see cref="RethinkDbOptions"/>.</param>
        public static IWebJobsBuilder AddRethinkDb(this IWebJobsBuilder builder, Action<RethinkDbOptions> configure)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure is null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddRethinkDb();
            builder.Services.Configure(configure);

            return builder;
        }

        /// <summary>
        /// Adds the RethinkDb binding extensions to the provided <see cref="IWebJobsBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder"/> to configure.</param>
        public static IWebJobsBuilder AddRethinkDb(this IWebJobsBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<RethinkDbExtensionConfigProvider>()
                .ConfigureOptions<RethinkDbOptions>((config, path, options) =>
                {
                    config.GetSection(path).Bind(options);
                });

            return builder;
        }
    }
}
