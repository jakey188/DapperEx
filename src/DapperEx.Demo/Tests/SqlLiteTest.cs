using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using DapperEx;
using Dapper;
using DapperEx.Demo;
using Microsoft.EntityFrameworkCore;
using DapperEx.Sqlite;

namespace Dapper.Demo
{
    public class SqlLiteTest
    {
        public static void Init()
        {
            //var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=DapperEx.db");
            //var dapper = connection.GetDapperDbContext();
            

            var d = new UserContext();
            //var a1 = d.Database.GetDbConnection();
            //var a11 = a1.GetDapperDbContext();
            var u1 = d.User.FirstOrDefault(x => x.Id == 1);
            var a2 = d.Database.GetDbConnection().GetDapperDbContext();
            a2.Query<User>(x => x.Id > 0).Take(1).FirstOrDefault();
            //var path = AppDomain.CurrentDomain  "Data Source=..\\..\\DapperEx.db";
            using (var db = new SqliteDbContext("Data Source=DapperEx.db"))
            {
                DeleteAll(db);
                Add(db);
                Query(db);
                //BulkInsert(db);
            }
        }

        public static void DeleteAll(SqliteDbContext db) {
            db.DeleteAll<User>();
        }

        private static void Add(SqliteDbContext db)
        {
            var user = new User
            {
                Age = 1,
                Gender = EnmUserGender.女,
                Name = "dfaf",
                OpTime = DateTime.Now
            };
            db.Add<User>(user);
        }

        //private static void BulkInsert(SQLiteDbContext db)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    var user = db.Query<User>(x => x.Id > 0).Take(1).FirstOrDefault();
        //    var dataTable = db.SqlQueryDataTable("select Name,Gender,Age,OpTime from Users LIMIT 1");

        //    DataTable dtNew = dataTable.Copy();
        //    dtNew.Clear();  //清楚数据

        //    for (var i = 0;i < 100000;i++)
        //    {
        //        dtNew.Rows.Add(dataTable.Rows[0].ItemArray);  //添加数据行
        //    }
        //    sw.Reset();
        //    sw.Start();
        //    db.BulkInsert("Users",dtNew);
        //    sw.Stop();
        //    Console.WriteLine("BulkInsert DataTable 耗时：" + sw.ElapsedMilliseconds);

        //    //var user = db.Query<User>(x => x.Id > 0).Take(1).FirstOrDefault();

        //    var newList = new List<User>();

        //    for (var i = 0;i < 100000;i++)
        //    {
        //        newList.Add(user);
        //    }
        //    sw.Reset();
        //    sw.Start();
        //    db.BulkInsert<User>("Users",newList);
        //    sw.Stop();
        //    Console.WriteLine("BulkInsert List 耗时：" + sw.ElapsedMilliseconds);
        //}

        private static void Query(SqliteDbContext db)
        {
            
            //var u = new User {
            //    Name = "测试"
            //};

            //db.Add<User>(u);

            var aa1 = db.Query<User>(x => x.Id > 12).Select(x => new { title = x.Name }).ToList();

            var aa = db.Query<User>(x => x.Id > 0).ToList();

            var d = db.SqlQueryDynamic(" select * from users ",null);

            //var dta = db.SqlQueryDataTable(" select * from users ",null);

            db.Update<User>(x => x.Id > 0,x => new User { Name = "212" });

            db.Query<User>(x => x.Id == 12).FirstOrDefault();

        }
    }
}
