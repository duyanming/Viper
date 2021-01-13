using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Anno.Common
{
    using System.Collections.Concurrent;
    public static class AnnoMapper
    {
        /// <summary>
        /// 类型缓存
        /// </summary>
        private static ConcurrentDictionary<Type, TypeValue> cacheType = new ConcurrentDictionary<Type, TypeValue>();
        /// <summary>
        /// DataTable 转化为 List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">表</param>
        /// <returns>对象集合</returns>
        public static IEnumerable<T> Mapper<T>(this DataTable table)
        {
            List<T> objList = new List<T>();
            if (table == null || table.Rows.Count <= 0)
            {
                yield return default;
            }
            var type = typeof(T);
            var typeValue = TryGetOrAdd(type);
            if (typeValue.CanWriteProperty.Count > 0 && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    T obj = (T)type.Assembly.CreateInstance(type.FullName);
                    foreach (DataColumn col in table.Columns)
                    {
                        try
                        {
                            var targetProp = typeValue.CanWriteProperty.Find(p =>
                                string.Equals(p.Name, col.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                            if (targetProp != null)
                            {
                                Type[] types = targetProp.PropertyType.GenericTypeArguments;
                                var value = row[col.ColumnName];
                                if (value != DBNull.Value)
                                {
                                    if (types.Length > 0)
                                    {
                                        if (types[0].FullName.IndexOf("System", StringComparison.Ordinal) != -1)
                                        {
                                            targetProp.SetValue(obj, Convert.ChangeType(value, types[0]), null);
                                        }
                                        else if (types[0].BaseType == typeof(Enum))
                                        {
                                            targetProp.SetValue(obj, Enum.Parse(types[0], value.ToString()), null);
                                        }
                                        else
                                        {
                                            targetProp.SetValue(obj, Convert.ChangeType(value, targetProp.PropertyType),
                                                null);
                                        }
                                    }
                                    else
                                    {
                                        if (targetProp.PropertyType.BaseType == typeof(Enum))
                                        {
                                            targetProp.SetValue(obj,
                                                Enum.Parse(targetProp.PropertyType, value.ToString()), null);
                                        }
                                        else
                                        {
                                            targetProp.SetValue(obj, value, null);
                                        }
                                    }
                                }
                                else
                                {
                                    targetProp.SetValue(obj, null, null);
                                }
                            }
                        }
                        catch
                        {
                            // 类型转换出错 直接忽略
                        }
                    }
                    yield return obj;
                }
            }
        }
        /// <summary>
        /// 获取字典信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> MapperToDictionary<T>(T source)
            where T : class
        {
            if (source == null)
            {
                return default;
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var typeValue = TryGetOrAdd(typeof(T));
            foreach (var p in typeValue.CanReadProperty)
            {
                if (!dic.Keys.Contains(p.Name))
                {
                    var value = p.GetValue(source);
                    dic.Add(p.Name, value);
                }
            }
            return dic;
        }

        /// <summary>
        /// 对象之间属性Copy
        /// </summary>
        /// <typeparam name="TTarget">目标</typeparam>
        /// <typeparam name="TSource">来源</typeparam>
        /// <param name="objSource">来源实例</param>
        /// <returns></returns>
        public static TTarget Mapp<TTarget, TSource>(TSource source)
            where TTarget : class
            where TSource : class
        {
            Type type = typeof(TTarget);
            if (source == null)
            {
                return default(TTarget);
            }
            TTarget target = (TTarget)type.Assembly.CreateInstance(type.FullName);
            var typeValue = TryGetOrAdd(type);
            if (typeValue.CanWriteProperty != null && typeValue.CanWriteProperty.Count > 0)
            {
                var originTypeValue = TryGetOrAdd(source.GetType());
                foreach (PropertyInfo targetProp in typeValue.CanWriteProperty)
                {
                    try
                    {
                        var op = originTypeValue.CanReadProperty.Find(p => string.Equals(p.Name, targetProp.Name, StringComparison.CurrentCultureIgnoreCase)); //忽略大小写
                        if (op != null)
                        {
                            var value = op.GetValue(source);

                            if (value == null)
                            {
                                targetProp.SetValue(target, null, null);
                                continue;
                            }
                            Type[] types = targetProp.PropertyType.GenericTypeArguments;
                            if (types.Length > 0)
                            {
                                if (types[0].FullName.IndexOf("System", StringComparison.Ordinal) != -1)
                                {
                                    targetProp.SetValue(target, Convert.ChangeType(value, types[0]), null);
                                }
                                else if (types[0].BaseType == typeof(Enum))
                                {
                                    targetProp.SetValue(target, Enum.Parse(types[0], value.ToString()), null);
                                }
                                else
                                {
                                    targetProp.SetValue(target, Convert.ChangeType(value, targetProp.PropertyType),
                                        null);
                                }
                            }
                            else
                            {
                                if (targetProp.PropertyType.BaseType == typeof(Enum))
                                {
                                    targetProp.SetValue(target, Enum.Parse(targetProp.PropertyType, value.ToString()),
                                        null);
                                }
                                else
                                {
                                    targetProp.SetValue(target, Convert.ChangeType(value, targetProp.PropertyType),
                                        null);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // 类型转换出错 直接忽略
                    }
                }
            }
            return target;
        }
        /// <summary>
        /// 对象之间属性Copy
        /// </summary>
        /// <typeparam name="T">目标</typeparam>
        /// <typeparam name="T1">来源</typeparam>
        /// <param name="objSourceList">来源实例</param>
        /// <returns></returns>
        public static IEnumerable<TTarget> Mapp<TTarget, TSource>(IEnumerable<TSource> sources)
            where TTarget : class
            where TSource : class
        {
            if (sources == null || sources.Count() <= 0)
            {
                yield return default;
            }
            else
            {
                foreach (var item in sources)
                {
                    yield return Mapp<TTarget, TSource>(item);
                }
            }
        }
        public static void SetPrivateFiledOrProp<T>(this T source, string name, object value)
            where T : class
        {
            try
            {
                var fileds = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(p => !p.CustomAttributes.Any(c => c.GetType().FullName == "System.Reflection.CustomAttributeData")).ToList();
                if (fileds != null)
                {
                    foreach (var item in fileds)
                    {
                        if (item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            item.SetValue(source, value);
                            return;
                        }
                    }
                }

                var Props = source.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                if (Props != null)
                {
                    foreach (var item in Props)
                    {
                        if (item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            item.SetValue(source, value);
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private static TypeValue TryGetOrAdd(Type type)
        {
            return cacheType.GetOrAdd(type, (key) =>
            {
                return new TypeValue(type);
            });
        }

    }

    internal class TypeValue
    {
        public List<PropertyInfo> Propertys { get; private set; }
        public List<PropertyInfo> CanReadProperty { get; private set; }
        public List<PropertyInfo> CanWriteProperty { get; private set; }
        internal TypeValue(Type type)
        {
            Propertys = type.GetProperties().ToList();
            CanReadProperty = Propertys.Where(p => p.CanRead).ToList();
            CanWriteProperty = Propertys.Where(p => p.CanWrite).ToList();
        }
    }
}
