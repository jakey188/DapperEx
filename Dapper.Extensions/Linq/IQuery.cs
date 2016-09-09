using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.Linq
{
    public interface IQuery<T>
    {
        IQuery<TResult> Select<TResult>(Expression<Func<T,TResult>> selector);
        IQuery<T> Where(Expression<Func<T,bool>> predicate);
        IQuery<T> OrderBy(Expression<Func<T,object>> predicate);
        IQuery<T> OrderByDescending(Expression<Func<T,object>> predicate);
        IQuery<T> ThenBy(Expression<Func<T,object>> predicate);
        IQuery<T> ThenByDescending(Expression<Func<T,object>> predicate);
        IQuery<T> Take(int count);
        IGrouping<T> GroupBy<TKey>(Expression<Func<T,TKey>> predicate);
        T First();
        T FirstOrDefault();
        List<T> ToList();
        List<T> ToPageList(int pageIndex,int pageSize,out int total);
        bool Any();
        int Count();
        TResult Max<TResult>(Expression<Func<T,TResult>> selector);
        TResult Min<TResult>(Expression<Func<T,TResult>> selector);
        TResult Sum<TResult>(Expression<Func<T,TResult>> selector);
    }
}
