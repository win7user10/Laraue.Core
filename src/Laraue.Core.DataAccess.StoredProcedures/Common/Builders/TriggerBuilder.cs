using EFSqlTranslator.Translation;
using EFSqlTranslator.Translation.DbObjects;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Laraue.Core.Extensions.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerBuilder<TTriggerEntity> : IVisitingTrigger
        where TTriggerEntity : class
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        private IModelInfoProvider _modelInfoProvider;
        
        private IDbObjectFactory _dbObjectFactory;

        private readonly List<IVisitingTrigger> _actions = new List<IVisitingTrigger>();

        public TriggerBuilder(IModelInfoProvider modelInfoProvider, IDbObjectFactory dbObjectFactory, TriggerType triggerType, TriggerTime triggerTime)
        {
            TriggerType = triggerType;
            TriggerTime = triggerTime;
            _dbObjectFactory = dbObjectFactory;
            _modelInfoProvider = modelInfoProvider;
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
            return "sql";
        }
    }
}
