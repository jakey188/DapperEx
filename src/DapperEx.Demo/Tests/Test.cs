using Dapper.Demo;
using DapperEx.MySql;
using DapperEx.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperEx.Demo.Tests
{
    public class Test
    {

        public static void Init(DapperDbContext db)
        {
            var user = db.Query<User>(x => x.Name == "张三").FirstOrDefault();
            Add(db);
            Delete(db);
            Update(db);
            Query(db);
        }

        //private static void BulkInsert(DapperDbContext db)
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

        private static void Add(DapperDbContext db)
        {
            var user = new User
            {
                Age = 1,
                Gender = EnmUserGender.女,
                Name = "张三",
                OpTime = DateTime.Now
            };
            db.Add<User>(user);
        }

        private static void Update(DapperDbContext db)
        {
            var user = db.Query<User>(x => x.Name == "张三").FirstOrDefault();
            user.Age = 20;
            db.Update(user);
            db.Update<User>(x => x.Name == "张三", x => new User { Gender = EnmUserGender.男 });
        }

        private static void Delete(DapperDbContext db)
        {
            var user = db.Query<User>(x => x.Name == "张三").FirstOrDefault();
            db.Delete(user);
            Add(db);
            db.Delete<User>(x => x.Name == "张三");
            Add(db);
            db.DeleteAll<User>();
            Add(db);
        }

        private static void Query(DapperDbContext db)
        {
            var a1 = db.Query<User>(x => x.Name == "张三").Select(x => new { title = x.Name }).ToList();

            var a2 = db.Query<User>(x => x.Id > 0).ToList();

            var a3 = db.SqlQueryDynamic(" select * from users ", null);

            var a4 = db.Query<User>(x => x.Id == 12).FirstOrDefault();

            var a5 = db.Query<User>(x => x.Id > 1).OrderBy(x => x.Id).ThenByDescending(x => x.Age).ToList();
            var total = 0;

            var a6 = db.Query<User>(x => x.Id > 1).OrderBy(x => x.Id).ThenByDescending(x => x.Age).ToPageList(1, 5, out total);

            var a7 = db.Query<User>(x => x.Id > 0).GroupBy(x => x.Age).Select(x => x.Id).ToList();

            var a8 = db.Query<User>(x => x.Id > 0).Max(x => x.Id); ;
        }
    }
}
