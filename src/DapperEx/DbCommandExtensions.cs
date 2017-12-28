using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            _sw = new Stopwatch();
            _sw.Restart();
            _builder.AppendLine($"{cmd?.CommandText}");
        }

        public void Complete()
        {
            _sw.Stop();
            _builder.Insert(0, $"[Sql Execute Time {_sw.ElapsedMilliseconds} Ms] ");
            Debug.WriteLine(_builder.ToString());
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
