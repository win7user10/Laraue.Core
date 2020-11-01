using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate
{
    public class OnUpdateTriggerUpdateAction<TTriggerEntity, TUpdateEntity> : TriggerUpdateAction<TTriggerEntity, TUpdateEntity>
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public OnUpdateTriggerUpdateAction(
            Expression<Func<TTriggerEntity, TTriggerEntity, TUpdateEntity, bool>> setFilter,
            Expression<Func<TTriggerEntity, TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
                : base (setFilter, setValues)
        {
        }

        public override string BuildSql(ITriggerSqlVisitor visitor)
        {
            return visitor.GetTriggerUpdateActionSql(this);
        }
    }
}
