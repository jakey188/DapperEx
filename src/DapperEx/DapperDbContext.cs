using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;
using Dapper.Linq.Builder.Clauses;
using DapperEx.Linq.Builder;

namespace DapperEx
{
    public abstract class DapperDbContext : IDisposable
    {
        public DbProviderFactory DbProviderFactory { get; set; }

        private IDbConnection _dbConnetion;

        public IDbConnection Connection
        {
            get
            {
                if (_dbConnetion != null)
                {
                    if (_dbConnetion.State != ConnectionState.Open && _dbConnetion.State != ConnectionState.Connecting)
                        _dbConnetion.Open();
                }
                return _dbConnetion;
            }
        }

        /// <summary>
        /// 数据库事物对象
        /// </summary>
        public IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 数据库类型适配器
        /// </summary>
        public Linq.Builder.ISqlAdapter Adapter { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public EnmDbType DbType { get; set; }

        /// <summary>
        /// 创建DbConnection
        /// </summary>
        /// <param name="connection"></param>
        public void CreateDbConnection(IDbConnection connection)
        {
            _dbConnetion = connection;
        }

        /// <summary>
        /// 开始事务操作
        /// </summary>
        public void BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// 提交数据库事务
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
            }
        }

        /// <summary>
        /// 数据库事务回滚
        /// </summary>
        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
        }

        /// <summary>
        /// 设置数据库适配
        /// </summary>
        /// <param name="dbType"></param>
        public void SetAdapter(EnmDbType dbType)
        {
            DbType = dbType;

            switch (dbType)
            {
                case EnmDbType.SqlServer:
                    Adapter = new DapperEx.Linq.Builder.SqlServerAdapter();
                    break;
                case EnmDbType.Sqlite:
                    Adapter = new DapperEx.Linq.Builder.SqliteAdapter();
                    break;
                case EnmDbType.MySql:
                    Adapter = new DapperEx.Linq.Builder.MySqlAdapter();
                    break;
            }

        }

        /// <summary>
        /// 插入方法
        /// </summary>
        /// <param name="t">当前T</param>
        /// <returns></returns>
        public virtual long Add<T>(T t) where T : class
        {
            return Connection.Insert(t, Transaction);
        }

        /// <summary>
        /// 少批量添加，如需保证数据一致性,请使用事务
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <remarks>建议100条以下</remarks>
        public virtual long AddRange<T>(List<T> list) where T : class
        {
            int row = 0;
            foreach (var item in list)
            {
                var key = Connection.Insert(item, Transaction);
                if (key > 0)
                    row++;
            }
            return row;
        }

        /// <summary>
        /// 单条删除方法
        /// </summary>
        /// <param name="t">当前T</param>
        /// <returns></returns>
        public virtual bool Delete<T>(T t) where T : class
        {
            return Connection.Delete(t, Transaction);
        }

        /// <summary>
        /// 单表清空方法
        /// </summary>
        /// <typeparam name="T">当前T</typeparam>
        /// <returns></returns>
        public virtual bool DeleteAll<T>() where T : class
        {
            return Connection.DeleteAll<T>(Transaction);
        }

        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="t">当前T</param>
        /// <returns>返回更新的</returns>
        public virtual bool Update<T>(T t) where T : class
        {
            return Connection.Update(t);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">删除条件</param>
        /// <returns></returns>
        public virtual int Delete<T>(Expression<Func<T, bool>> expression) where T : class
        {
            if (expression == null)
                return 0;
            var builder = new SqlBuilder<T>(Adapter, false);
            var resolve = new WhereExpressionVisitor<T>();
            resolve.Evaluate(expression, builder);
            string sql = $"DELETE FROM {builder.Table} {builder.Where}";
            return Connection.Execute(sql, builder.Parameters, Transaction);
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereExpression">修改条件</param>
        /// <param name="updateExpression">修改字段</param>
        /// <returns></returns>
        public virtual int Update<T>(Expression<Func<T, bool>> whereExpression, Expression<Func<T, T>> updateExpression)
            where T : class
        {
            if (whereExpression == null)
                return 0;
            var builder = new SqlBuilder<T>(Adapter,false);
            var resolve = new WhereExpressionVisitor<T>();
            resolve.Evaluate(whereExpression, builder);

            string set = string.Empty;
            var expression = (MemberInitExpression)updateExpression.Body;
            int i = 0;
            int bindingCount = expression.Bindings.Count;
            foreach (var binding in expression.Bindings)
            {
                i++;
                var name = binding.Member.Name;
                object value;
                var memberExpression = ((MemberAssignment)binding).Expression;

                var constantExpression = memberExpression as ConstantExpression;
                if (constantExpression != null)
                {
                    value = constantExpression.Value;
                }
                else
                {
                    var lambda = Expression.Lambda(memberExpression, null);
                    value = lambda.Compile().DynamicInvoke();
                }
                var column = builder.Adapter.Field(builder.Table, builder.TableAliasName, name);

                set += $"{column}=@{name}";
                if (i < bindingCount)
                    set += ", ";
                builder.Parameters.Add(name, value);
            }
            var table = $"{(builder.IsEnableAlias ? builder.TableAliasName : builder.Table)}";
            string sql = $"UPDATE {table} SET {set} {builder.Where}";

            return Connection.Execute(sql, builder.Parameters, Transaction);
        }

        /// <summary>
        /// 增删改SQL执行方法
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandType">执行方式</param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string sql, object param, CommandType? commandType = null)
        {
            return Connection.Execute(sql: sql, param: param, transaction: Transaction, commandTimeout: null,
                commandType: commandType);
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public virtual List<T> SqlQuery<T>(string sql, object param = null, CommandType? commandType = null)
        {
            return Connection.Query<T>(sql: sql, param: param, transaction: Transaction, buffered: true,
                commandTimeout: null, commandType: commandType).AsList();
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public virtual IEnumerable<dynamic> SqlQueryDynamic(string sql, object param = null,
            CommandType? commandType = null)
        {
            return Connection.Query(sql: sql, param: param, transaction: Transaction, buffered: true,
                commandTimeout: null, commandType: commandType);
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="where">查询条件</param>
        /// <param name="select">返回字段</param>
        /// <param name="order">排序条件</param>
        /// <param name="parametes">查询条件参数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public virtual List<T> SqlPageQuery<T>(string table, string where, string select, string order, object parametes,
            int pageIndex, int pageSize, out int total)
        {
            total = 0;
            if (pageIndex < 1)
                pageIndex = 1;

            var sql = Adapter.QueryStringPage(table, select, where, order, pageSize, pageIndex);

            var data = Connection.Query<T>(sql: sql, param: parametes, transaction: Transaction).AsList();

            if (data != null && data.Count > 0 && pageSize > 0)
            {
                total = Connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {table} {where}", parametes);
            }
            else
            {
                total = data.Count;
            }

            return data;
        }


        public void Dispose()
        {
            if (_dbConnetion != null && _dbConnetion.State != ConnectionState.Closed)
            {
                Transaction?.Dispose();
                _dbConnetion.Close();
                Debug.WriteLine("Connetion " + _dbConnetion.State);
                _dbConnetion = null;
            }
        }
    }
}
