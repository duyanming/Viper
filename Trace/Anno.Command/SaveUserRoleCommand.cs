using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.Dto;

namespace Anno.Command
{
    /// <summary>
    /// 根据ID 修改用户角色
    /// </summary>
    public class SaveUserRoleCommand :CommandBus.Command
    {
        public SaveUserRoleCommand(long id,List<UserRoleDto> roleDto, int version) : base(id, version)
        {
            UserRole = roleDto;
        }
        /// <summary>
        /// 用户的角色
        /// </summary>
        public List<UserRoleDto> UserRole { get; set; }
    }
}
