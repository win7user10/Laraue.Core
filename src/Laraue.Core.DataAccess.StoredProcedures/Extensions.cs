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

        public static StoredProcedureBuilder<T> AddTrigger<T>(this EntityTypeBuilder<T> entityTypeBuilder, TriggerType triggerType, TriggerTime triggerTime = TriggerTime.AfterTransaction) where T : class
        {
            return new StoredProcedureBuilder<T>();
        }
    }

    public enum TriggerType
    {
        Insert,
        Update,
        Delete,
    }

    public enum TriggerTime
    {
        BeforeTransaction,
        AfterTransaction,
    }
}
