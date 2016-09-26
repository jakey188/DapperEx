using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Linq
{
    internal class SqlAdapter
    {
        public string QueryString(int top,List<string> selectList,string source,string conditions,List<string> orderList,string grouping,string having)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            stopwatch.Start();

            var order = orderList.Count> 0 ? " ORDER BY " + string.Join(",",orderList) : "";
            var selection = string.Join(",", selectList);
            var sql = $"SELECT {(top == 0 ? "" : "TOP (" + top+")")} {selection} FROM {source} {conditions} {order} {grouping} {having}";

            Trace.WriteLine($"SQL打印:{sql}");
            stopwatch.Stop();

            return sql;
        }


        public string QueryStringPage(string source,string select,string conditions,string order, int pageSize,int pageNumber)
        {
            var innerQuery = $"SELECT {select},ROW_NUMBER() OVER ({order}) AS RN FROM {source} {conditions}";

            return $"SELECT TOP {pageSize} * FROM ({innerQuery}) InnerQuery WHERE RN > {pageSize * (pageNumber - 1)} ORDER BY RN";
        }


        public string Table(string tableName,string tableAliasName)
        {
            return $"[{tableName}] AS [{tableAliasName}]";
        }


        public string Field(string tableAliasName,string fieldName,string aliasName="")
        {
            return $"[{tableAliasName}].[{fieldName}]{(string.IsNullOrEmpty(aliasName) ? "" : " AS ["+aliasName+"]")}";
        }

        public string Parameter(string parameterId)
        {
            return "@" + parameterId;
        }
    }
}
