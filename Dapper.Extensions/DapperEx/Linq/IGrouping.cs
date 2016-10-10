using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Linq
{
    public interface IGrouping<T>
    {
        IQuery<TResult> Select<TResult>(Expression<Func<T,TResult>> selector);
    }
}
