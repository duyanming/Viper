using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command
{
    public class SysFuncCommand : CommandBus.Command
    {
        public SysFuncCommand(long id, int version) : base(id, version)
        {
        }
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string Fname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Fcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Forder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? Pid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short? Show { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        #endregion

        public SysType SysType { get; set; } = SysType.Update;
    }

    /// <summary>
    /// 系统功能业务类型
    /// </summary>
    public enum SysType
    {
        /// <summary>
        /// 添加平级
        /// </summary>
        AddFlatLevel=1,
        /// <summary>
        /// 添加子节点
        /// </summary>
        AddChild,
        /// <summary>
        /// 移除节点
        /// </summary>
        Remove,
        /// <summary>
        /// 更新节点
        /// </summary>
        Update,
    }
}
