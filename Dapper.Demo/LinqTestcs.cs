using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Demo.Entites;

namespace Dapper.Demo
{
    public class LinqTestcs
    {

        public static void Create(DapperDbContext db)
        {
            Join(db);
            //Delete(db);
            //Update(db);
            //PagedTest(db);
            //WhereTest(db);
            //SelectTest(db);
            //GroupByTest(db);
            //TakeTest(db);
        }

        private static void Join(DapperDbContext db)
        {
            var query = db.Query<Products>().Where(x => x.ProductId > 0)
                .LeftJoin<Categorys>((p,c) => p.CategoryId == c.CategoryId)
                .Where(x => x.CategoryName == "aa");

            var sql = query.ToString();

        }

        private static void Delete(DapperDbContext db)
        {
            db.Delete<Products>(x => x.ProductId > 70);
        }

        private static void Update(DapperDbContext db)
        {
            db.Update<Products>(x => x.ProductId > 70, x => new Products {ProductName = "测试分类1"});
        }

        private static void PagedTest(DapperDbContext db)
        {
            int total = 0;
            var a = db.Query<Products>(x=>x.ProductId>0)
                .OrderBy(x => x.ProductId)
                .ThenByDescending(x => x.ReorderLevel)
                .ToPageList(1, 10, out total);
        }

        private static void WhereTest(DapperDbContext db)
        {
            var list = db.Query<Products>(x => x.CategoryId > 75).ToList();


            db.Query<Products>(x=>x.ProductId>0).Where(x => x.CategoryId > 0).Where(x=>x.ProductName=="22").ToList();

            var aa = new List<int>() {1,2};//只支持List类型

            db.Query<Products>(x => aa.Contains(x.ProductId));

            db.Query<Products>(x => x.ProductName.Contains("a") && x.ProductName.StartsWith("n") && x.ProductName.EndsWith("aa") && string.IsNullOrEmpty(x.ProductName))
                .ToList();


        }

        private static void GroupByTest(DapperDbContext db)
        {
            db.Query<Products>().GroupBy(x=> new { x.ProductId}).Select(x=>x.ProductId).ToList();
        }

        private static void TakeTest(DapperDbContext db)
        {
            db.Query<Products>().Take(10).Select(x => x.ProductId).ToList();
            db.Query<Products>().Take(10).ToList();
        }

        private static void SelectTest(DapperDbContext db)
        {
            db.Query<Products>().Select(x => x.ProductId).ToList();

            db.Query<Products>().Select(x => new Products { ProductId = x.ProductId }).ToList();

            db.Query<Products>().Select(x => new { x.ProductId }).ToList();

            db.Query<Products>().Select(x => new { id = x.ProductId,name=x.ProductName }).ToList();

            db.Query<Products>().Select(x => new newProducts { Id = x.ProductId,Name = x.ProductName }).ToList();
        }
    }
}
