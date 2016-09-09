using System.Linq.Expressions;

namespace Dapper.Linq
{
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
        public string FieldName { get; set; }
        public string FiledAliasName { get; set; }
    }
}
