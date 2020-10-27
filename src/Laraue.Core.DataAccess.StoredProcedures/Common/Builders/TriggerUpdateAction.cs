using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using System;
using System.Linq.Expressions;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerUpdateAction<TTriggerEntity, TUpdateEntity> : IVisitingTrigger
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression<Func<TTriggerEntity, TUpdateEntity, bool>> _setFilter;
        public Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> _setExpression;

        public TriggerUpdateAction(
            Expression<Func<TTriggerEntity, TUpdateEntity, bool>> setFilter,
            Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
        {
            _setFilter = setFilter;
            _setExpression = setValues;
        }

        public string BuildSql(IVisitor builderVisitor)
        {
            throw new NotImplementedException();
        }
    }
}
