using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using Dapper.Linq;
using DapperEx.Linq.Helpers;
using DapperEx.Linq.Types;

namespace DapperEx.Linq.Builder
{
    internal class QueryBuilder<T>
    {
        private string ParameterPrefix = "p";
        private int _paramIndex;
        private StringBuilder nodeBuilder;
        /// <summary>
        /// SQL表名
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// SQL条件
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// SQL查询字段
        /// </summary>
        public List<string> SelectField { get; set; } = new List<string>();
        /// <summary>
        /// 查询条数
        /// </summary>
        public int Take { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public List<string> Order { get; set; } = new List<string>();
        /// <summary>
        /// SQL参数
        /// </summary>
        public DynamicParameters Parameters { get;  set; }
        /// <summary>
        /// SQL生成适配器
        /// </summary>
        public SqlAdapter Adapter;

        public QueryBuilder()
        {
            var table = CacheHelper.GetTableInfo(typeof(T));
            Table = table.Name;
            Adapter = new SqlAdapter();
            Parameters = new DynamicParameters();
            //SelectField = new List<string>() { Select(table.Name };
        }

        public string GetQueryString()
        {
            return Adapter.QueryString(Take,string.Join(",",SelectField),Table,Where,"","","");
        }

        public string GetQueryPageString()
        {
            string sql = "";
            return sql;
        }

        /// <summary>
        /// 解析Lambda表达式节点Node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string ResolverNode(Node node)
        {
            nodeBuilder = new StringBuilder();
            BuildSql(node);
            return nodeBuilder.ToString();
        }

        public List<string> ResolverNode(List<MemberNode> list)
        {
            return list.Select(x => x.FieldName).ToList();
        }

        #region 私有方法

        #region BuildSql
        private void BuildSql(Node node)
        {
            BuildSql((dynamic)node);
        }

        private void BuildSql(LikeNode node)
        {
            if (node.Method == LikeMethod.Equals)
            {
                QueryByField(node.MemberNode.TableName,node.MemberNode.FieldName,
                    Dapper.Linq.Helpers.Helper.GetOperator(ExpressionType.Equal),node.Value);
            }
            else
            {
                string value = node.Value;
                switch (node.Method)
                {
                    case LikeMethod.StartsWith:
                        value = node.Value + "%";
                        break;
                    case LikeMethod.EndsWith:
                        value = "%" + node.Value;
                        break;
                    case LikeMethod.Contains:
                        value = "%" + node.Value + "%";
                        break;
                }
                QueryByFieldLike(node.MemberNode.TableName,node.MemberNode.FieldName,value);
            }
        }

        private void BuildSql(OperationNode node)
        {
            BuildSql((dynamic)node.Left,(dynamic)node.Right,node.Operator);
        }

        private void BuildSql(MemberNode memberNode)
        {
            QueryByField(memberNode.TableName,memberNode.FieldName,Dapper.Linq.Helpers.Helper.GetOperator(ExpressionType.Equal),true);
        }

        private void BuildSql(SingleOperationNode node)
        {
            if (node.Operator == ExpressionType.Not)
                Not();
            BuildSql(node.Child);
        }

        private void BuildSql(MemberNode memberNode,ValueNode valueNode,ExpressionType op)
        {
            if (valueNode.Value == null)
            {
                ResolveNullValue(memberNode,op);
            }
            else
            {
                QueryByField(memberNode.TableName,memberNode.FieldName,Dapper.Linq.Helpers.Helper.GetOperator(op),valueNode.Value);
            }
        }

        private void BuildSql(ValueNode valueNode,MemberNode memberNode,ExpressionType op)
        {
            BuildSql(memberNode,valueNode,op);
        }

        private void BuildSql(MemberNode leftMember,MemberNode rightMember,ExpressionType op)
        {
            QueryByFieldComparison(leftMember.TableName,leftMember.FieldName,Dapper.Linq.Helpers.Helper.GetOperator(op),rightMember.TableName,rightMember.FieldName);
        }

        private void BuildSql(SingleOperationNode leftMember,Node rightMember,ExpressionType op)
        {
            if (leftMember.Operator == ExpressionType.Not)
                BuildSql(leftMember as Node,rightMember,op);
            else
                BuildSql((dynamic)leftMember.Child,(dynamic)rightMember,op);
        }

        private void BuildSql(Node leftMember,SingleOperationNode rightMember,ExpressionType op)
        {
            BuildSql(rightMember,leftMember,op);
        }

        private void BuildSql(Node leftNode,Node rightNode,ExpressionType op)
        {
            BeginExpression();
            BuildSql((dynamic)leftNode);
            ResolveOperation(op);
            BuildSql((dynamic)rightNode);
            EndExpression();
        }

        private void ResolveNullValue(MemberNode memberNode,ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.Equal:
                    QueryByFieldNull(memberNode.TableName,memberNode.FieldName);
                    break;
                case ExpressionType.NotEqual:
                    QueryByFieldNotNull(memberNode.TableName,memberNode.FieldName);
                    break;
            }
        }
        #endregion

        #region CreateNodeBuilder
        private void BeginExpression()
        {
            nodeBuilder.Append("(");
        }

        private void EndExpression()
        {
            nodeBuilder.Append(")");
        }

        private void And()
        {
            nodeBuilder.Append(" AND ");
        }

        private void Or()
        {
            nodeBuilder.Append(" OR ");
        }

        private void Not()
        {
            nodeBuilder.Append(" NOT ");
        }

        private void ResolveOperation(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    And();
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    Or();
                    break;
                default:
                    throw new ArgumentException(string.Format("Unrecognized binary expression operation '{0}'",op.ToString()));
            }
        }

        public void OrderBy(string tableName,string fieldName,bool desc = false)
        {
            var order = Adapter.Field(tableName,fieldName);
            if (desc)
                order += " DESC";

            Order.Add(order);
        }

        public void Select(string tableName)
        {
            var selectionString = $"{Adapter.Table(tableName)}.*";
            SelectField.Add(selectionString);
        }

        public void Select(string tableName,string fieldName)
        {
            SelectField.Add(Adapter.Field(tableName,fieldName));
        }

        private void QueryByField(string tableName,string fieldName,string op,object fieldValue)
        {
            var paramId = NextParamId();
            nodeBuilder.Append(Adapter.Field(tableName,fieldName) + op + Adapter.Parameter(paramId));
            AddParameter(paramId,fieldValue);
        }

        private void QueryByFieldLike(string tableName,string fieldName,string fieldValue)
        {
            var paramId = NextParamId();
            nodeBuilder.Append(Adapter.Field(tableName,fieldName) + " LIKE " + Adapter.Parameter(paramId));
            AddParameter(paramId,fieldValue);
        }

        private void QueryByFieldNull(string tableName,string fieldName)
        {
            nodeBuilder.Append(Adapter.Field(tableName,fieldName) + " IS NULL ");
        }

        private void QueryByFieldNotNull(string tableName,string fieldName)
        {
            nodeBuilder.Append(Adapter.Field(tableName,fieldName) + " IS NOT NULL ");
        }

        private void QueryByFieldComparison(string leftTableName,string leftFieldName,string op,
            string rightTableName,string rightFieldName)
        {
            nodeBuilder.Append(Adapter.Field(leftTableName,leftFieldName) + op + Adapter.Field(rightTableName,rightFieldName));
        }
        #endregion

        private string NextParamId()
        {
            ++_paramIndex;
            return ParameterPrefix + _paramIndex.ToString(CultureInfo.InvariantCulture);
        }

        private void AddParameter(string key,object value)
        {
             Parameters.Add(key,value);
        } 
        #endregion
    }
}
