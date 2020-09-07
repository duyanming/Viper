using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Anno.CommandBus
{
    public class Command : ICommand
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; private set; }
        /// <summary>
        /// Command 执行结果集
        /// </summary>
        public CommandResult Result { get; set; }=new CommandResult();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public Command(long id, int version)
        {
            Id = id;
            Version = version;
        }
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
                        if (string.Equals(targetProp.Name, "id", StringComparison.CurrentCultureIgnoreCase) && this.Id.ToString() != "0")//ID 不为空的时候Copy 属性
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
    }
    /// <summary>
    /// CommandBus 执行结果集 只返回简单的状态
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public Boolean Status { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 字典
        /// </summary>
        public Dictionary<string, object> Output { get; set; }
        /// <summary>
        /// 结果集
        /// </summary>
        public object OutputData { get; set; }
    }
}
