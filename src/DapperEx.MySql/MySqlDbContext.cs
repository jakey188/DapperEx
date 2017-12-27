using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace DapperEx.MySql
{
    public class MySqlDbContext : DapperDbContext
    {
        internal MySqlDbContext(IDbConnection connection)
        {
            SetAdapter(EnmDbType.Sqlite);
            CreateDbConnection(connection);
        }

        /// <summary>
        /// 初始化连接字符
        /// </summary>
        /// <param name="connectionString">连接字符串名称</param>
        public MySqlDbContext(string connectionString = "")
        {
            SetAdapter(EnmDbType.Sqlite);
            CreateDbConnection(new MySqlConnection(connectionString));
        }
    }
}
