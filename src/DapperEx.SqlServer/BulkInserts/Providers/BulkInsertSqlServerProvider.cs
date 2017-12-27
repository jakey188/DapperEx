using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Dapper
{
    public class BulkInsertSqlServerProvider :BulkInsertProvider
    {
        private SqlServerDbContext _db;

        public BulkInsertSqlServerProvider(SqlServerDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dataTable">插入的数据源</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public override int BulkInsert(string tableName,DataTable dataTable,int batchSize = 1000)
        {
            SqlTransaction tran = _db.Transaction != null ? (SqlTransaction)_db.Transaction : null;
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)_db.Connection,SqlBulkCopyOptions.Default,tran))
            {
                sqlBulkCopy.BatchSize = batchSize;
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.WriteToServer(dataTable);
            }
            return 1;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dataReader">插入的数据源</param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public override int BulkInsert(string tableName,IDataReader dataReader,int batchSize = 1000)
        {
            SqlTransaction tran = _db.Transaction != null ? (SqlTransaction)_db.Transaction : null;
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)_db.Connection,SqlBulkCopyOptions.Default,tran))
            {
                sqlBulkCopy.BatchSize = batchSize;
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.WriteToServer(dataReader);
            }
            return 1;
        }

        /// <summary>
        /// 批量插入,不建议使用List插入、性能会受一定影响
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destinationTableName"></param>
        /// <param name="list">List列明必须与数据表列一致,严格大小写区分</param>
        /// <param name="batchSize"></param>
        public override int BulkInsert<T>(string destinationTableName,IList<T> list,int batchSize = 1000)
        {
            var dataTable = ToDataTable<T>(list);
            return BulkInsert(destinationTableName, dataTable, batchSize);
        }

        private DataTable ToDataTable<TResult>(IEnumerable<TResult> value)
        {
            var pList = new List<PropertyInfo>();
            Type type = typeof(TResult);
            var dt = new DataTable();
            Array.ForEach<PropertyInfo>(type.GetProperties(),p =>
            {
                pList.Add(p);
                dt.Columns.Add(p.Name,p.PropertyType);
            });
            foreach (var item in value)
            {
                DataRow row = dt.NewRow();
                pList.ForEach(p => row[p.Name] = p.GetValue(item,null));
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
