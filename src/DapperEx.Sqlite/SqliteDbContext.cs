
using System;
using System.Data;

namespace DapperEx.Sqlite
{
    public class SqliteDbContext : DapperDbContext
    {
        internal SqliteDbContext(IDbConnection connection)
        {
            SetAdapter(EnmDbType.Sqlite);
            CreateDbConnection(connection);
        }

        /// <summary>
        /// 初始化连接字符
        /// </summary>
        /// <param name="connectionString">连接字符串名称</param>
        public SqliteDbContext(string connectionString = "")
        {

            SetAdapter(EnmDbType.Sqlite);

#if NET452
            //DbProviderFactory = System.Data.SQLite.SQLiteFactory.Instance;
            CreateDbConnection(new System.Data.SQLite.SQLiteConnection(connectionString));

#endif

#if NETSTANDARD2_0
            //DbProviderFactory = Microsoft.Data.Sqlite.SqliteFactory.Instance;
            CreateDbConnection(new Microsoft.Data.Sqlite.SqliteConnection(connectionString));
#endif
        }
    }
}
