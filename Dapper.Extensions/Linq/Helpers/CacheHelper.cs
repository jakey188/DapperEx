using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.Linq.Helpers
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

    internal class CacheHelper
    {
        private static readonly ConcurrentDictionary<Type,TableInfo> TypeTableInfo = new ConcurrentDictionary<Type,TableInfo>();

        internal static int Size
        {
            get { return TypeTableInfo.Count; }
        }

        public static TableInfo GetTableInfo(Expression expression)
        {
            var exp = Helper.GetMemberExpression(expression);
            if (!(exp is MemberExpression))
                return null;

            return GetTableInfo(exp.Expression.Type);
        }

        public static TableInfo GetTableInfo(Type type)
        {
            var tableInfo = new TableInfo();
            if (!TypeTableInfo.TryGetValue(type,out tableInfo))
            {
                var properties = new Dictionary<string,string>();
                type.GetProperties().ToList().ForEach(
                        x =>
                        {
                            var col = (ColumnAttribute)x.GetCustomAttribute(typeof(ColumnAttribute));
                            properties.Add(x.Name,(col != null) ? col.Name : x.Name);
                        }
                    );

                var attrib = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));

                tableInfo = new TableInfo
                {
                    Name = (attrib != null ? attrib.Name : type.Name),
                    Columns = properties,
                    Alias = string.Format("t{0}",Size + 1)
                };
                TypeTableInfo.TryAdd(type,tableInfo);
            }
            return tableInfo;
        }

        public static string GetTableAlias(Expression expression)
        {
            return GetTableInfo(expression).Alias;
        }
    }
}
