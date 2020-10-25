using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.SqlMigrationBuilder;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class CSharpMigrationsGenerator : Microsoft.EntityFrameworkCore.Migrations.Design.CSharpMigrationsGenerator
    {
        public CSharpMigrationsGenerator(
            MigrationsCodeGeneratorDependencies dependencies,
            CSharpMigrationsGeneratorDependencies csharpDependencies)
            : base(dependencies, csharpDependencies)
        {
        }

        protected override IEnumerable<string> GetNamespaces(IEnumerable<MigrationOperation> operations)
        {
            var typesToAdd = new Type[]
            {
                typeof(MigrationBuilderExtensions),
                typeof(TriggerTime)
            }
                .Select(x => x.Namespace)
                .ToHashSet();

            return typesToAdd.Concat(base.GetNamespaces(operations));
        }
    }
}