using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Linq.Helpers;

namespace Dapper.Demo
{
    public class TableInfo
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
    public class TimeTest
    {
        private static readonly ConcurrentDictionary<Type,TableInfo> TypeTableInfo = new ConcurrentDictionary<Type,TableInfo>();
        public TableInfo GetTableInfo(Type type)
        {
            for (var i = 0;i < 10;i++)
            {
               
                var sw = new Stopwatch();
                sw.Reset();
                sw.Start();
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
                        Alias = string.Format("t{0}",1)
                    };
                    if (i > 0)
                    {
                        //TypeTableInfo.TryAdd(type,tableInfo);
                    }
                    
                }
                sw.Stop();
                if (i > 0)
                {
                    Console.WriteLine("耗时" + sw.ElapsedMilliseconds + "毫秒");
                }
            }

            return null;
        }
    }
}
