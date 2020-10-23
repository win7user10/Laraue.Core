using System;
using System.Linq;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Update
{
    public class UpdateTriggerBuilder<TTriggerEntity, TUpdateEntity>
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> UpdateCondition { get; private set; }
        private readonly TriggerBuilder<TTriggerEntity> _triggerBuilder;

        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        public Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> SetExpression { get; private set; }

        internal UpdateTriggerBuilder(TriggerBuilder<TTriggerEntity> triggerBuilder, Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> condition)
        {
            _triggerBuilder = triggerBuilder ?? throw new ArgumentNullException(nameof(triggerBuilder));
            UpdateCondition = condition;
        }

        public TriggerBuilder<TTriggerEntity> Set(Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setExpression)
        {
            SetExpression = setExpression ?? throw new ArgumentNullException(nameof(setExpression));
            return _triggerBuilder;
        }

        public UpdateTrigger Build()
        {
            return new UpdateTrigger(TriggerType, TriggerTime, UpdateCondition, SetExpression);
        }
    }
}