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

namespace Dapper.Demo
{
    public class SqlLiteTest
    {
        static string conString = "Data Source=DapperEx.db";

        public static void Init()
        {
            using (var db = new SqliteDbContext(conString))
            {
                Test.Init(db);
            }
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
