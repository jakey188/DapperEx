using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Dapper
{
    public class SQLiteDbContext: DapperDbContext , IDisposable
    {
        /// <summary>
        /// 初始化连接字符
        /// </summary>
        /// <param name="connectionStringName">连接字符串名称</param>
        public SQLiteDbContext(string connectionStringName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new Exception("connectionStringName不能为空");

            var configurationManager = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (string.IsNullOrEmpty(configurationManager.ProviderName))
                throw new Exception("请设置connectionStringName的providerName");

            DbType = EnmDbType.SQLite;

            CreateConnection(configurationManager.ConnectionString);
        }

        /// <summary>
        /// 建立DbConnection
        /// </summary>
        /// <param name="connectionString"></param>
        private void CreateConnection(string connectionString)
        {
            Connection = new SQLiteConnection(connectionString);
            
            if (Connection.State != ConnectionState.Open && Connection.State != ConnectionState.Connecting)
                Connection.Open();
        }
    }
}
