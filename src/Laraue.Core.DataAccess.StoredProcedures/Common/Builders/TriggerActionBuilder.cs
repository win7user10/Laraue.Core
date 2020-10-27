using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerActionBuilder<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        private Expression<Func<TTriggerEntity, bool>> _actionsCondition;
        private readonly List<IVisitingTrigger> _actionExpressions = new List<IVisitingTrigger>();

        public TriggerActionBuilder<TTriggerEntity> Condition(Expression<Func<TTriggerEntity, bool>> actionsCondition)
        {
            _actionsCondition = actionsCondition;
            return this;
        }

        public TriggerActionBuilder<TTriggerEntity> UpdateAnotherEntity<TUpdateEntity>(
                Expression<Func<TTriggerEntity, TUpdateEntity, bool>> setFilter,
                Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
            where TUpdateEntity : class
        {
            _actionExpressions.Add(new TriggerUpdateAction<TTriggerEntity, TUpdateEntity>(setFilter, setValues));
            return this;
        }

        public string BuildSql(IVisitor builderVisitor)
        {
            throw new NotImplementedException();
        }
    }
}
