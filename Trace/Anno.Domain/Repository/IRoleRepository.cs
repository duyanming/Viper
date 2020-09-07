using System;
using System.Collections.Generic;
using System.Text;


namespace Anno.Domain.Repository
{
    using Anno.Domain.Role;
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>
        /// 根据ID 判断角色是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exist(long id);
        /// <summary>
        /// 根据角色名称判断角色是否存在
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        bool Exist(string roleName);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        (Boolean success,string msg) Add(Role role);

        /// <summary>
        /// 移除系统角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        (Boolean success, string msg) Remove(long id);
    }
}
