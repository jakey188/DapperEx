using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.SQLite;

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
                BulkInsert(db);
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

            var list = db.SqlQueryDataTable("select Name,Gender,Age,CityId,OpTime from Users LIMIT 2");
            db.BulkInsert("Users",list);

            //var list = db.Query<User>(x => x.Id > 0).Take(2).ToList();
            //db.BulkInsert<User>("Users", list);
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
