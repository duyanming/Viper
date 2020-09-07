using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command
{
    public class AddUserCommand : CommandBus.Command
    {
        public AddUserCommand(long id, int version) : base(id, version)
        {
        }
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pwd { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public long coid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// _1 启用 0 禁用
        /// </summary>
        public short? state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? timespan { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? rdt { get; set; }
        #endregion
        /// <summary>
        /// 角色
        /// </summary>
        public List<RoleCommand> RolesCmd=new List<RoleCommand>();
    }

    /// <summary>
    /// 用户角色
    /// </summary>
    public class RoleCommand
    {
        public long id { get; set; }
        public string name { get; set; }
    }
}
