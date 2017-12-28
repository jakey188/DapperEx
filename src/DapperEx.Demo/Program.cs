using DapperEx.Demo.Tests;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
            SqlLiteTest.Init();
            //MySqlTest.Init();
            Console.ReadKey();
        }
    }

}
