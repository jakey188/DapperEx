using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using DapperEx.SqlServer.BulkInserts.Providers;

namespace DapperEx.SqlServer.BulkInserts
{
    public static class BulkInsertExtension
    {
        public const int DefaultBatchSize = 1000;
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="context">当前DbContext</param>
        /// <param name="tableName">要插入的表名称</param>
        /// <param name="dataReader">IDataReader</param>
        public static int BulkInsert(this SqlServerDbContext context, string tableName, DbDataReader dataReader)
        {
            var provider = new BulkInsertSqlServerProvider(context);
            var result = provider.BulkInsert(tableName, dataReader);
            return result;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="context">当前DbContext</param>
        /// <param name="tableName">要插入的表名称</param>
        /// <param name="dataTable">DataTable</param>
        public static int BulkInsert(this SqlServerDbContext context, string tableName, DataTable dataTable)
        {
            var provider = new BulkInsertSqlServerProvider(context);
            var result = provider.BulkInsert(tableName, dataTable);
            return result;
        }

        /// <summary>
        /// 批量插入,不建议使用List插入、性能会受一定影响
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">当前DbContext</param>
        /// <param name="tableName">要插入的表名称</param>
        /// <param name="list">List列明必须与数据表列一致,严格大小写区分</param>
        public static int BulkInsert<T>(this SqlServerDbContext context, string tableName, IList<T> list)
        {
            var provider = new BulkInsertSqlServerProvider(context);
            var result = provider.BulkInsert(tableName,list);
            return result;
        }

        /// <summary>
        /// 批量插入(SqlServer)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="list">List列明必须与数据表列一致,严格大小写区分</param>
        /// <param name="batchSize"></param>
        public static void BulkInsertSqlBulkCopy<T>(this SqlServerDbContext context, string destinationTableName, IEnumerable<T> list, SqlBulkCopyOptions options,
            int batchSize = DefaultBatchSize) where T : class
        {
            var provider = new BulkInsertSqlServerProvider(context);
            var table = list.ToDataTable();
            BulkInsert(context, table, destinationTableName, options, batchSize);
        }

        public static void BulkInsert(this SqlServerDbContext context, DataTable dataTable, string destinationTableName, SqlBulkCopyOptions options, int batchSize)
        {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(context.Connection as SqlConnection, options, context.Transaction as SqlTransaction))
            {
                sqlBulkCopy.BatchSize = batchSize;
                sqlBulkCopy.DestinationTableName = destinationTableName;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }

        public static void BulkInsert(this SqlServerDbContext context, IDataReader dataReader, string destinationTableName, SqlBulkCopyOptions options, int batchSize)
        {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(context.Connection as SqlConnection, options, context.Transaction as SqlTransaction))
            {
                sqlBulkCopy.BatchSize = batchSize;
                sqlBulkCopy.DestinationTableName = destinationTableName;
                sqlBulkCopy.WriteToServer(dataReader);
            }
        }

        private static DataTable ToDataTable<TResult>(this IEnumerable<TResult> value) where TResult : class
        {
            var pList = new List<PropertyInfo>();
            Type type = typeof(TResult);
            var dt = new DataTable();
            Array.ForEach<PropertyInfo>(type.GetProperties(), p =>
            {
                pList.Add(p);
                dt.Columns.Add(p.Name, p.PropertyType);
            });
            foreach (var item in value)
            {
                DataRow row = dt.NewRow();
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                dt.Rows.Add(row);
            }
            return dt;
        }

    }
}
