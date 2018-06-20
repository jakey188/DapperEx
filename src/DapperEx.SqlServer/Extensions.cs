using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperEx.SqlServer;

namespace DapperEx.SqlServer
{
    public static class Extensions
    {
        public static SqlServerDbContext GetDapperDbContext(this IDbConnection connection)
        {
            return new SqlServerDbContext(connection);
        }
    }
}
