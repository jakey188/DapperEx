using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Dapper;

namespace Dapper
{
    public static class BulkInsertExtension
    {
        public const int DefaultBatchSize = 1000;

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="context">当前DbContext</param>
        /// <param name="tableName">要插入的表名称</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="batchSize">每次插入的数量</param>
        /// <remarks>数据一致性,请开启事务</remarks>
        public static void BulkInsert(this SqlServerDbContext context, string tableName, DataTable dataTable,
            int batchSize = DefaultBatchSize)
        {
            var provider = new BulkInsertSqlServerProvider(context);
            provider.BulkInsert(tableName, dataTable, batchSize);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="context">当前DbContext</param>
        /// <param name="tableName">要插入的表名称</param>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="batchSize">每次插入的数量</param>
        /// <remarks>数据一致性,请开启事务</remarks>
        public static void BulkInsert(this SqlServerDbContext context,string tableName, IDataReader dataReader,
            int batchSize = DefaultBatchSize)
        {
            var provider = new BulkInsertSqlServerProvider(context);
            provider.BulkInsert(tableName, dataReader, batchSize);
        }

        /// <summary>
        /// 批量插入,不建议使用List插入、性能会受一定影响
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="tableName"></param>
        /// <param name="list">List列明必须与数据表列一致,严格大小写区分</param>
        /// <param name="batchSize"></param>
        public static void BulkInsert<T>(this SqlServerDbContext context, string tableName, IList<T> list,
            int batchSize = DefaultBatchSize)
        {
            var provider = new BulkInsertSqlServerProvider(context);
            provider.BulkInsert(tableName, list, batchSize);
        }

    }
}
