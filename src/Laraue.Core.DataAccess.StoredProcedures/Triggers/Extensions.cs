using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.Triggers
{
    public static class Extensions
    {
        private static EntityTypeBuilder<T> AddTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, TriggerType triggerType, TriggerTime triggerTime, Action<TriggerBuilder<T>> configuration) where T : class
        {
            entityTypeBuilder.Metadata.Model.AddAnnotation(Constants.TriggerAnnotationName, "testValue");
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
