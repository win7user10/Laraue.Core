using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerAction<TTriggerEntity> : ISqlTrigger
        where TTriggerEntity : class
    {
        public readonly List<ISqlTrigger> ActionConditions = new List<ISqlTrigger>();

        public readonly List<ISqlTrigger> ActionExpressions = new List<ISqlTrigger>();

        public TriggerAction<TTriggerEntity> Condition(Expression<Func<TTriggerEntity, bool>> condition)
        {
            ActionConditions.Add(new TriggerCondition<TTriggerEntity>(condition));
            return this;
        }

        public TriggerAction<TTriggerEntity> UpdateAnotherEntity<TUpdateEntity>(
                Expression<Func<TTriggerEntity, TUpdateEntity, bool>> entityFilter,
                Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
            where TUpdateEntity : class
        {
            ActionExpressions.Add(new TriggerUpdateAction<TTriggerEntity, TUpdateEntity>(entityFilter, setValues));
            return this;
        }

        public string BuildSql(IProvider visitor)
        {
            return visitor.GetTriggerActionSql(this);
        }
    }
}