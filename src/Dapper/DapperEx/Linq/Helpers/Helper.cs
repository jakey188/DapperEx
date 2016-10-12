using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.Linq.Helpers
{
    public class Helper
    {
        internal static bool IsEqualsExpression(Expression exp)
        {
            return exp.NodeType == ExpressionType.Equal || exp.NodeType == ExpressionType.NotEqual;
        }


        internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, Dictionary<string, string> propertyList)
        {
            if (propertyList == null) return false;
            return ((exp is MemberExpression) &&
                    (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                    propertyList[(((MemberExpression)exp).Member.Name)] != null);
        }

        internal static object GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType)
        {
            if (!IsEqualsExpression(be))
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                var me = (MemberExpression)be.Left;

                if (me.Member.DeclaringType == memberDeclaringType)
                {
                    return GetValueFromExpression(be.Right);
                }
            }
            else if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                var me = (MemberExpression)be.Right;

                if (me.Member.DeclaringType == memberDeclaringType)
                {
                    return GetValueFromExpression(be.Left);
                }
            }

            // We should have returned by now. 
            throw new Exception("There is a bug in this program.");
        }

        internal static string GetPropertyNameFromEqualsExpression(BinaryExpression be, Type memberDeclaringType)
        {
            if (!IsEqualsExpression(be))
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                return GetPropertyNameFromExpression(be.Left);
            }
            if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                return GetPropertyNameFromExpression(be.Right);
            }

            // We should have returned by now. 
            throw new Exception("There is a bug in this program.");
        }

        internal static string GetPropertyNameWithIdentifierFromExpression(Expression expression)
        {
            var exp = GetMemberExpression(expression);
            if (!(exp is MemberExpression)) return string.Empty;

            var table = CacheHelper.GetTableInfo(((MemberExpression)exp).Expression.Type);
            var member = ((MemberExpression)exp).Member;

            return table.Columns[member.Name];
        }

        /// <summary>
        /// 获取表字段
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string GetPropertyNameFromExpression(Expression expression)
        {
            var exp = GetMemberExpression(expression);
            if (!(exp is MemberExpression)) return string.Empty;

            var member = ((MemberExpression)exp).Member;
            var columns = CacheHelper.GetTableInfo(((MemberExpression)exp).Expression.Type).Columns;
            return columns[member.Name];
        }


        internal static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is UnaryExpression)
                return GetMemberExpression((((UnaryExpression)expression).Operand));
            if (expression is LambdaExpression)
                return GetMemberExpression((((LambdaExpression)expression).Body));
            if (expression is MemberExpression)
                return expression as MemberExpression;
            return null;
        }

        internal static BinaryExpression GetBinaryExpression(Expression expression)
        {
            if (expression is BinaryExpression)
                return expression as BinaryExpression;

            throw new ArgumentException("Binary expression expected");
        }



        internal static object GetValueFromExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        internal static string GetOperator(string methodName)
        {
            switch (methodName)
            {
                case "Add": return "+";
                case "Subtract": return "-";
                case "Multiply": return "*";
                case "Divide": return "/";
                case "Negate": return "-";
                case "Remainder": return "%";
                default: return null;
            }
        }

        internal static string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return "-";
                case ExpressionType.UnaryPlus:
                    return "+";
                case ExpressionType.Not:
                    return IsBoolean(u.Operand.Type) ? "NOT" : "~";
                default:
                    return "";
            }
        }

        internal static string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(b.Left.Type)) ? "AND" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(b.Left.Type) ? "OR" : "|");
                default:
                    return GetOperator(b.NodeType);
            }
        }

        internal static string GetOperator(ExpressionType exprType)
        {
            switch (exprType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.RightShift:
                    return ">>";
                default:
                    return "";
            }
        }

        internal static bool IsHasValue(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Member.Name == "HasValue");
        }

        internal static bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        internal static bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(expr.Type);
                case ExpressionType.Not:
                    return IsBoolean(expr.Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(expr.Type);
                default:
                    return false;
            }
        }

        internal static bool IsVariable(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Expression is ConstantExpression);
        }


    }
}
