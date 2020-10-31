using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class Trigger<TTriggerEntity> : ISqlTrigger
        where TTriggerEntity : class
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        public readonly List<ISqlTrigger> Actions = new List<ISqlTrigger>();

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

        public string BuildSql(IProvider visitor)
        {
            var sql = visitor.GetTriggerSql(this);
            return sql;
        }

        public string Name => $"{Constants.AnnotationKey}_{TriggerTime}_{TriggerType}_{typeof(TTriggerEntity).Name}";
    }
}
