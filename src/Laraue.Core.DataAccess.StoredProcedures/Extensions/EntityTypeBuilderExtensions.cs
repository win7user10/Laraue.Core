using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
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

        private static EntityTypeBuilder<T> AddOnUpdateTrigger<T>(
                this EntityTypeBuilder<T> entityTypeBuilder,
                Action<OnUpdateTrigger<T>> configuration,
                TriggerTime triggerTime)
            where T : class
        {
            var trigger = new OnUpdateTrigger<T>(triggerTime);
            configuration.Invoke(trigger);
            return entityTypeBuilder.AddTriggerAnnotation(trigger);
        }
    }
}