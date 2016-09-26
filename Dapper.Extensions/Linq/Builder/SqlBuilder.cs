using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper.Linq.Builder;
using Dapper.Linq.Helpers;

namespace Dapper.Linq
{
    internal class SqlBuilder<T>
    {
        private int _paramIndex;
        /// <summary>
        /// SQL表名
        /// </summary>
        public string Table { get; set; }
        public string TableAliasName { get; set; }
        /// <summary>
        /// SQL条件
        /// </summary>
        public StringBuilder Where { get; set; }
        /// <summary>
        /// SQL查询字段
        /// </summary>
        public List<string> SelectField { get; set; }
        /// <summary>
        /// 查询条数
        /// </summary>
        public int Take { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public List<string> Order { get; set; } = new List<string>();

        public string GroupBy { get; set; }
        /// <summary>
        /// SQL参数
        /// </summary>
        public DynamicParameters Parameters { get; set; }
        /// <summary>
        /// SQL生成适配器
        /// </summary>
        public SqlAdapter Adapter;

        public SqlBuilder()
        {
            var table = CacheHelper.GetTableInfo(typeof(T));
            Where = new StringBuilder();
            Adapter = new SqlAdapter();
            Parameters = new DynamicParameters();
            SelectField = new List<string> {"*"};
            TableAliasName = table.Alias;
            Table = Adapter.Table(table.Name,table.Alias);
        }

        public string GetQueryString()
        {
            return Adapter.QueryString(Take, SelectField, Table, Where.ToString(), Order, GroupBy, "");
        }

        public string GetQueryPageString(int pageIndex,int pageSize)
        {
            var selection = string.Join(",",SelectField);

            var order = Order.Count > 0 ? " ORDER BY " + string.Join(",",Order) : "";

            return Adapter.QueryStringPage(Table,selection, Where.ToString(),order, pageSize, pageIndex);
        }

        public string NextParamId()
        {
            ++_paramIndex;
            return "@p" + _paramIndex.ToString(CultureInfo.InvariantCulture);
        }

        public void AddParameter(string key,object value)
        {
            Parameters.Add(key,value);
        }
    }
}
