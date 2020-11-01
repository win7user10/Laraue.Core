using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Triggers.OnUpdate;
using Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Visitor;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Laraue.Core.DataAccess.StoredProcedures.Common.Builders.Providers
{
    public class PostgreSqlProvider : BaseExpressionSqlVisitor, ITriggerSqlVisitor
    {
        public PostgreSqlProvider(IModel model) : base(model)
        {
        }

        protected override string NewEntityPrefix => "NEW";

        protected override string OldEntityPrefix => "OLD";

        public string GetDropTriggerSql(string triggerName, Type entityType)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"DROP FUNCTION {triggerName}();")
                .Append($"DROP TRIGGER {triggerName} ON {GetTableName(entityType)};");
            return sqlBuilder.ToString();
        }

        #region TriggerActions

        public string GetTriggerActionsSql<TTriggerEntity>(OnUpdateTriggerActions<TTriggerEntity> triggerActions) where TTriggerEntity : class
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

        #endregion

        public string GetTriggerConditionSql<TTriggerEntity>(OnUpdateTriggerCondition<TTriggerEntity> triggerCondition) where TTriggerEntity : class
        {
            var lambdaExpression = triggerCondition.Condition as LambdaExpression;

            var argsDictionary = new Dictionary<string, ArgumentPrefix>
            {
                { lambdaExpression.Parameters[0].Name, ArgumentPrefix.Old },
                { lambdaExpression.Parameters[1].Name, ArgumentPrefix.New },
            };

            if (lambdaExpression.Body is BinaryExpression binaryExpression)
                return GetBinaryExpressionSql(binaryExpression, argsDictionary);
            else if (lambdaExpression.Body is MemberExpression memberExpression)
                return GetBinaryExpressionSql(Expression.MakeBinary(ExpressionType.Equal, memberExpression, Expression.Constant(true)), argsDictionary);
            throw new NotImplementedException($"Trigger condition of type {triggerCondition.Condition.GetType()} is not supported.");
        }

        public string GetTriggerSql<TTriggerEntity>(OnUpdateTrigger<TTriggerEntity> trigger) where TTriggerEntity : class
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

        public string GetTriggerUpdateActionSql<TTriggerEntity, TUpdateEntity>(OnUpdateTriggerUpdateAction<TTriggerEntity, TUpdateEntity> triggerUpdateAction)
            where TTriggerEntity : class
            where TUpdateEntity : class
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("update ")
                .Append($"{GetTableName(typeof(TUpdateEntity))} ");

            var setLambda = (LambdaExpression)triggerUpdateAction.SetExpression;
            var setFilterLambda = (LambdaExpression)triggerUpdateAction.SetFilter;

            var expressionArgs = new Dictionary<string, ArgumentPrefix>
            {
                { setLambda.Parameters[0].Name, ArgumentPrefix.Old },
                { setLambda.Parameters[1].Name, ArgumentPrefix.New },
            };

            sqlBuilder.Append(GetMemberInitSql((MemberInitExpression)setLambda.Body, expressionArgs))
                .Append(" where ")
                .Append(GetBinaryExpressionSql((BinaryExpression)setFilterLambda.Body, expressionArgs));

            return sqlBuilder.ToString();
        }
    }
}
