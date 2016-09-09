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
            WhereTest(db);
            SelectTest(db);
            GroupByTest(db);
            TakeTest(db);
        }

        private static void WhereTest(DapperDbContext db)
        {
            db.Query<Products>(x => x.CategoryId > 0).ToList();
            
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
