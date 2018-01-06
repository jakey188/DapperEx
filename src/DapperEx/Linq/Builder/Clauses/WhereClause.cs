using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DapperEx.Linq.Builder;
using DapperEx.Linq.Helpers;
using DapperEx.Linq.Types;

namespace Dapper.Linq.Builder.Clauses
{
    internal class WhereExpressionVisitor<T> : ExpressionVisitor
    {
        private bool _notOperater;
        private SqlBuilder<T> _builder;

        public void Evaluate(Expression node = null, SqlBuilder<T> builder = null, string predicate = "")
        {
            _builder = builder;

            _builder.Where.Append(string.IsNullOrEmpty(_builder.Where.ToString()) ? " WHERE " : " AND ");
            if (!string.IsNullOrEmpty(predicate))
                _builder.Where.Append(predicate);
            if (node != null)
                base.Visit(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
                _notOperater = true;

            //if (node.Operand is LambdaExpression) return node;
            if (!(node.Operand is MemberExpression))
                return base.VisitUnary(node);

            Visit(node.Operand);
            if (Helper.IsBoolean(node.Operand.Type) && !Helper.IsHasValue(node.Operand))
                Boolean(!Helper.IsPredicate(node));

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var table = CacheHelper.GetTableInfo(node.Expression.Type);

            if (node.Expression!=null && Helper.IsSpecificMemberExpression(node, node.Expression.Type,table.Columns))
            {
                var propertyName = Helper.GetPropertyNameWithIdentifierFromExpression(node);
                var columnName = _builder.Adapter.Field(_builder.Table, _builder.TableAliasName, propertyName);
                ColumnName(columnName);

                return node;
            }
            else if (Helper.IsVariable(node))
            {
                Parameter(Helper.GetValueFromExpression(node));
                return node;
            }
            else if (Helper.IsHasValue(node))
            {
                var me = base.VisitMember(node);
                IsNull();
                return me;
            }
            return base.VisitMember(node);
            ;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value as ConstantExpression;
            var val = Helper.GetValueFromExpression(value ?? node);

            Parameter(val);

            return base.VisitConstant(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var op = Helper.GetOperator(node);
            Expression left = node.Left;
            Expression right = node.Right;

            OpenBrace();

            if (Helper.IsBoolean(left.Type))
            {
                Visit(left);
                WhiteSpace();
                Write(op);
                WhiteSpace();
                Visit(right);
            }
            else
            {
                VisitValue(left);
                WhiteSpace();
                Write(op);
                WhiteSpace();
                VisitValue(right);
            }

            CloseBrace();

            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case MethodCall.EndsWith:
                case MethodCall.StartsWith:
                case MethodCall.Contains:
                    return LikeInMethod(node);
                case MethodCall.IsNullOrEmpty:
                    // ISNULL(x, '') (!)= ''
                    if (IsNullMethod(node))
                        return node;
                    break;
               
            }
            return base.VisitMethodCall(node);
        }

        protected virtual Expression VisitValue(Expression expr)
        {
            return Visit(expr);
        }

        protected virtual Expression VisitPredicate(Expression expr)
        {
            if (!Helper.IsPredicate(expr) && !Helper.IsHasValue(expr))
            {
                Boolean(true);
            }
            return expr;
        }

        private bool IsNullMethod(MethodCallExpression node)
        {
            if (!Helper.IsSpecificMemberExpression(node.Arguments[0],typeof(T), CacheHelper.GetTableInfo(typeof(T)).Columns))
                return false;

            IsNullFunction();
            OpenBrace();
            Visit(node.Arguments[0]);
            Delimiter();
            WhiteSpace();
            EmptyString();
            CloseBrace();
            WhiteSpace();
            Operator();
            WhiteSpace();
            EmptyString();
            return true;
        }

        private Expression LikeInMethod(MethodCallExpression node)
        {
            var type = typeof (T);
            var tableInfo = CacheHelper.GetTableInfo(type);
            if (node.Method.DeclaringType == typeof(string))
            {
                // LIKE '..'
                if (!Helper.IsSpecificMemberExpression(node.Object,type,tableInfo.Columns))
                    return node;

                Visit(node.Object);
                Like();
                if (node.Method.Name == MethodCall.EndsWith || node.Method.Name == MethodCall.Contains)
                    LikePrefix();
                Visit(node.Arguments[0]);
                if (node.Method.Name == MethodCall.StartsWith || node.Method.Name == MethodCall.Contains)
                    LikeSuffix();
                return node;
            }

            // IN (...)
            object ev;

            if (node.Method.DeclaringType == typeof(List<string>))
            {
                if (
                    !Helper.IsSpecificMemberExpression(node.Arguments[0],type,tableInfo.Columns))
                    return node;


                Visit(node.Arguments[0]);
                ev = Helper.GetValueFromExpression(node.Object);

            }
            else if (node.Method.DeclaringType == typeof(Enumerable))
            {
                if (!Helper.IsSpecificMemberExpression(node.Arguments[1],type,tableInfo.Columns))
                    return node;

                Visit(node.Arguments[1]);
                ev = Helper.GetValueFromExpression(node.Arguments[0]);

            }
            else
            {
                return node;
            }

            In();

            // Add each string in the collection to the list of locations to obtain data about. 
            var queryStrings = (IList<object>)ev;
            var count = queryStrings.Count();
            OpenBrace();
            for (var i = 0;i < count;i++)
            {
                Parameter(queryStrings.ElementAt(i));

                if (i + 1 < count)
                    Delimiter();
            }
            CloseBrace();

            return node;
        }

        #region Write Clause

        internal void In()
        {
            if (_notOperater)
                Write(" NOT");
            Write(" IN ");
            _notOperater = false;
        }

        internal void Like()
        {
            if (_notOperater)
                Write(" NOT");
            Write(" LIKE ");
            _notOperater = false;
        }

        internal void LikePrefix()
        {
            Write("'%' + ");
        }

        internal void LikeSuffix()
        {
            Write("+ '%'");
        }

        internal void EmptyString()
        {
            Write("''");
        }

        internal void Delimiter()
        {
            Write(", ");
        }

        internal void IsNullFunction()
        {
            Write("ISNULL");
        }

        private void Parameter(object val)
        {
            if (val == null)
            {
                Write("NULL");
                return;
            }

            var param = _builder.NextParamId();

            _builder.AddParameter(param, val);

            Write(param);
        }


        private void IsNull()
        {
            Write(" IS");
            if (!_notOperater)
                Write(" NOT");
            Write(" NULL");
            _notOperater = false;
        }

        private void ColumnName(string columnName)
        {
            Write(columnName);
        }

        private void Boolean(bool op)
        {
            Write((op ? " <> " : " = ") + "0");
        }

        private void Operator()
        {
            Write(Helper.GetOperator((_notOperater) ? ExpressionType.NotEqual : ExpressionType.Equal));
            _notOperater = false;
        }

        private void Write(object value)
        {
            _builder.Where.Append(value);
        }

        private void OpenBrace()
        {
            _builder.Where.Append("(");
        }

        private void CloseBrace()
        {
            _builder.Where.Append(")");
        }

        private void WhiteSpace()
        {
            _builder.Where.Append(" ");
        }

        #endregion
    }
}
