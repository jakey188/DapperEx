using System;
using System.Linq.Expressions;
using DapperEx.Linq;

namespace DapperEx
{
    public static  class LinqDapper
    {
        /// <summary>
        /// 转换成可使用Linq的操作对象
        /// </summary>
        /// <typeparam name="T">当前T</typeparam>
        /// <returns></returns>
        public static IQuery<T> Query<T>(this DapperDbContext db, Expression<Func<T,bool>> expression = null)
        {
            return new Query<T>(db,expression);
        }
    }
}
