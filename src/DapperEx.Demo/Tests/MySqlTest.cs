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
using DapperEx.MySql;
using DapperEx.Demo.Tests;

namespace Dapper.Demo
{
    public class MySqlTest
    {
        static string conString = "Server=localhost;DataBase=test;User ID=root;Pwd=123456;Connect timeout=30";

        public static void Init()
        {
            using (var db = new MySqlDbContext(conString))
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
