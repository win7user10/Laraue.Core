using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System.Collections.Generic;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base
{
    public abstract class TriggerActions : ISqlConvertible
    {
        public readonly List<ISqlConvertible> ActionConditions = new List<ISqlConvertible>();

        public readonly List<ISqlConvertible> ActionExpressions = new List<ISqlConvertible>();

        public abstract string BuildSql(ITriggerSqlVisitor visitor);
    }
}