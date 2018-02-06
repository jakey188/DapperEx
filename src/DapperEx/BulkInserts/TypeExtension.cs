using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperEx.BulkInserts
{
    public static class TypeExtension
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<PropertyInfo>> CustomProperties = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();
        static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo[]> TypeInfo = new ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo[]>();
        static readonly ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo[]> StringTypeInfo = new ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo[]>();
        /// <summary>
        /// 获取实体对象信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertiesFromCache(this Type type)
        {
            PropertyInfo[] propertyInfo;
            if (!TypeInfo.TryGetValue(type.TypeHandle, out propertyInfo))
            {
                propertyInfo = type.GetProperties();
                TypeInfo.TryAdd(type.TypeHandle, propertyInfo);
            }
            return propertyInfo;
        }
        public static IEnumerable<PropertyInfo> CustomPropertiesCache(this PropertyInfo[] propertys,Type type, string attribute)
        {
            IEnumerable<PropertyInfo> pi;
            var cacheTag = type.TypeHandle.Value + attribute;
            if (CustomProperties.TryGetValue(cacheTag, out pi))
            {
                return pi;
            }

            //var col = item.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "KeyAttribute").SingleOrDefault() as dynamic;
            //var foreignCol = item.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "ForeignKeyAttribute").SingleOrDefault() as dynamic;

            var customProperties = propertys.Where(p => p.GetCustomAttributes(false).Any(a => a.GetType().Name == attribute)).ToList();

            CustomProperties[cacheTag] = customProperties;
            return customProperties;
        }
    }
}
