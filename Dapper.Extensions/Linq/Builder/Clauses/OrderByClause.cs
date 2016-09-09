using System.Linq.Expressions;
using Dapper.Linq.Builder.Visitors;

namespace Dapper.Linq.Builder.Clauses
{
    internal class OrderByClause<T>
    {
        private readonly SqlBuilder<T> _builder;

        public OrderByClause(SqlBuilder<T> builder)
        {
            _builder = builder;
        }

        public void Build(Expression expression,bool desc = false)
        {
            var resolve = new ExpressionResolve();

            resolve.VisitMember(expression).ForEach(x =>
            {
                var order = _builder.Adapter.Field(x.TableName,x.FieldName);
                _builder.Order.Add(order + (desc ? " DESC " : " ASC "));
            });
        }
    }
}
