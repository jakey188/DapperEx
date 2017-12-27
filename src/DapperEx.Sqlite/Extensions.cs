using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperEx.Sqlite;

namespace DapperEx.Sqlite
{
    public static class Extensions
    {
        public static SqliteDbContext GetDapperDbContext(this IDbConnection connection)
        {
            return new SqliteDbContext(connection);
        }
    }
}
