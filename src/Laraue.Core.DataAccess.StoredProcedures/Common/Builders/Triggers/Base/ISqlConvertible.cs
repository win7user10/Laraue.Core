using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base
{
    public interface ISqlConvertible
    {
        public string BuildSql(ITriggerSqlVisitor visitor);
    }
}