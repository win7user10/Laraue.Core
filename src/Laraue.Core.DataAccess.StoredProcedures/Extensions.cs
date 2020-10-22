using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace Laraue.Core.DataAccess.StoredProcedures
{
    public static class Extensions
    {
        public static void DoUpdate<T>(this IQueryable<T> entityTypeBuilder, Action<T> predicate) where T : class
        {
        }

        private static EntityTypeBuilder<T> AddTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, TriggerType triggerType, TriggerTime triggerTime, Action<TriggerBuilder<T>> configuration) where T : class
        {
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

        enum TriggerType
        {
            Insert,
            Update,
            Delete,
        }

        enum TriggerTime
        {
            BeforeTransaction,
            AfterTransaction,
        }
    }
}
