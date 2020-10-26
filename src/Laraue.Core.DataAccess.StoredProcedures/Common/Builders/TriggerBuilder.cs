using Laraue.Core.Extensions.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerBuilder<TTriggerEntity> : ITrigger
        where TTriggerEntity : class
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        private Expression<Func<TTriggerEntity, bool>> _triggerCondition;

        private Expression _actionExpression;

        private Expression _actionCondition;

        internal TriggerBuilder(TriggerType triggerType, TriggerTime triggerTime)
        {
            TriggerType = triggerType;
            TriggerTime = triggerTime;
        }

        /// <summary>
        /// Add condition, when trigger should works.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TriggerBuilder<TTriggerEntity> When(Expression<Func<TTriggerEntity, bool>> condition)
        {
            if (condition is null) throw new ArgumentNullException(nameof(condition));
            if (_triggerCondition == null)
                _triggerCondition = condition;
            else
                _triggerCondition = _triggerCondition.AndAlso(condition);
            return this;
        }

        public TriggerBuilder<TTriggerEntity> Update<TUpdateEntity>(
            Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> condition,
            Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setExpression)
            where TUpdateEntity : class
        {
            _actionCondition = condition;
            _actionExpression = setExpression;

            return this;
        }

        public TriggerAnnatation Visit(IBuilderVisitor builderVisitor)
        {
            throw new NotImplementedException();
        }

        public Trigger Build()
        {
            return new Trigger
            {
                TriggerConditionExpression = _triggerCondition,
                TriggerTime = TriggerTime,
                TriggerType = TriggerType,
                ActionConditionExpression = _actionCondition,
                ActionExpression = _actionExpression
            };
        }
    }
}
