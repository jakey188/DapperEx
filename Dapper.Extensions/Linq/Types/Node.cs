using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Linq
{
    internal class TableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        internal Dictionary<string,string> Columns { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        internal string Alias { get; set; }
    }

    public class MethodCall
    {
        internal const string EndsWith = "EndsWith";
        internal const string StartsWith = "StartsWith";
        internal const string Contains = "Contains";
        internal const string IsNullOrEmpty = "IsNullOrEmpty";
    }

    internal class MemberNode
    {
        public string TableName { get; set; }
        public string TableAliasName { get; set; }
        public string FieldName { get; set; }
        public string FiledAliasName { get; set; }
    }
}
