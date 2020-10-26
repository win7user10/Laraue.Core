using EFSqlTranslator.EFModels;
using EFSqlTranslator.Translation;
using EFSqlTranslator.Translation.DbObjects.PostgresQlObjects;
using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public static class EntityTypeBuilderExtensions
    {
        private static IModelInfoProvider _modelInfoProvider;

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
            var builtTrigger = triggerBuilder.Build();

            var translatedExpression = QueryTranslator.Translate(builtTrigger.ActionExpression, _modelInfoProvider, new PostgresQlObjectFactory());
            entityTypeBuilder.Metadata.Model.FindEntityType(typeof(T).FullName).AddAnnotation(Constants.TriggerAnnotationName, translatedExpression);
            return entityTypeBuilder;

        }

        public static EntityTypeBuilder<T> AddBeforeDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Delete, TriggerTime.BeforeTransaction, configuration);

        public static EntityTypeBuilder<T> AddAfterDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Delete, TriggerTime.AfterTransaction, configuration);

        public static EntityTypeBuilder<T> AddBeforeUpdateTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.BeforeTransaction, configuration);

        public static EntityTypeBuilder<T> AddAfterUpdateTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.AfterTransaction, configuration);

        public static EntityTypeBuilder<T> AddBeforeInsertTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.BeforeTransaction, configuration);

        public static EntityTypeBuilder<T> AddAfterInsertTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<TriggerBuilder<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.AfterTransaction, configuration);
    }
}