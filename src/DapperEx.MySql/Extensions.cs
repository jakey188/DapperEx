using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperEx.MySql
{
    public static class Extensions
    {
        public static MySqlDbContext GetDapperDbContext(this IDbConnection connection)
        {
            return new MySqlDbContext(connection);
        }
    }
}
