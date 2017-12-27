using System.Collections.Generic;
using System.Linq.Expressions;
using Dapper.Linq;
using DapperEx.Linq.Builder.Visitors;

namespace DapperEx.Linq.Builder.Clauses
{
    internal class GroupByClause<T>
    {
        private readonly SqlBuilder<T> _builder;

        public GroupByClause(SqlBuilder<T> builder)
        {
            _builder = builder;
        }

        public void Build(Expression expression)
        {
            var resolve = new ExpressionResolve();
            var memberList = new List<string>();
            resolve.VisitMember(expression).ForEach(x =>
            {
                memberList.Add(_builder.Adapter.Field((_builder.IsEnableAlias ? x.TableAliasName : x.TableName),x.FieldName));
            });
            _builder.GroupBy = " GROUP BY " + string.Join(",",memberList);
        }
    }
}
