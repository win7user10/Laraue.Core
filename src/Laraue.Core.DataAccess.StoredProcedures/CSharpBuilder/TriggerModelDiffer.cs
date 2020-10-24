using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Update;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public class TriggerModelDiffer : MigrationsModelDiffer, IMigrationsModelDiffer
    {
        public TriggerModelDiffer(
            IRelationalTypeMappingSource typeMappingSource,
            IMigrationsAnnotationProvider migrationsAnnotations,
            IChangeDetector changeDetector,
            IUpdateAdapterFactory updateAdapterFactory,
            CommandBatchPreparerDependencies commandBatchPreparerDependencies)
                : base(typeMappingSource, migrationsAnnotations, changeDetector, updateAdapterFactory, commandBatchPreparerDependencies)
        {
            
        }

        public new IReadOnlyList<MigrationOperation> GetDifferences(IModel source, IModel target)
        {
            var operations = new List<MigrationOperation>();
            operations.Add(new SqlOperation { Sql = "smth" });

            return operations.Concat(base.GetDifferences(source, target)).ToList();
        }

        public bool HasDifferences(IModel source, IModel target)
        {
            return true;
        }
    }
}
