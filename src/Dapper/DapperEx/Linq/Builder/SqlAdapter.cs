using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Linq
{
    public interface ISqlAdapter
    {
        string QueryString(int top,string select,string source,string conditions,string order,string grouping,string having);
        string QueryStringPage(string source,string select,string conditions,string order,int pageSize,int pageNumber);
        string Table(string tableName,string tableAliasName = "");

        string Field(string tableName,string fieldName,string fieldAliasName = "");
    }
    internal class SQLiteAdapter:SqlAdapter, ISqlAdapter
    {
        public string QueryStringPage(string source,string select,string conditions,string order,int pageSize,int pageNumber)
        {
            return $"SELECT {select} FROM {source} {conditions} {order} LIMIT  {pageSize} OFFSET {pageSize * (pageNumber - 1)}";
        }

        public string QueryString(int top,string select,string source,string conditions,string order,string grouping,string having)
        {
            return $"SELECT {select} FROM {source} {conditions} {order} {(top == 0 ? "" : " LIMIT " + top + "")}";
        }
    }

    internal  class SqlServerAdapter :SqlAdapter,ISqlAdapter
    {
        public string QueryStringPage(string source,string select,string conditions,string order,int pageSize,int pageNumber)
        {
            var innerQuery = $"SELECT {select},ROW_NUMBER() OVER ({order}) AS RN FROM {source} {conditions}";

            return $"SELECT TOP {pageSize} * FROM ({innerQuery}) InnerQuery WHERE RN > {pageSize * (pageNumber - 1)} ORDER BY RN";
        }

        public string QueryString(int top,string select,string source,string conditions,string order,string grouping,string having)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            stopwatch.Start();

            var sql = $"SELECT {(top == 0 ? "" : "TOP (" + top + ")")} {select} FROM {source} {conditions} {order} {grouping} {having}";

            Trace.WriteLine($"SQL打印:{sql}");
            stopwatch.Stop();

            return sql;
        }

    }

    internal  class SqlAdapter
    {
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableAliasName">表别名</param>
        /// <returns></returns>
        public string Table(string tableName,string tableAliasName="")
        {
            if(string.IsNullOrEmpty(tableAliasName))
                return $"[{tableName}]";
            return $"[{tableName}] AS [{tableAliasName}]";
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableAliasName">表别名</param>
        /// <param name="fieldAliasName">字段别名</param>
        /// <returns></returns>
        public string Field(string tableName,string fieldName,string fieldAliasName = "")
        {
            return $"[{tableName}].[{fieldName}]{(string.IsNullOrEmpty(fieldAliasName) ? "" : " AS [" + fieldAliasName + "]")}";
        }

    }
}
