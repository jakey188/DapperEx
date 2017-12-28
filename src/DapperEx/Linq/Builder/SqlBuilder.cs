using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Dapper;
using DapperEx.Linq.Helpers;

namespace DapperEx.Linq.Builder
{
    internal class SqlBuilder<T>
    {
        private int _paramIndex;

        /// <summary>
        /// 表名
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 表别名
        /// </summary>
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

        /// <summary>
        /// 分组字段
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// SQL参数
        /// </summary>
        public DynamicParameters Parameters { get; set; }

        /// <summary>
        /// SQL生成适配器
        /// </summary>
        public DapperEx.Linq.Builder.ISqlAdapter Adapter { get; }

        /// <summary>
        /// 是否启用别名
        /// </summary>
        public bool IsEnableAlias { get; private set; }


        public SqlBuilder(DapperEx.Linq.Builder.ISqlAdapter adapter, bool isEnableAlias = true)
        {
            var table = CacheHelper.GetTableInfo(typeof(T));
            Where = new StringBuilder();
            Parameters = new DynamicParameters();
            SelectField = new List<string> { "*" };
            TableAliasName = isEnableAlias ? table.Alias : string.Empty;
            IsEnableAlias = isEnableAlias;
            Adapter = adapter;
            Table = isEnableAlias ? Adapter.Table(table.Name,table.Alias) : Adapter.Table(table.Name);
        }

        public string GetQueryString()
        {
            var selection = string.Join(",",SelectField);

            var order = Order.Count > 0 ? " ORDER BY " + string.Join(",",Order) : "";

            return Adapter.QueryString(Take,selection, Table, Where.ToString(),order, GroupBy, "");
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
