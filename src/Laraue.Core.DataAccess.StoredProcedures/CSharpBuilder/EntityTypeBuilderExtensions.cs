using EFSqlTranslator.EFModels;
using EFSqlTranslator.Translation;
using EFSqlTranslator.Translation.DbObjects;
using EFSqlTranslator.Translation.DbObjects.PostgresQlObjects;
using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public static class EntityTypeBuilderExtensions
    {
        private static IModelInfoProvider _modelInfoProvider;
        private static IDbObjectFactory _dbObjectFactory = new PostgresQlObjectFactory();

        public static void SetupProvider(DbContext dbContext)
        {
            _modelInfoProvider = new EFModelInfoProvider(dbContext);
        }

        private static EntityTypeBuilder<T> AddTrigger<T>(
            this EntityTypeBuilder<T> entityTypeBuilder,
            TriggerType triggerType,
            TriggerTime triggerTime,
            Action<TriggerBuilder<T>> configuration) where T : class
        {
            var triggerBuilder = new TriggerBuilder<T>(triggerType, triggerTime);
            configuration.Invoke(triggerBuilder);

            entityTypeBuilder.Metadata.Model.FindEntityType(typeof(T).FullName).AddAnnotation(
                Constants.TriggerAnnotationName, 
                triggerBuilder.BuildSql(new PostgreSqlVisitor(entityTypeBuilder.Metadata.Model)));

            return entityTypeBuilder;

        }

        public static EntityTypeBuilder<T> AddBeforeDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Delete, TriggerTime.BeforeTransaction, configuration);
    }
}