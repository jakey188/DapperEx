using System;
using System.Linq.Expressions;
using Dapper.Linq;

namespace DapperEx.Linq
{
    public interface IGrouping<T>
    {
        IQuery<TResult> Select<TResult>(Expression<Func<T,TResult>> selector);
    }
}
