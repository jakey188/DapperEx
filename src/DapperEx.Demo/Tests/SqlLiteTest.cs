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
using DapperEx.Demo.Tests;
using DapperEx.Sqlite.BulkInserts;

namespace Dapper.Demo
{
    public class SqlLiteTest
    {
        static string conString = "Data Source=DapperEx.db";

        public static void Init()
        {
            using (var db = new SqliteDbContext(conString))
            {
                
                //db.DeleteAll<User>();
                var user = new User
                {
                    Age = 1,
                    Gender = EnmUserGender.女,
                    Name = "张三",
                    OpTime = DateTime.Now,
                    Gender1 = false
                };
                //db.Add<User>(user);
                //BulkInsert(db);
                Test.Init(db);
            }
        }

        private static void BulkInsert(SqliteDbContext db)
        {
            Stopwatch sw = new Stopwatch();
            var user = db.Query<User>(x => x.Id > 0).Take(1).FirstOrDefault();

            //var user = db.Query<User>(x => x.Id > 0).Take(1).FirstOrDefault();

            var newList = new List<User>();

            for (var i = 0; i < 2; i++)
            {
                newList.Add(user);
            }
            sw.Reset();
            sw.Start();
            db.BulkInsert<User>("Users", newList);
            sw.Stop();
            Console.WriteLine("BulkInsert List 耗时：" + sw.ElapsedMilliseconds);
        }

        public void ConnectionTest()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection(conString);
            var dapper = connection.GetDapperDbContext();
            var d = new UserContext();
            var a1 = d.Database.GetDbConnection().GetDapperDbContext();
            var u1 = d.User.FirstOrDefault(x => x.Id == 1);
        }
    }
}
