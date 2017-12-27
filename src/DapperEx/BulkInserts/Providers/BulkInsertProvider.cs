using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace DapperEx.BulkInserts.Providers
{
    public abstract class BulkInsertProvider
    {
        private readonly DapperDbContext _db;

        public BulkInsertProvider(DapperDbContext db)
        {
            _db = db ?? throw new ArgumentNullException("context");
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="destinationTableName">要插入的表名称</param>
        /// <param name="dataTable">DataTable</param>
        /// <param name="batchSize">每次插入的数量</param>
        /// <remarks>数据一致性,请开启事务</remarks>
        public virtual int BulkInsert(string destinationTableName,DataTable dataTable,int batchSize = 1000)
        {
            var sql = GenerateBulkInsertSql(destinationTableName,dataTable);
            return _db.Connection.Execute(sql);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="destinationTableName">要插入的表名称</param>
        /// <param name="dataReader">IDataReader</param>
        /// <param name="batchSize">每次插入的数量</param>
        /// <remarks>数据一致性,请开启事务</remarks>
        public virtual int BulkInsert(string destinationTableName,IDataReader dataReader,int batchSize = 1000)
        {
            var sql = GenerateBulkInsertSql(destinationTableName,dataReader);
            return _db.Connection.Execute(sql);
        }

        /// <summary>
        /// 批量插入,不建议使用List插入、性能会受一定影响
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destinationTableName"></param>
        /// <param name="list">List列明必须与数据表列一致,严格大小写区分</param>
        /// <param name="batchSize"></param>
        public virtual int BulkInsert<T>(string destinationTableName,IList<T> list,int batchSize = 1000)
        {
            var sql = GenerateBulkInsertSql<T>(destinationTableName,list);
            return _db.Connection.Execute(sql);
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dataTable">DataTable</param>
        /// <returns></returns>
        private string GenerateBulkInsertSql(string tableName, DataTable dataTable)
        {
            var values = new StringBuilder();

            var propertiesDictionary = new Dictionary<string,int>();

            foreach (DataColumn column in dataTable.Columns)
            {
                var key = column.ColumnName;
                var value = 0;
                if (column.DataType == typeof(string) || column.DataType == typeof(DateTime?) || column.DataType == typeof(DateTime))
                {
                    value = 1;
                }
                propertiesDictionary.Add(key,value);
            }

            for (var m = 0; m < dataTable.Rows.Count; m++)
            {
                var dr = dataTable.Rows[m];
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];
                    if (i == 0)
                        values.Append("(");

                    var value = dr[column.ColumnName];

                    if (propertiesDictionary[column.ColumnName] == 1)
                    {
                        values.Append("'" + value + "'");
                    }
                    else
                    {
                        values.Append(value);
                    }

                    values.Append(i < dataTable.Columns.Count-1 ? "," : ")");
                }

                if (m < dataTable.Rows.Count-1)
                    values.Append(",");
            }

            return $"INSERT INTO {tableName} ({string.Join(",",propertiesDictionary.Select(x=>x.Key))}) VALUES {values}";
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dataReader">IDataReader</param>
        /// <returns></returns>
        private string GenerateBulkInsertSql(string tableName, IDataReader dataReader)
        {
            var dt = new DataTable();
            dt.Load(dataReader);
            return GenerateBulkInsertSql(tableName,dt);
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="list">IList</param>
        /// <returns></returns>
        private string GenerateBulkInsertSql<T>(string tableName, IList<T> list)
        {
            var type = typeof (T);
            var values = new StringBuilder();

            var properties = type.GetProperties();

            var propertiesDictionary = new Dictionary<string, int>();

            foreach (var item in properties)
            {
                var col = item.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "KeyAttribute").SingleOrDefault() as dynamic;

                if (col == null && item.Name.ToLower() != "id")
                {
                    var key = item.Name;
                    var value = 0;
                    if (item.PropertyType == typeof(string) || item.PropertyType == typeof(DateTime?) || item.PropertyType == typeof(DateTime))
                    {
                        value = 1;
                    }
                    propertiesDictionary.Add(key,value);
                }
            }

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                for (var j = 0; j < properties.Count(); j++)
                {
                    var property = properties[j];
                    if (j == 0)
                        values.Append("(");
                    if (!propertiesDictionary.ContainsKey(property.Name)) continue;
                    if (propertiesDictionary[property.Name] == 1)
                    {
                        values.Append("'" + property.GetValue(item) + "'");
                    }
                    else
                    {
                        values.Append(property.GetValue(item));
                    }
                    values.Append(j < properties.Count() - 1 ? "," : ")");
                }

                if (i < list.Count-1)
                    values.Append(",");
            }
            return
                $"INSERT INTO {tableName} ({string.Join(",", propertiesDictionary.Select(x => x.Key))}) VALUES {values}";
        }
    }
}
