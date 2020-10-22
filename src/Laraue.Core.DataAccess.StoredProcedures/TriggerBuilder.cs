using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures
{
    public class TriggerBuilder<T> where T : class
    {
        public TriggerBuilder<T> When(Expression<Func<T, bool>> condition)
        {
            return this;
        }

        public UpdateBuilder<TEntity> Update<TEntity>(DbSet<TEntity> dbSet) where TEntity : class
        {
            return new UpdateBuilder<TEntity>();
        }
    }
}
