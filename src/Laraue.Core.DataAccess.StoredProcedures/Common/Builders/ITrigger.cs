using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public interface ITrigger
    {
        public TriggerAnnatation Visit(IBuilderVisitor builderVisitor);
    }
}