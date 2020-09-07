using System;
using Anno.CommandBus;
namespace Anno.Command
{
    /// <summary>
    /// 用户修改密码
    /// </summary>
    public class EnableUserCommand : CommandBus.Command
    {
        /// <summary>
        /// 构造函数 修改用户密码 （测试修改 值传递了 用户的 id）
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="version">版本可以为空</param>
        public EnableUserCommand(long id,int version,UserState state):base(id,version)
        {
            State = state;
        }
        /// <summary>
        /// 用户状态
        /// </summary>
        public UserState State { get; set; }

    }

    /// <summary>
    /// 用户启用停用装填
    /// </summary>
    public enum UserState
    {
        停用=0,
        启用=1
    }
}
