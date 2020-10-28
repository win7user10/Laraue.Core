using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Laraue.Core.Extensions;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders
{
    public class TriggerUpdateAction<TTriggerEntity, TUpdateEntity> : VisitingTrigger
        where TTriggerEntity : class
        where TUpdateEntity : class
    {
        public Expression<Func<TTriggerEntity, TUpdateEntity, bool>> _setFilter;
        public Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> _setExpression;

        public TriggerUpdateAction(
            IModel model,
            Expression<Func<TTriggerEntity, TUpdateEntity, bool>> setFilter,
            Expression<Func<TTriggerEntity, TUpdateEntity, TUpdateEntity>> setValues)
                : base(model)
        {
            _setFilter = setFilter;
            _setExpression = setValues;
        }

        public override string BuildSql()
        {
            var actionSql = ActionSql;

            throw new NotImplementedException();
        }

        public string ActionSql
        {
            get
            {
                var setExpression = (MemberInitExpression)_setExpression.Body;
                var setExpressionBindings = setExpression.Bindings;

                foreach (var memberBinding in setExpressionBindings)
                {
                    var assignment = (BinaryExpression)((MemberAssignment)memberBinding).Expression;
                    var assignmentSql = GetBinarySql(assignment);
                }

                return "";
            }
        }

        private string GetBinarySql(BinaryExpression binaryExpression)
        {
            var sqlBuilder = new StringBuilder();
            var parts = new[] { binaryExpression.Left, binaryExpression.Right };
            foreach (var part in parts)
            {
                if (part is MemberExpression memberExpression)
                    sqlBuilder.Append(GetMemberExpressionSql(memberExpression));
                else if (part is ConstantExpression constantExpression)
                    sqlBuilder.Append(GetConstantExpressionSql(constantExpression));
                else if (part is BinaryExpression binaryExp)
                    sqlBuilder.Append(GetBinarySql(binaryExp));
                else
                    throw new InvalidOperationException($"{part.GetType()} expression does not supports in set statement.");

                if(part != binaryExpression.Right)
                    sqlBuilder.Append(GetExpressionTypeSign(binaryExpression.NodeType));
            }
            return sqlBuilder.ToString();
        }

        private string GetMemberExpressionSql(MemberExpression memberExpression)
        {
            var sqlBuilder = new StringBuilder();
            if (memberExpression.Member.DeclaringType == typeof(TTriggerEntity))
                sqlBuilder.Append("NEW.");
            sqlBuilder.Append(GetColumnName(memberExpression.Member));
            return sqlBuilder.ToString();
        }

        private string GetConstantExpressionSql(ConstantExpression constantExpression) => constantExpression.Value.ToString();
    }
}
