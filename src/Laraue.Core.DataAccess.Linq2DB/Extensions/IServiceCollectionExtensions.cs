using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Laraue.Core.DataAccess.Linq2DB.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add to container linq2DB provider to enable its functions.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddLinq2Db(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<Linq2DbProvider>();
        }

        /// <summary>
        /// Initializer of Linq2DB provider and <see cref="ILogger"/> for it.
        /// </summary>
        private class Linq2DbProvider
        {
            public Linq2DbProvider(ILogger<Linq2DbProvider> logger)
            {
                LinqToDBForEFTools.Initialize();
                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (msg, category, level) => logger.Log(GetLogLevel(level), msg);
            }
        }

        /// <summary>
        /// Convert logs of Linq2DB to Microsoft.
        /// </summary>
        /// <param name="traceLevel"></param>
        /// <returns></returns>
        private static LogLevel GetLogLevel(TraceLevel traceLevel) => traceLevel switch
        {
            TraceLevel.Verbose => LogLevel.Trace,
            TraceLevel.Error => LogLevel.Error,
            TraceLevel.Warning => LogLevel.Warning,
            TraceLevel.Info => LogLevel.Information,
            _ => LogLevel.Debug,
        };

        /// <summary>
        /// Run Linq2DB initializer.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IServiceProvider UseLinq2Db(this IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<Linq2DbProvider>();
            return serviceProvider;
        }
    }
}
