using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Linq.Helpers;

namespace Dapper.Demo
{
    public class TimeTest
    {
        public static void Init(SqlServerDbContext db)
        {
            LinqToStringTest(db);
        }


        private static void LinqToStringTest(SqlServerDbContext db)
        {
            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();

            var sql = db.Query<User>(x => x.Id > 0 && x.Age == 1).OrderByDescending(x => x.Id)
                .Select(x => new {id = x.Id}).ToString();

            stopwatch.Stop();

            var time = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("耗时：" + time + "毫秒："+ sql);
        }
    }
}
