using Laraue.Core.Extensions.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Update;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerBuilder<TTriggerEntity> where TTriggerEntity : class
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        private Expression<Func<TTriggerEntity, bool>> _triggerCondition;

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

        public UpdateTriggerBuilder<TTriggerEntity, TUpdateEntity> Update<TUpdateEntity>(Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> condition)
            where TUpdateEntity : class
        {
            return new UpdateTriggerBuilder<TTriggerEntity, TUpdateEntity>(this, condition);
        }
    }
}
