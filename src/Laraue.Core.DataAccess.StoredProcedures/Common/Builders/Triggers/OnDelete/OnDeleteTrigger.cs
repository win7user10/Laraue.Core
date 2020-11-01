using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate
{
    public class OnDeleteTrigger<TTriggerEntity> : Trigger<TTriggerEntity>
        where TTriggerEntity : class
    {
        public OnDeleteTrigger(TriggerTime triggerTime) : base(TriggerType.Delete, triggerTime)
        {
        }

        public OnDeleteTrigger<TTriggerEntity> Action(Action<OnDeleteTriggerActions<TTriggerEntity>> actionSetup)
        {
            var actionTrigger = new OnDeleteTriggerActions<TTriggerEntity>();
            actionSetup.Invoke(actionTrigger);
            Actions.Add(actionTrigger);
            return this;
        }

        public override string BuildSql(ITriggerSqlVisitor visitor)
        {
            return visitor.GetTriggerSql(this);
        }
    }
}
