using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.Member;

namespace Anno.Domain.Repository
{
    /// <summary>
    /// 会员模型仓储
    /// </summary>
    public interface IMemberRepository:IRepository<SysMember>
    {
        /// <summary>
        /// 添加会员
        /// </summary>
        /// <param name="sysMember"></param>
        /// <returns></returns>
        (bool success, string msg) AddMember(SysMember sysMember);
        /// <summary>
        /// 用户名称是否已存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool Exist(string account);
    }
}
