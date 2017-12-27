using Dapper.Demo;
using DapperEx.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperEx.Demo.Tests
{
    public class Test
    {
        public static void Init()
        {
            var db = new SqliteDbContext("Data Source=DapperEx.db");
            
                var aa = db.Query<User>(x => x.Id == 1).FirstOrDefault();
            

            using (var d = new UserContext())
            {
                using (var dapper = d.Database.GetDbConnection().GetDapperDbContext())
                {
                    var a = dapper.Query<User>(x => x.Id == 1).FirstOrDefault();
                }
            }
        }
    }
}
