using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerActionBuilder<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        private Expression<Func<TTriggerEntity, bool>> _actionsCondition;

        private readonly List<IVisitingTrigger> _actionExpressions = new List<IVisitingTrigger>();

        public TriggerActionBuilder()
        {
        }

        public TriggerActionBuilder<TTriggerEntity> Condition(Expression<Func<TTriggerEntity, bool>> actionsCondition)
        {
            _actionsCondition = actionsCondition;
            return this;
        }

        public TriggerActionBuilder<TTriggerEntity> UpdateAnotherEntity<TUpdateEntity>(
                Expression<Func<TTriggerEntity, TUpdateEntity, bool>> entityFilter,
                Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
            where TUpdateEntity : class
        {
            _actionExpressions.Add(new TriggerUpdateAction<TTriggerEntity, TUpdateEntity>(entityFilter, setValues));
            return this;
        }

        public string BuildConditionSql(IVisitor visitor)
        {
            return _actionsCondition is null ? null : "condition";
        }

        public string BuildSql(IVisitor visitor)
        {
            var sqlBuilder = new StringBuilder();
            if (_actionsCondition != null)
                sqlBuilder.Append($"IF {BuildConditionSql(visitor)} THEN ");

            var actionsSql = _actionExpressions.Select(actionExpression => actionExpression.BuildSql(visitor));
            foreach (var actionSql in actionsSql)
                sqlBuilder.Append(actionSql)
                    .Append(';');

            if (_actionsCondition != null)
                sqlBuilder.Append($"END IF;");

            sqlBuilder.Append($"RETURN NEW");

            return sqlBuilder.ToString();
        }
    }
}
