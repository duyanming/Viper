using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Anno.Model.BaseModel
{
    public abstract class Entity : Entity<long>, IEntity
    {

    }
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public TPrimaryKey ID { get; set; }

        public bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(ID, default(TPrimaryKey)))
            {
                return true;
            }
            if (typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(ID) <= 0;
            }

            if (typeof(TPrimaryKey) == typeof(long))
            {
                return Convert.ToInt64(ID) <= 0;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TPrimaryKey>))
            {
                return false;
            }

            //相同的实例必须被认为是相等的。
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //临时不被视为平等的
            var other = (Entity<TPrimaryKey>)obj;
            if (IsTransient() && other.IsTransient())
            {
                return false;
            }

            //必须有一个继承关系或类型必须相同
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return ID.Equals(other.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator ==(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{GetType().Name} {ID}]";
        }

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
                        if (string.Equals(targetProp.Name, "id", StringComparison.CurrentCultureIgnoreCase) && this.ID?.ToString() != "0")//ID 不为空的时候Copy 属性
                        {
                            continue;
                        }
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

        public Entity<TPrimaryKey> Clone()
        {
            return this.MemberwiseClone() as Entity<TPrimaryKey>;
        }

        #endregion
    }
}
