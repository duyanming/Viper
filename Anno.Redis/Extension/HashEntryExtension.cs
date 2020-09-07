using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using StackExchange.Redis;

namespace Anno.Redis
{
    public static class HashEntryExtension
    {
        /// <summary>
        /// 对象转换为HashEntity
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HashEntry[] ToHashEntries(this object obj)
        {
            List<PropertyInfo> properties = obj.GetType().GetProperties().Where(p => p.CanRead).ToList();
            return properties.Select(property => new HashEntry(property.Name, property.GetValue(obj).ToString()))
                .ToArray();
        }
        /// <summary>
        /// HashEntity转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashEntries"></param>
        /// <returns></returns>
        public static T ToObjFromRedis<T>(this HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.CanWrite).ToArray();
            var obj = Activator.CreateInstance<T>();
            foreach (var property in properties)
            {
                Type[] types = property.PropertyType.GenericTypeArguments;
                HashEntry hashEntry = hashEntries.FirstOrDefault(h => h.Name.ToString().Equals(property.Name, StringComparison.CurrentCultureIgnoreCase));
                if (hashEntry.Equals(default(HashEntry))) continue;
                if (types.Length > 0)
                {
                    var fullName = types[0].FullName;
                    if (fullName != null && fullName.IndexOf("System", StringComparison.Ordinal) != -1)
                    {
                        property.SetValue(obj, Convert.ChangeType(hashEntry.Value, types[0]), null);
                    }
                    else if (types[0].BaseType == typeof(Enum))
                    {
                        property.SetValue(obj, Enum.Parse(types[0], hashEntry.Value.ToString()), null);
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(hashEntry.Value, property.PropertyType),
                            null);
                    }
                }
                else
                {
                    if (property.PropertyType.BaseType == typeof(Enum))
                    {
                        property.SetValue(obj, Enum.Parse(property.PropertyType, hashEntry.Value),
                            null);
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(hashEntry.Value, property.PropertyType),
                            null);
                    }
                }
            }
            return obj;
        }
    }
}
