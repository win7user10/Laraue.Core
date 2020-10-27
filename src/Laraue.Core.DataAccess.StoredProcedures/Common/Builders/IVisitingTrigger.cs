using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public interface IVisitingTrigger
    {
        public string BuildSql(IVisitor builderVisitor);
    }
}