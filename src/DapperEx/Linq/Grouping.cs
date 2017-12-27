using System;
using System.Linq.Expressions;
using Dapper;
using Dapper.Linq;
using DapperEx.Linq.Builder;
using DapperEx.Linq.Builder.Clauses;

namespace DapperEx.Linq
{
    internal class Grouping<T> : IGrouping<T>
    {
        private readonly SqlBuilder<T> _builder;
        private readonly DapperDbContext _db;
        public Grouping(SqlBuilder<T> builder,DapperDbContext db)
        {
            _builder = builder;
            _db = db;
        }

        public IQuery<TResult> Select<TResult>(Expression<Func<T,TResult>> selector)
        {
            new SelectClause<T>(_builder).Build(selector.Body);

            var builder = new SqlBuilder<TResult>(_db.Adapter)
            {
                Table = _builder.Table,
                Parameters = _builder.Parameters,
                Take = _builder.Take,
                Where = _builder.Where,
                Order = _builder.Order,
                GroupBy = _builder.GroupBy,
                SelectField = _builder.SelectField
            };
            return new Query<TResult>(builder,_db);
        }
    }
}
