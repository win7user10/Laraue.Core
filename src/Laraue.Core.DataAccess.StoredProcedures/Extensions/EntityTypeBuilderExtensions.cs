using Laraue.Core.DataAccess.StoredProcedures.Common;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers;
using Laraue.Core.DataAccess.StoredProcedures.Extensions;
using Laraue.Core.DataAccess.StoredProcedures.Migrations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.CSharpBuilder
{
    public static class EntityTypeBuilderExtensions
    {
        private static EntityTypeBuilder<T> AddTrigger<T>(
            this EntityTypeBuilder<T> entityTypeBuilder,
            TriggerType triggerType,
            TriggerTime triggerTime,
            Action<Trigger<T>> configuration) where T : class
        {
            var trigger = new Trigger<T>(triggerType, triggerTime);
            configuration.Invoke(trigger);

            var sqlProvider = Initializer.GetSqlProvider(entityTypeBuilder.Metadata.Model);
            entityTypeBuilder.Metadata.Model.FindEntityType(typeof(T).FullName).AddAnnotation(trigger.Name, trigger.BuildSql(sqlProvider));

            return entityTypeBuilder;

        }

        public static EntityTypeBuilder<T> AddBeforeDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, Action<Trigger<T>> configuration) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Delete, TriggerTime.Before, configuration);

    }
}