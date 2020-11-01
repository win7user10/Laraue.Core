using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnDelete;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnInsert;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public static class EntityTypeBuilderExtensions
    {
        private static EntityTypeBuilder<T> AddTriggerAnnotation<T>(
            this EntityTypeBuilder<T> entityTypeBuilder,
            Trigger<T> configuredTrigger) where T : class
        {
            var sqlProvider = Initializer.GetSqlProvider(entityTypeBuilder.Metadata.Model);
            entityTypeBuilder.Metadata.Model.FindEntityType(typeof(T).FullName).AddAnnotation(configuredTrigger.Name, configuredTrigger.BuildSql(sqlProvider));
            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<T> BeforeUpdate<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnUpdateTrigger<T>> configuration) where T : class
            => entityTypeBuilder.AddOnUpdateTrigger(configuration, TriggerTime.Before);

        public static EntityTypeBuilder<T> AfterUpdate<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnUpdateTrigger<T>> configuration) where T : class
            => entityTypeBuilder.AddOnUpdateTrigger(configuration, TriggerTime.After);

        public static EntityTypeBuilder<T> BeforeDelete<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnDeleteTrigger<T>> configuration) where T : class
            => entityTypeBuilder.AddOnDeleteTrigger(configuration, TriggerTime.Before);

        public static EntityTypeBuilder<T> AfterDelete<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnDeleteTrigger<T>> configuration) where T : class
            => entityTypeBuilder.AddOnDeleteTrigger(configuration, TriggerTime.After);

        public static EntityTypeBuilder<T> BeforeInsert<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnInsertTrigger<T>> configuration) where T : class
            => entityTypeBuilder.AddOnInsertTrigger(configuration, TriggerTime.Before);

        public static EntityTypeBuilder<T> AfterInsert<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnInsertTrigger<T>> configuration) where T : class
            => entityTypeBuilder.AddOnInsertTrigger(configuration, TriggerTime.After);

        private static EntityTypeBuilder<T> AddOnUpdateTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnUpdateTrigger<T>> configuration,
            TriggerTime triggerTime) where T : class
        {
            var trigger = new OnUpdateTrigger<T>(triggerTime);
            configuration.Invoke(trigger);
            return entityTypeBuilder.AddTriggerAnnotation(trigger);
        }

        private static EntityTypeBuilder<T> AddOnDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnDeleteTrigger<T>> configuration,
            TriggerTime triggerTime) where T : class
        {
            var trigger = new OnDeleteTrigger<T>(triggerTime);
            configuration.Invoke(trigger);
            return entityTypeBuilder.AddTriggerAnnotation(trigger);
        }

        private static EntityTypeBuilder<T> AddOnInsertTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<OnInsertTrigger<T>> configuration,
            TriggerTime triggerTime) where T : class
        {
            var trigger = new OnInsertTrigger<T>(triggerTime);
            configuration.Invoke(trigger);
            return entityTypeBuilder.AddTriggerAnnotation(trigger);
        }
    }
}