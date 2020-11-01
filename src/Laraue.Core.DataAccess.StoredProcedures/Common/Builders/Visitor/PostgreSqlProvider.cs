using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.Base;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers
{
    public class PostgreSqlProvider : BaseTriggerSqlVisitor
    {
        public PostgreSqlProvider(IModel model) : base(model)
        {
        }

        protected override string NewEntityPrefix => "NEW";

        protected override string OldEntityPrefix => "OLD";

        public override string GetDropTriggerSql(string triggerName, Type entityType)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"DROP FUNCTION {triggerName}();")
                .Append($"DROP TRIGGER {triggerName} ON {GetTableName(entityType)};");
            return sqlBuilder.ToString();
        }

        public override string GetTriggerActionsSql(TriggerActions triggerActions)
        {
            var sqlBuilder = new StringBuilder();
            if (triggerActions.ActionConditions.Count > 0)
            {
                sqlBuilder.Append($"IF ");
                foreach (var actionCondition in triggerActions.ActionConditions)
                {
                    sqlBuilder.Append(actionCondition.BuildSql(this));
                }
                sqlBuilder.Append($" THEN ");
            }

            foreach (var actionExpression in triggerActions.ActionExpressions)
            {
                sqlBuilder.Append(actionExpression.BuildSql(this))
                    .Append(";");
            }

            if (triggerActions.ActionConditions.Count > 0)
                sqlBuilder.Append($"END IF;");

            sqlBuilder.Append($"RETURN NEW;");

            return sqlBuilder.ToString();
        }

        public override string GetTriggerConditionSql(LambdaExpression triggerCondition, Dictionary<string, ArgumentPrefix> argumentPrefixes)
        {
            if (triggerCondition.Body is BinaryExpression binaryExpression)
                return GetBinaryExpressionSql(binaryExpression, argumentPrefixes);
            else if (triggerCondition.Body is MemberExpression memberExpression)
                return GetBinaryExpressionSql(Expression.MakeBinary(ExpressionType.Equal, memberExpression, Expression.Constant(true)), argumentPrefixes);
            throw new NotImplementedException($"Trigger condition of type {triggerCondition.Body.GetType()} is not supported.");
        }

        public override string GetTriggerSql<TTriggerEntity>(Trigger<TTriggerEntity> trigger)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"CREATE FUNCTION {trigger.Name}() as ${trigger.Name}$ ")
                .Append("BEGIN ");

            foreach (var action in trigger.Actions)
                sqlBuilder.Append(action.BuildSql(this));

            sqlBuilder.Append(" END;")
                .Append($"${trigger.Name}$ LANGUAGE plpgsql;")
                .Append($"CREATE TRIGGER {trigger.Name} {trigger.TriggerTime.ToString().ToUpper()} {trigger.TriggerType.ToString().ToUpper()} ")
                .Append($"ON {GetTableName(typeof(TTriggerEntity))} FOR EACH ROW EXECUTE PROCEDURE {trigger.Name}()");

            return sqlBuilder.ToString();
        }

        public override string GetTriggerUpdateActionSql<TUpdateEntity>(LambdaExpression condition, LambdaExpression setExpression, Dictionary<string, ArgumentPrefix> argumentPrefixes)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("update ")
                .Append($"{GetTableName(typeof(TUpdateEntity))} ");

            sqlBuilder.Append(GetMemberInitSql((MemberInitExpression)setExpression.Body, argumentPrefixes))
                .Append(" where ")
                .Append(GetBinaryExpressionSql((BinaryExpression)condition.Body, argumentPrefixes));

            return sqlBuilder.ToString();
        }
    }
}
