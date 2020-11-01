using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate
{
    public class OnUpdateTriggerCondition<TTriggerEntity> : TriggerCondition<TTriggerEntity>
        where TTriggerEntity : class
    {
        public OnUpdateTriggerCondition(Expression<Func<TTriggerEntity, TTriggerEntity, bool>> condition) : base(condition)
        {
        }

        public override string BuildSql(ITriggerSqlVisitor visitor)
        {
            return visitor.GetTriggerConditionSql(this);
        }
    }
}
