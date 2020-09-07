using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.Dto
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class UserRoleDto : BaseDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string name { get; set; }
    }
}
