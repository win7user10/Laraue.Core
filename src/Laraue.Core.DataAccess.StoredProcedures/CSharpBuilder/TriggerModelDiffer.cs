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
            var triggerOperations = new List<MigrationOperation>();

            foreach (var entityType in source?.GetEntityTypes() ?? Enumerable.Empty<IEntityType>())
            {
                var oldTriggerAnnotation = entityType?.FindAnnotation(Constants.TriggerAnnotationName)?.Value as string;
                var newTriggerAnnotation = target?.FindEntityType(entityType.Name)?.FindAnnotation(Constants.TriggerAnnotationName)?.Value as string;
                if (oldTriggerAnnotation != newTriggerAnnotation)
                {
                    triggerOperations.Add(new CreateTriggerOperation(
                        "On_After_Transaction_Inserted",
                        TriggerType.Delete,
                        TriggerTime.AfterTransaction,
                        "NEW.is_verified = true",
                        "update users set {0} = {1}",
                        new string[] { "users.balance" },
                        new object[] { "user.balance + NEW.balance" }
                    ));

                    triggerOperations.Add(new DeleteTriggerOperation(
                        "On_After_Transaction_Inserted"));
                }
            }

            return triggerOperations.Concat(base.GetDifferences(source, target)).ToList();
        }

        public new bool HasDifferences(IModel source, IModel target)
        {
            return GetDifferences(source, target).Count > 0 || base.HasDifferences(source, target);
        }
    }
}
