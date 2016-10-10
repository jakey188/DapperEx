using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.Linq.Builder;
using Dapper.Linq.Builder.Clauses;

namespace Dapper.Linq
{
    internal class Grouping<T> : IGrouping<T>
    {
        private readonly SqlBuilder<T> _builder;
        private readonly DbContext _db;
        public Grouping(SqlBuilder<T> builder,DbContext db)
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
