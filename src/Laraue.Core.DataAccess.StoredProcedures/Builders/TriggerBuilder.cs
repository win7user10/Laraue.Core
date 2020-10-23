using Laraue.Core.Extensions.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures
{
    public class TriggerBuilder<TTriggerEntity> where TTriggerEntity : class
    {
        private Expression<Func<TTriggerEntity, bool>> _triggerCondition;

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

        internal TriggerBuilder() { }

        public UpdateBuilder<TTriggerEntity, TUpdateEntity> Update<TUpdateEntity>(Expression<Func<TTriggerEntity, IQueryable<TUpdateEntity>, IQueryable<TUpdateEntity>>> condition)
            where TUpdateEntity : class
        {
            return new UpdateBuilder<TTriggerEntity, TUpdateEntity>(this, condition);
        }
    }
}
