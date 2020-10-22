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

        private static StoredProcedureBuilder<T> AddTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, TriggerType triggerType, TriggerTime triggerTime) where T : class
        {
            return new StoredProcedureBuilder<T>();
        }

        public static StoredProcedureBuilder<T> AddBeforeDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Delete, TriggerTime.BeforeTransaction);

        public static StoredProcedureBuilder<T> AddAfterDeleteTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Delete, TriggerTime.AfterTransaction);

        public static StoredProcedureBuilder<T> AddBeforeUpdateTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.BeforeTransaction);

        public static StoredProcedureBuilder<T> AddAfterUpdateTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.AfterTransaction);

        public static StoredProcedureBuilder<T> AddBeforeInsertTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.BeforeTransaction);

        public static StoredProcedureBuilder<T> AddAfterInsertTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class =>
            entityTypeBuilder.AddTrigger(TriggerType.Update, TriggerTime.AfterTransaction);

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
