using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System.Collections.Generic;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base
{
    public abstract class Trigger<TTriggerEntity> : ISqlConvertible
        where TTriggerEntity : class
    {
        public TriggerType TriggerType { get; }

        public TriggerTime TriggerTime { get; }

        public readonly List<ISqlConvertible> Actions = new List<ISqlConvertible>();

        public Trigger(TriggerType triggerType, TriggerTime triggerTime)
        {
            TriggerType = triggerType;
            TriggerTime = triggerTime;
        }

        public abstract string BuildSql(ITriggerSqlVisitor visitor);

        public string Name => $"{Constants.AnnotationKey}_{TriggerTime}_{TriggerType}_{typeof(TTriggerEntity).Name}";
    }
}
