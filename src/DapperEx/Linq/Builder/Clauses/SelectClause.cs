using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Linq;
using DapperEx.Linq.Builder.Visitors;
using DapperEx.Linq.Types;

namespace DapperEx.Linq.Builder.Clauses
{
    internal class SelectClause<T>
    {
        private readonly SqlBuilder<T> _builder;

        public SelectClause(SqlBuilder<T> builder)
        {
            _builder = builder;
        }

        public void Build(Expression expression)
        {
            var resolve = new ExpressionResolve();
            _builder.SelectField = new List<string>();



            _builder.SelectField =
                resolve.VisitMember(expression).Select(x =>
                _builder.Adapter.Field((_builder.IsEnableAlias ? x.TableAliasName : x.TableName),x.FieldName,x.FiledAliasName))
                .ToList();
        }


        public void Build(Expression expression,AverageFunction ave)
        {
            var resolve = new ExpressionResolve();
            _builder.SelectField = new List<string>();
            resolve.VisitMember(expression).ForEach(x =>
            {
                var field = string.Empty;
                switch (ave)
                {
                    case AverageFunction.MAX:
                        field = " MAX({0})";
                        break;
                    case AverageFunction.MIN:
                        field = " MIN({0})";
                        break;
                    case AverageFunction.SUM:
                        field = " SUM({0})";
                        break;
                    case AverageFunction.AVG:
                        field = " AVG({0})";
                        break;
                }

                _builder.SelectField.Add(string.Format(field,_builder.Adapter.Field(_builder.IsEnableAlias ? x.TableAliasName : x.TableName, x.FieldName, x.FiledAliasName)));
            });
        }
    }
}
