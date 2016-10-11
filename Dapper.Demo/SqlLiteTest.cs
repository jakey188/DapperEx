using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.SQLite;
using System.Diagnostics;
using System.Data;

namespace Dapper.Demo
{
    public class SqlLiteTest
    {
        public static void Init()
        {
            using (var db = new SQLiteDbContext("SqlLiteName"))
            {
                //Add(db);
                //Query(db);
                //BulkInsert(db);
            }
        }

        private static void Add(SQLiteDbContext db)
        {
            var user = new User
            {
                Age = 1,
                CityId = 11,
                Gender = 1,
                Name = "dfaf",
                OpTime = DateTime.Now
            };
            db.Add<User>(user);
        }

        private static void BulkInsert(SQLiteDbContext db)
        {
            Stopwatch sw = new Stopwatch();

            var dataTable = db.SqlQueryDataTable("select Name,Gender,Age,CityId,OpTime from Users LIMIT 1");

            DataTable dtNew = dataTable.Copy();
            dtNew.Clear();  //清楚数据

            for (var i = 0;i < 100000;i++)
            {
                dtNew.Rows.Add(dataTable.Rows[0].ItemArray);  //添加数据行
            }
            sw.Reset();
            sw.Start();
            db.BulkInsert("Users",dtNew);
            sw.Stop();
            Console.WriteLine("BulkInsert DataTable 耗时：" + sw.ElapsedMilliseconds);

            var user = db.Query<User>(x => x.Id > 0).Take(1).FirstOrDefault();

            var newList = new List<User>();

            for (var i = 0;i < 100000;i++)
            {
                newList.Add(user);
            }
            sw.Reset();
            sw.Start();
            db.BulkInsert<User>("Users",newList);
            sw.Stop();
            Console.WriteLine("BulkInsert List 耗时：" + sw.ElapsedMilliseconds);
        }

        private static void Query(SQLiteDbContext db)
        {

            //var u = new User {
            //    Name = "测试"
            //};

            //db.Add<User>(u);

            var aa1 = db.Query<User>(x => x.Id > 12).Select(x => new { title = x.Name }).ToList();

            var aa = db.Query<User>(x => x.Id > 0).ToList();

            var d = db.SqlQueryDynamic(" select * from users ",null);

            var dta = db.SqlQueryDataTable(" select * from users ",null);

            db.Update<User>(x => x.Id > 0,x => new User { Name = "212" });

            db.Query<User>(x => x.Id == 12).FirstOrDefault();

        }
    }
}
