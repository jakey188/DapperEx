using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.Linq;

namespace Dapper
{
    public static  class LinqDapper
    {
        /// <summary>
        /// 转换成可使用Linq的操作对象
        /// </summary>
        /// <typeparam name="T">当前T</typeparam>
        /// <returns></returns>
        public static IQuery<T> Query<T>(this DbContext db, Expression<Func<T,bool>> expression = null)
        {
            return new Query<T>(db,expression);
        }
    }
}
