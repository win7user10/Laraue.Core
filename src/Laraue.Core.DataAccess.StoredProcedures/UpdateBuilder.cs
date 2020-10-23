using System;
using System.Linq;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures
{
    public class UpdateBuilder<TTriggerEntity, TUpdateEntity>
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        private Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> _updateCondition;
        private readonly TriggerBuilder<TTriggerEntity> _triggerBuilder;

        internal UpdateBuilder(TriggerBuilder<TTriggerEntity> triggerBuilder, Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> condition)
        {
            _triggerBuilder = triggerBuilder ?? throw new ArgumentNullException(nameof(triggerBuilder));
            _updateCondition = condition;
        }

        public TriggerBuilder<TTriggerEntity> Set(Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> updatingExpression)
        {
            return _triggerBuilder;
        }
    }
}
