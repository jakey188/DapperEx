using System.Diagnostics;

namespace DapperEx.Linq.Builder
{
    public interface ISqlAdapter
    {
        string QueryString(int top, string select, string source, string conditions, string order, string grouping, string having);
        string QueryStringPage(string source, string select, string conditions, string order, int pageSize, int pageNumber);
        string Table(string tableName, string tableAliasName = "");
        string Field(string tableName, string tableAliasName, string fieldName, string selectFieldAliasName = "");
    }
    internal class SqliteAdapter : SqlAdapter, ISqlAdapter
    {
        public string QueryStringPage(string source, string select, string conditions, string order, int pageSize, int pageNumber)
        {
            return $"SELECT {select} FROM {source} {conditions} {order} LIMIT  {pageSize} OFFSET {pageSize * (pageNumber - 1)}";
        }

        public string QueryString(int top, string select, string source, string conditions, string order, string grouping, string having)
        {
            return $"SELECT {select} FROM {source} {conditions} {order} {(top == 0 ? "" : " LIMIT " + top + "")}";
        }
    }
    internal class MySqlAdapter : SqlAdapter, ISqlAdapter
    {
        public string QueryStringPage(string source, string select, string conditions, string order, int pageSize, int pageNumber)
        {
            return $"SELECT {select} FROM {source} {conditions} {order} LIMIT  {pageSize} OFFSET {pageSize * (pageNumber - 1)}";
        }

        public string QueryString(int top, string select, string source, string conditions, string order, string grouping, string having)
        {
            return $"SELECT {select} FROM {source} {conditions} {order} {(top == 0 ? "" : " LIMIT " + top + "")}";
        }

        public override string Table(string tableName, string tableAliasName = "")
        {
            if (string.IsNullOrEmpty(tableAliasName))
                return $"`{tableName}`";
            return $"`{tableName}` AS `{tableAliasName}`";
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">表别名</param>
        /// <param name="tableAliasName">表别名</param>
        /// <param name="selectFieldAliasName">select字段</param>
        /// <returns></returns>
        public override string Field(string tableName, string tableAliasName, string fieldName, string selectFieldAliasName = "")
        {
            if (string.IsNullOrEmpty(tableAliasName))
                return $"`{fieldName}`";
            return $"`{tableAliasName}`.`{fieldName}`{(string.IsNullOrEmpty(selectFieldAliasName) ? "" : " AS `" + selectFieldAliasName + "`")}";
        }
    }
    internal class SqlServerAdapter : SqlAdapter, ISqlAdapter
    {
        public string QueryStringPage(string source, string select, string conditions, string order, int pageSize, int pageNumber)
        {
            var innerQuery = $"SELECT {select},ROW_NUMBER() OVER ({order}) AS RN FROM {source} {conditions}";

            return $"SELECT TOP {pageSize} * FROM ({innerQuery}) InnerQuery WHERE RN > {pageSize * (pageNumber - 1)} ORDER BY RN";
        }

        public string QueryString(int top, string select, string source, string conditions, string order, string grouping, string having)
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

    internal class SqlAdapter
    {
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableAliasName">表别名</param>
        /// <returns></returns>
        public virtual string Table(string tableName, string tableAliasName = "")
        {
            if (string.IsNullOrEmpty(tableAliasName))
                return $"[{tableName}]";
            return $"[{tableName}] AS [{tableAliasName}]";
        }

        /// <summary>
        /// 设置列
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">表别名</param>
        /// <param name="tableAliasName">表别名</param>
        /// <param name="selectFieldAliasName">select字段</param>
        /// <returns></returns>
        public virtual string Field(string tableName, string tableAliasName, string fieldName, string selectFieldAliasName = "")
        {
            if (string.IsNullOrEmpty(tableAliasName))
                return $"[{fieldName}]";
            return $"[{tableAliasName}].[{fieldName}]{(string.IsNullOrEmpty(selectFieldAliasName) ? "" : " AS [" + selectFieldAliasName + "]")}";
        }
    }
}
