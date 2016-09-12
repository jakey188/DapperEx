using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Demo.Entites;
using Dapper.Linq.Helpers;

namespace Dapper.Demo
{
    public class TimeTest
    {
        public static void Init(DapperDbContext db)
        {
            LinqToStringTest(db);
        }


        private static void LinqToStringTest(DapperDbContext db)
        {
            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();

            var sql = db.Query<Products>(x => x.ProductId > 0 && x.ReorderLevel == 1).OrderByDescending(x => x.ProductId)
                .Select(x => new {id = x.ProductId}).ToString();

            stopwatch.Stop();

            var time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("耗时：" + time + "毫秒："+ sql);
        }
    }
}
