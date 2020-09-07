using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Domain.Dto
{
    public class BaseDto
    {
        public long ID { get; set; }

        #region 对象属性Copy
        /// <summary>
        /// 对象之间属性Copy
        /// </summary>
        /// <typeparam name="T">目标</typeparam>
        /// <typeparam name="T1">来源</typeparam>
        /// <param name="objSource">来源实例</param>
        /// <returns></returns>
        public T Mapp<T, T1>(T1 objSource)
        {
            Type t = typeof(T);
            if (objSource == null)
            {
                return default(T);
            }
            T target = (T)t.Assembly.CreateInstance(t.FullName);
            List<PropertyInfo> targetProps = t.GetProperties().Where(p => p.CanWrite == true).ToList();
            if (targetProps != null && targetProps.Count > 0)
            {
                var origin = objSource.GetType().GetProperties().ToList();
                foreach (PropertyInfo targetProp in targetProps)
                {
                    try
                    {
                        var op = origin.Find(p => string.Equals(p.Name, targetProp.Name, StringComparison.CurrentCultureIgnoreCase)); //忽略大小写
                        if (op != null)
                        {
                            var value = op.GetValue(objSource);
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
        public List<T> Mapp<T, T1>(List<T1> objSourceList)
        {
            List<T> tlist = new List<T>();
            Type t = typeof(T);
            if (objSourceList == null)
            {
                return tlist;
            }
            objSourceList.ForEach(o =>
            {
                tlist.Add(Mapp<T, T1>(o));
            });
            return tlist;
        }

        /// <summary>
        /// 主要用于 初始化 领域模型属性
        /// </summary>
        /// <param name="obj">来源对象s</param>
        public void Mapp(object obj)
        {
            List<PropertyInfo> targetProps = this.GetType().GetProperties().Where(p => p.CanWrite == true).ToList();
            if (targetProps != null && targetProps.Count > 0)
            {
                var origin = obj.GetType().GetProperties().ToList();
                foreach (PropertyInfo targetProp in targetProps)
                {
                    var op = origin.Find(p => string.Equals(p.Name, targetProp.Name, StringComparison.CurrentCultureIgnoreCase));//忽略大小写
                    if (op != null)
                    {
                        var value = op.GetValue(obj);
                        try
                        {
                            if (value == null)
                            {
                                targetProp.SetValue(this, null, null);
                                continue;
                            }
                            Type[] types = targetProp.PropertyType.GenericTypeArguments;
                            if (types.Length > 0)
                            {
                                if (types[0].FullName.IndexOf("System", StringComparison.Ordinal) != -1)
                                {
                                    targetProp.SetValue(this, Convert.ChangeType(value, types[0]), null);
                                }
                                else if (types[0].BaseType == typeof(Enum))
                                {
                                    targetProp.SetValue(this, Enum.Parse(types[0], value.ToString()), null);
                                }
                                else
                                {
                                    targetProp.SetValue(this, Convert.ChangeType(value, targetProp.PropertyType), null);
                                }
                            }
                            else
                            {
                                if (targetProp.PropertyType.BaseType == typeof(Enum))
                                {
                                    targetProp.SetValue(this, Enum.Parse(targetProp.PropertyType, value.ToString()), null);
                                }
                                else
                                {
                                    targetProp.SetValue(this, Convert.ChangeType(value, targetProp.PropertyType), null);
                                }
                            }
                        }
                        catch
                        {
                            // 类型转换出错 直接忽略
                        }
                    }
                }
            }
        }
        #endregion
    }
}
