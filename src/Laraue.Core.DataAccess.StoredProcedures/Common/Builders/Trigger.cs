using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class Trigger<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        public readonly List<IVisitingTrigger> Actions = new List<IVisitingTrigger>();

        public Trigger(TriggerType triggerType, TriggerTime triggerTime)
        {
            TriggerType = triggerType;
            TriggerTime = triggerTime;
        }

        public Trigger<TTriggerEntity> Action(Action<TriggerAction<TTriggerEntity>> actionSetup)
        {
            var actionTrigger = new TriggerAction<TTriggerEntity>();
            actionSetup.Invoke(actionTrigger);
            Actions.Add(actionTrigger);
            return this;
        }

        public string BuildSql(IVisitor visitor)
        {
            var sql = visitor.GetTriggerSql(this);
            return sql;
        }

        public string Name => $"{TriggerTime}_{TriggerTime}_{typeof(TTriggerEntity).Name}";
    }
}
