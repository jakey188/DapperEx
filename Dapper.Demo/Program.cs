using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Demo.Entites;
using Dapper.Linq;

namespace Dapper.Demo
{
    public class newProducts {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var list = new List<Products>(); 
            SqlLiteTest();

            using (var db = new DapperDbContext("Group_Set"))
            {
                LinqTestcs.Create(db);
                for (var i = 0;i < 100;i++)
                {
                    //TimeTest.Init(db);

                }
                LinqTestcs.Create(db);
            }
            Console.ReadKey();
        }

        static void SqlLiteTest() {
            using (var db = new SQLiteDbContext("SqlLiteName"))
            {

                //var u = new User {
                //    Name = "测试"
                //};

                //db.Add<User>(u);

                var aa1 = db.Query<User>(x => x.Id > 12).Select(x=>new { title = x.Name }).ToList();
                var aa = db.Query<User>(x => x.Id > 0).ToList();

                db.Update<User>(x => x.Id > 0,x=>new User { Name = "212" });

                db.Query<User>(x => x.Id == 12).FirstOrDefault();

            }
        }

    }

}
