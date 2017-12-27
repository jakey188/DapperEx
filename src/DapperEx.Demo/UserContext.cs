using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Demo;
using Microsoft.EntityFrameworkCore;

namespace DapperEx.Demo
{
    public  class UserContext : DbContext
    {
        public DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DapperEx.db");
        }
    }
}
