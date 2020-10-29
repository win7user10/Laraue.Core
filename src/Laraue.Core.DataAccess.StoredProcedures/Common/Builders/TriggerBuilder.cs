using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerBuilder<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        private TriggerType _triggerType { get; }

        public TriggerTime _triggerTime { get; }

        private readonly List<IVisitingTrigger> _actions = new List<IVisitingTrigger>();

        public TriggerBuilder(TriggerType triggerType, TriggerTime triggerTime)
        {
            _triggerType = triggerType;
            _triggerTime = triggerTime;
        }

        public TriggerBuilder<TTriggerEntity> Action(Action<TriggerActionBuilder<TTriggerEntity>> actionSetup)
        {
            var actionTrigger = new TriggerActionBuilder<TTriggerEntity>();
            actionSetup.Invoke(actionTrigger);
            _actions.Add(actionTrigger);
            return this;
        }

        public string BuildSql(IVisitor visitor)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("BEGIN");
            foreach (var action in _actions)
            {
                sqlBuilder.Append(action.BuildSql(visitor));
            }
            sqlBuilder.Append("END;");

            return sqlBuilder.ToString();
        }
    }
}
