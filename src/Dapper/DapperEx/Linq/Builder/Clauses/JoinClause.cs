using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.Linq.Builder.Visitors;
using Dapper.Linq.Helpers;

namespace Dapper.Linq.Builder.Clauses
{
    internal class JoinClause<T>
    {
        private readonly SqlBuilder<T> _builder;

        public JoinClause(SqlBuilder<T> builder)
        {
            _builder = builder;
        }

        public void Build<T1, TResult>(Expression<Func<T1,TResult,bool>> expression,JoinType joinType)
        {
            var joinExpression = Helper.GetBinaryExpression(expression.Body);

            var resolve = new ExpressionResolve();

            var originalMember = resolve.VisitMember(joinExpression.Left).FirstOrDefault();
            var joinMember = resolve.VisitMember(joinExpression.Right).FirstOrDefault();

            var originalTable = CacheHelper.GetTableInfo(typeof(T));
            var joinTable = CacheHelper.GetTableInfo(typeof(TResult));

            var joinTypeString = string.Empty;

            switch (joinType)
            {
                case JoinType.InnerJoin:
                    joinTypeString = "INNER JOIN";
                    break;
                case JoinType.LeftJoin:
                    joinTypeString = "LEFT JOIN";
                    break;
                case JoinType.RightJoin:
                    joinTypeString = "RIGHT JOIN";
                    break;
            }

            var joinString = string.Format(" {0} {1} ON {2} = {3}",
                                            joinTypeString,
                                           _builder.Adapter.Table(joinTable.Name,joinTable.Alias),
                                           _builder.Adapter.Field(originalMember.TableAliasName,originalMember.FieldName),
                                           _builder.Adapter.Field(joinMember.TableAliasName,joinMember.FieldName));

            _builder.Table = _builder.Table + joinString;
        }
    }
}
