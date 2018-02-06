using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dapper
{
    public class DbCommandInterceptor
    {
        Stopwatch _sw;
        StringBuilder _builder = new StringBuilder();
        public DbCommandInterceptor(IDbCommand cmd)
        {
            #if DEBUG
            _sw = new Stopwatch();
            _sw.Restart();
            if (cmd.Parameters != null)
            {
                _builder.AppendLine("参数值：");
                foreach (var item in cmd.Parameters)
                {
                    if (item != null)
                    {
                        var parameter = item as DbParameter;
                        _builder.AppendLine($"{parameter.ParameterName}={parameter.Value}");
                    }
                }
            }
            _builder.AppendLine($"{cmd?.CommandText}");
        #endif
        }

        public void Complete()
        {
            #if DEBUG
            _sw.Stop();
            _builder.Insert(0, $"[Sql Execute Time {_sw.ElapsedMilliseconds} Ms] \r\n");
            Debug.WriteLine(_builder.ToString());
            #endif
        }
    }

    public static class DbCommandExtensions
    {

        public static int ExecuteNonQueryInterceptor(this IDbCommand cmd)
        {
            var interceptor = new DbCommandInterceptor(cmd);
            var result = cmd.ExecuteNonQuery();
            interceptor.Complete();
            return result;
        }


        public static async Task<int> ExecuteNonQueryAsyncInterceptor(this DbCommand cmd, CancellationToken cancellationToken)
        {
            var interceptor = new DbCommandInterceptor(cmd);
            var result = await cmd.ExecuteNonQueryAsync(cancellationToken);
            interceptor.Complete();
            return result;
        }

        public static IDataReader ExecuteReaderInterceptor(this IDbCommand cmd, CommandBehavior behavior)
        {
            var interceptor = new DbCommandInterceptor(cmd);
            var result = cmd.ExecuteReader(behavior);
            interceptor.Complete();
            return result;
        }

        public static async Task<DbDataReader> ExecuteReaderAsyncInterceptor(this DbCommand cmd, CommandBehavior behavior, CancellationToken cancellationToken)
        {
            var interceptor = new DbCommandInterceptor(cmd);
            var result = await cmd.ExecuteReaderAsync(behavior, cancellationToken);
            interceptor.Complete();
            return result;
        }

        public static object ExecuteScalarInterceptor(this IDbCommand cmd)
        {
            var interceptor = new DbCommandInterceptor(cmd);
            var result = cmd.ExecuteScalar();
            interceptor.Complete();
            return result;
        }

        public static async Task<object> ExecuteScalarAsyncInterceptor(this DbCommand cmd, CancellationToken cancellationToken)
        {
            var interceptor = new DbCommandInterceptor(cmd);
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            interceptor.Complete();
            return result;
        }
    }
}
