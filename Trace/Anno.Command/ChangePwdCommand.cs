using System;
using Anno.CommandBus;
namespace Anno.Command
{
    /// <summary>
    /// 用户修改密码
    /// </summary>
    public class ChangePwdCommand:CommandBus.Command
    {
        /// <summary>
        /// 构造函数 修改用户密码 （测试修改 值传递了 用户的 id）
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="version">版本可以为空</param>
        public ChangePwdCommand(long id,int version):base(id,version) {

        }
        public ChangePwdCommand(long id, int version,string pwd,string opwd) : this(id, version) {
            this.pwd = pwd;
            this.opwd=opwd;
        }
        /// <summary>
        /// 新密码
        /// </summary>
        public string pwd { get; set; }
        /// <summary>
        /// 原始密码
        /// </summary>
        public string opwd { get; set; }
    }
}
