using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperEx;

namespace Dapper.Demo
{
    public class SqlServiceTest
    {

        public static void Create(SqlServerDbContext db)
        {
            //Join(db);
            //Delete(db);
            //Update(db);
            //PagedTest(db);
            //WhereTest(db);
            //SelectTest(db);
            //GroupByTest(db);
            //TakeTest(db);
            //BlukInsert(db);

        }

        //static void BlukInsert(SqlServerDbContext db)
        //{
            
        //    var datatable = db.SqlQueryDataTable("select * from Users");

        //    db.BulkInsert("Users",datatable);
        //}

        //private static void Join(DapperDbContext db)
        //{
        //    var query = db.Query<User>().Where(x => x.Id > 0)
        //        .LeftJoin<Categorys>((p,c) => p.CategoryId == c.CategoryId)
        //        .Where(x => x.CategoryName == "aa");

        //    var sql = query.ToString();

        //}

        private static void Delete(SqlServerDbContext db)
        {
            db.Delete<User>(x => x.Id > 70);
        }

        private static void Update(SqlServerDbContext db)
        {
            db.Update<User>(x => x.Id > 70, x => new User { Name = "1"});
        }

        private static void PagedTest(SqlServerDbContext db)
        {
            int total = 0;
            var a = db.Query<User>(x=>x.Id>0)
                .OrderBy(x => x.Id)
                .ThenByDescending(x => x.Gender)
                .ToPageList(1, 10, out total);
        }

        private static void WhereTest(SqlServerDbContext db)
        {
            var list = db.Query<User>(x => x.Id > 75).ToList();


            db.Query<User>(x=>x.Id>0).Where(x => x.Age > 0).Where(x=>x.Name == "22").ToList();

            var aa = new List<int>() {1,2};//只支持List类型

            db.Query<User>(x => aa.Contains(x.Id));

            db.Query<User>(x => x.Name.Contains("a") && x.Name.StartsWith("n") && x.Name.EndsWith("aa") && string.IsNullOrEmpty(x.Name))
                .ToList();
        }

        private static void GroupByTest(SqlServerDbContext db)
        {
            db.Query<User>().GroupBy(x=> new { x.Id}).Select(x=>x.Id).ToList();
        }

        private static void TakeTest(SqlServerDbContext db)
        {
            db.Query<User>().Take(10).Select(x => x.Id).ToList();
            db.Query<User>().Take(10).ToList();
        }

        private static void SelectTest(SqlServerDbContext db)
        {
            db.Query<User>().Select(x => x.Id).ToList();

            db.Query<User>().Select(x => new User { Id = x.Id }).ToList();

            db.Query<User>().Select(x => new { x.Id }).ToList();

            db.Query<User>().Select(x => new { id = x.Id,name=x.Name }).ToList();

            db.Query<User>().Select(x => new newProducts { Id = x.Id,Name = x.Name }).ToList();
        }
    }
}
