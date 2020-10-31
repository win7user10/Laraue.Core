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

        public override string GetMemberExpressionSql(MemberExpression memberExpression, Type triggeredEntityType)
        {
            var sqlBuilder = new StringBuilder();

            if (triggeredEntityType != memberExpression.Member.DeclaringType)
                sqlBuilder.Append(GetTableName(memberExpression.Member));
            else
                sqlBuilder.Append("NEW");

            sqlBuilder.Append('.')
                .Append(GetColumnName(memberExpression.Member));

            return sqlBuilder.ToString();
        }

        public override string GetTriggerActionSql<TTriggerEntity>(TriggerAction<TTriggerEntity> triggerAction)
        {
            var sqlBuilder = new StringBuilder();
            if (triggerAction.ActionConditions.Count > 0)
            {
                sqlBuilder.Append($"IF ");
                foreach (var actionCondition in triggerAction.ActionConditions)
                {
                    sqlBuilder.Append(actionCondition.BuildSql(this));
                }
                sqlBuilder.Append($" THEN ");
            }

            foreach (var actionExpression in triggerAction.ActionExpressions)
            {
                sqlBuilder.Append(actionExpression.BuildSql(this))
                    .Append(";");
            }

            if (triggerAction.ActionConditions.Count > 0)
                sqlBuilder.Append($"END IF;");

            sqlBuilder.Append($"RETURN NEW;");

            return sqlBuilder.ToString();
        }

        public override string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(TriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("update ")
                .Append($"{GetTableName(typeof(TUpdateEntity))} ")
                .Append(GetMemberInitSql((MemberInitExpression)triggerUpdateAction.SetExpression.Body, typeof(TTriggerEntity)))
                .Append(" where ")
                .Append(GetBinaryExpressionSql((BinaryExpression)triggerUpdateAction.SetFilter.Body, typeof(TTriggerEntity)));

            return sqlBuilder.ToString();
        }

        public override string GetTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"CREATE FUNCTION {trigger.Name}() as ${trigger.Name}$");
            sqlBuilder.Append("BEGIN ");
            foreach (var action in trigger.Actions)
            {
                sqlBuilder.Append(action.BuildSql(this));
            }
            sqlBuilder.Append(" END;");
            sqlBuilder.Append($"${trigger.Name}$ LANGUAGE plpgsql;");

            return sqlBuilder.ToString();
        }

        public override string GetTriggerConditionSql<TTriggerEntity>(TriggerCondition<TTriggerEntity> triggerCondition)
        {
            if (triggerCondition.Condition.Body is BinaryExpression binaryExpression)
                return GetBinaryExpressionSql(binaryExpression, typeof(TTriggerEntity));
            else if (triggerCondition.Condition.Body is MemberExpression memberExpression)
                return GetBinaryExpressionSql(Expression.MakeBinary(ExpressionType.Equal, memberExpression, Expression.Constant(true)), typeof(TTriggerEntity));
            throw new NotImplementedException($"Trigger condition of type {triggerCondition.Condition.GetType()} is not supported.");
        }
    }
}
