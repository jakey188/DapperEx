using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Linq.Helpers;

namespace Dapper.Linq.Builder.Visitors
{
    internal class ExpressionResolve
    {
        public List<MemberNode> VisitMember(Expression expression)
        {
            var list = new List<MemberNode>();
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    var name = GetMemberInfo(Helper.GetMemberExpression(expression));
                    list.Add(name);
                    break;
                case ExpressionType.New:
                    var newExpressions = (NewExpression)expression;
                    var nameArr = newExpressions.Members.Select(x => x.Name).ToArray();
                    int i = 0;
                    foreach (MemberExpression memberExp in newExpressions.Arguments)
                    {
                        var node = GetMemberInfo(memberExp);
                        if (!node.FieldName.Equals(nameArr[i]))
                            node.FiledAliasName = nameArr[i];
                        list.Add(node);
                        i++;
                    }
                    break;
                case ExpressionType.MemberInit:
                    var memberInitExpression = (MemberInitExpression)expression;
                    foreach (var item in memberInitExpression.Bindings)
                    {
                        var memberExpression = ((MemberAssignment)item).Expression;
                        var node = GetMemberInfo(Helper.GetMemberExpression(memberExpression));
                        var model = new MemberNode
                        {
                            FieldName = node.FieldName,
                            FiledAliasName = item.Member.Name == node.FieldName ? "" : item.Member.Name,
                            TableName = node.TableName
                        };
                        list.Add(model);
                    }
                    break;
            }
            return list;
        }

        private MemberNode GetMemberInfo(MemberExpression memberExpression)
        {
            var member = CacheHelper.GetTableInfo(memberExpression);
            return new MemberNode()
            {
                TableName = member.Alias,
                FieldName = Helper.GetPropertyNameFromExpression(memberExpression)
            };
        }
    }
}
