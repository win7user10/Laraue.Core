using EFSqlTranslator.Translation;
using EFSqlTranslator.Translation.DbObjects;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Laraue.Core.Extensions.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerBuilder<TTriggerEntity> : VisitingTrigger
        where TTriggerEntity : class
    {
        private TriggerType _triggerType { get; }

        public TriggerTime _triggerTime { get; }

        private readonly List<IVisitingTrigger> _actions = new List<IVisitingTrigger>();

        public TriggerBuilder(IModel model, TriggerType triggerType, TriggerTime triggerTime) : base(model)
        {
            _triggerType = triggerType;
            _triggerTime = triggerTime;
        }

        public TriggerBuilder<TTriggerEntity> Action(Action<TriggerActionBuilder<TTriggerEntity>> actionSetup)
        {
            var actionTrigger = new TriggerActionBuilder<TTriggerEntity>(Model);
            actionSetup.Invoke(actionTrigger);
            _actions.Add(actionTrigger);
            return this;
        }

        public override string BuildSql()
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("BEGIN");
            foreach (var action in _actions)
            {
                sqlBuilder.Append(action.BuildSql());
            }
            sqlBuilder.Append("END;");

            return sqlBuilder.ToString();
        }
    }
}
