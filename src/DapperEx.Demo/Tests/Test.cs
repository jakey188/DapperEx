using Dapper.Demo;
using DapperEx.MySql;
using DapperEx.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DapperEx.Sqlite.BulkInserts;
using System.Data;

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

        

        private static void Add(DapperDbContext db)
        {
            var user = new User
            {
                Age = 1,
                Gender = EnmUserGender.女,
                Name = "张三",
                OpTime = DateTime.Now,
                Gender1 = true
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

            var a8 = db.Query<User>(x => x.Id > 0).Max(x => x.Id);

            var a9 = db.Query<User>().Where("age=1").FirstOrDefault();
            var id = 10;
            var a10 = db.Query<User>(x=>x.Id==id).FirstOrDefault();

            var g = true;
            var a11 = db.Query<User>(x => x.Gender1 && x.Gender1==g).FirstOrDefault();
        }
    }
}
