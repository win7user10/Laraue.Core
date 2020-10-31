using Laraue.Core.DataAccess.StoredProcedures.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laraue.Core.DataAccess.StoredProcedures.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbProvider? ActualProvider { get; private set; }

        private static readonly Dictionary<string, DbProvider> _existsProviders = new Dictionary<string, DbProvider>
        {
            ["NpgsqlOptionsExtension"] = DbProvider.PostgreSql,
        };

        public static DbContextOptionsBuilder<TContext> UseTriggers<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder)
            where TContext : DbContext
        {
            var providers = optionsBuilder.Options
                .Extensions
                .Where(x => x.Info.IsDatabaseProvider)
                .ToArray();

            if (providers.Length == 0) throw new InvalidOperationException("No one DB provider was found!");
            if (providers.Length > 1) throw new InvalidOperationException($"Found {providers.Length} DB providers, try to chose provider explicitly using another overload.");

            var providerName = providers.First().GetType().Name;

            if (!_existsProviders.TryGetValue(providerName, out var dbProvider))
                throw new InvalidOperationException($"Extension {providerName} is not supporting!");

            ActualProvider = dbProvider;
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseTriggers<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, DbProvider dbProvider)
            where TContext : DbContext
        {
            ActualProvider = dbProvider;
            return optionsBuilder;
        }
    }
}
