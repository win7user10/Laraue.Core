using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public interface ISqlTrigger
    {
        public string BuildSql(IProvider visitor);
    }
}