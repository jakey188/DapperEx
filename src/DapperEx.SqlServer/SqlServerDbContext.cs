
using System;
using System.Data;
using System.Data.SqlClient;

namespace DapperEx.SqlServer
{
    public class SqlServerDbContext : DapperDbContext
    {
        internal SqlServerDbContext(IDbConnection connection)
        {
            SetAdapter(EnmDbType.SqlServer);
            CreateDbConnection(connection);
        }

        /// <summary>
        /// 初始化连接字符
        /// </summary>
        /// <param name="connectionString">连接字符串名称</param>
        public SqlServerDbContext(string connectionString = "")
        {

            SetAdapter(EnmDbType.SqlServer);

#if NET452
            //DbProviderFactory = System.Data.SQLite.SQLiteFactory.Instance;
            CreateDbConnection(new SqlConnection(connectionString));

#endif

#if NETSTANDARD2_0
            //DbProviderFactory = Microsoft.Data.Sqlite.SqliteFactory.Instance;
            CreateDbConnection(new SqlConnection(connectionString));
#endif
        }
    }
}
