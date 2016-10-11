# DapperEx 是Dapper的一系列封装和拓展
1.DapperEx支持部分Linq写法,支持批量插入,支持按需修改
2.DapperEx支持SqlServer,SQLite

using (var db = new SqlServerDbContext("ConnectionString"))
{

    var user = new User
    {
        Age = 1,
        CityId = 11,
        Gender = 1,
        Name = "dfaf",
        OpTime = DateTime.Now
    };
    db.Add<User>(user);//单条记录添加

    var dataTable = new DataTable();//要插入的数据集：DataTable
    db.BulkInsert("Users",dataTable);

    var bulkInsertList = new List<User>();//要插入的数据集：List<T>
    db.BulkInsert<User>("Users",bulkInsertList);

    db.Update<User>(x => x.Id == 10,x => new User { Name = "我是DapperEx" });

    db.SqlQueryDynamic("select * from users ",null);

    db.SqlQueryDataTable("select * from users ",null);

    db.Query<Products>(x => x.ProductId > 0).Where(x => x.CategoryId > 0).Where(x => x.ProductName == "22").ToList();

    var aa = new List<int>() { 1,2 };//只支持List类型

    db.Query<Products>(x => aa.Contains(x.ProductId));

    db.Query<Products>(x => x.ProductName.Contains("a") && x.ProductName.StartsWith("n") && x.ProductName.EndsWith("aa") && string.IsNullOrEmpty(x.ProductName))
        .ToList();

    db.Query<Products>().GroupBy(x => new { x.ProductId }).Select(x => x.ProductId).ToList();

    db.Query<Products>().Take(10).Select(x => x.ProductId).ToList();
    db.Query<Products>().Take(10).ToList();

    db.Query<Products>().Select(x => x.ProductId).ToList();

    db.Query<Products>().Select(x => new Products { ProductId = x.ProductId }).ToList();

    db.Query<Products>().Select(x => new { x.ProductId }).ToList();

    db.Query<Products>().Select(x => new { id = x.ProductId,name = x.ProductName }).ToList();

    db.Query<Products>().Select(x => new newProducts { Id = x.ProductId,Name = x.ProductName }).ToList();

}