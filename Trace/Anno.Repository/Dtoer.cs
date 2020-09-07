using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Anno.Repository
{
    public static class Dtoer
    {
        /// <summary>
        /// DataTable（SQLsugar） 转化为 List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">表</param>
        /// <returns>对象集合</returns>
        public static List<T> Mapper<T>(DataTable table)
        {
            List<T> objList = new List<T>();
            if (table == null || table.Rows.Count <= 0)
            {
                return default(List<T>);
            }
            var t = typeof(T);
            List<PropertyInfo> targetProps = t.GetProperties().Where(p => p.CanWrite).ToList();
            if (targetProps.Count > 0 && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    T obj = (T)t.Assembly.CreateInstance(t.FullName);
                    foreach (DataColumn col in table.Columns)
                    {
                        try
                        {
                            var targetProp = targetProps.Find(p =>
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
                    objList.Add(obj);
                }
            }
            return objList;
        }
    }
}
