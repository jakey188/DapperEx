using System.Collections.Generic;

namespace DapperEx.Linq.Types
{
    public class TableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public Dictionary<string,string> Columns { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
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
        public string SelectFiledAliasName { get; set; }
    }
}
