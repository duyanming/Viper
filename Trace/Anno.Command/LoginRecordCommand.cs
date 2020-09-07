using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command
{
    /// <summary>
    /// 登录后持久化登录状态（缓存 Redis Mysql）
    /// </summary>
    public class LoginRecordCommand : CommandBus.Command
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="profile">Token(Guid)</param>
        /// <param name="version"></param>
        public LoginRecordCommand(long id,string profile, int version) : base(id, version)
        {
            Token = profile;
        }
        /// <summary>
        /// 用户令牌
        /// </summary>
        public string Token { get; set; }
    }
}
