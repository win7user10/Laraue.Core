using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor
{
    class PostgreSqlVisitor : SqlVisitor
    {
        public PostgreSqlVisitor(IModel model) : base(model)
        {
        }

        public override string GetMemberExpressionSql(MemberExpression memberExpression, Type newMemberType, TriggerType triggerType)
        {
            var sqlBuilder = new StringBuilder();

            if (newMemberType == memberExpression.Member.DeclaringType)
                sqlBuilder.Append(GetTableName(memberExpression.Member));
            else
            {
                sqlBuilder.Append(triggerType switch
                {
                    TriggerType.Insert => "NEW",
                    TriggerType.Update => "NEW",
                    _ => throw new NotImplementedException(),
                });
            }

            sqlBuilder.Append('.')
                .Append(GetColumnName(memberExpression.Member));

            return sqlBuilder.ToString();
        }

        public override string GetTriggerActionSql<TTriggerEntity>(TriggerAction<TTriggerEntity> triggerAction)
        {
            var sqlBuilder = new StringBuilder();
            if (triggerAction.ActionsCondition != null)
                sqlBuilder.Append($"IF condition THEN ");

            foreach (var actionExpression in triggerAction.ActionExpressions)
            {
                sqlBuilder.Append(actionExpression.BuildSql(this))
                    .Append(";");
            }

            if (triggerAction.ActionsCondition != null)
                sqlBuilder.Append($"END IF;");

            sqlBuilder.Append($"RETURN NEW");

            return sqlBuilder.ToString();
        }

        public override string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(TriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("update ")
                .Append($"{GetTableName(typeof(TUpdateEntity))} ")
                .Append(GetMemberInitSql((MemberInitExpression)triggerUpdateAction.SetExpression.Body, typeof(TUpdateEntity), TriggerType.Update))
                .Append(" where ")
                .Append(GetBinaryExpressionSql((BinaryExpression)triggerUpdateAction.SetFilter.Body, typeof(TUpdateEntity), TriggerType.Update));

            return sqlBuilder.ToString();
        }

        public override string GetTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("BEGIN ");
            foreach (var action in trigger.Actions)
            {
                sqlBuilder.Append(action.BuildSql(this));
            }
            sqlBuilder.Append(" END;");

            return sqlBuilder.ToString();
        }
    }
}
