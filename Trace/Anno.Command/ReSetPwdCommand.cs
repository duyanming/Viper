using System;
using Anno.CommandBus;
namespace Anno.Command
{
    /// <summary>
    /// 用户修改密码
    /// </summary>
    public class ReSetPwdCommand : CommandBus.Command
    {
        /// <summary>
        /// 构造函数 修改用户密码 （测试修改 值传递了 用户的 id）
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="version">版本可以为空</param>
        public ReSetPwdCommand(long id,int version):base(id,version) {

        }
    }
}
