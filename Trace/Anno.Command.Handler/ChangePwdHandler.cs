using System;
using Autofac;

using Anno.CommandBus;
using Anno.Domain.Member;
using Anno.Domain.Repository;

namespace Anno.Command.Handler
{
    /// <summary>
    /// 作者：杜燕明
    /// 日期：2018-01-11
    /// 用户 Handler
    /// </summary>
    public class ChangePwdHandler
    : ICommandHandler<ChangePwdCommand>
    , ICommandHandler<LoginRecordCommand>
    , ICommandHandler<ReSetPwdCommand>
    , ICommandHandler<EnableUserCommand>
    , ICommandHandler<SaveUserRoleCommand>
    , ICommandHandler<AddUserCommand>
    {
        /// <summary>
        /// 会员仓储
        /// </summary>
        private readonly IMemberRepository memberRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChangePwdHandler(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="command"></param>
        public void Execute(ChangePwdCommand command)
        {
            SysMember sysMember = memberRepository.GetById(command.Id);
            sysMember.ChangePwd(command.pwd, command.opwd);
            memberRepository.SaveChange(sysMember);
            command.Result = new CommandResult() { Status = true, Msg = "修改密码成功！" };
        }
        /// <summary>
        /// 用户登录后 持久化指令 Token
        /// </summary>
        /// <param name="command"></param>
        public void Execute(LoginRecordCommand command)
        {
            SysMember sysMember = memberRepository.GetById(command.Id);
            sysMember.SetProfile(command.Token);
            memberRepository.SaveChange(sysMember);
            command.Result = new CommandResult() { Status = true };
        }
        /// <summary>
        /// 管理员重置用户密码
        /// </summary>
        /// <param name="command"></param>
        public void Execute(ReSetPwdCommand command)
        {
            SysMember sysMember = memberRepository.GetById(command.Id);
            sysMember.ReSetPwd(Common.CryptoHelper.TripleDesEncrypting(Const.AppSettings.DefaultPwd));
            memberRepository.SaveChange(sysMember);
            command.Result = new CommandResult() { Status = true };
        }

        /// <summary>
        /// 用户启用停用
        /// </summary>
        /// <param name="command"></param>
        public void Execute(EnableUserCommand command)
        {
            SysMember sysMember = memberRepository.GetById(command.Id);
            sysMember.Enable((short)command.State);
            memberRepository.SaveChange(sysMember);
            command.Result = new CommandResult() { Status = true };
        }

        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <param name="command"></param>
        public void Execute(SaveUserRoleCommand command)
        {
            SysMember sysMember = memberRepository.GetById(command.Id);
            sysMember.RemoveAllRole();
            command.UserRole.ForEach(r => { sysMember.AddRole(r.ID, r.name); });
            //var rlt = sysMember.AddRoles(command.UserRole);
            memberRepository.SaveChange(sysMember);
            command.Result = new CommandResult() { Status = true };
        }
        /// <summary>
        /// 添加系统用户
        /// </summary>
        /// <param name="command"></param>
        public void Execute(AddUserCommand command)
        {
            var sysMember = new SysMember();
            sysMember.Mapp(command);
            var check = sysMember.ValidateUserInfo();
            if (!check.success)
            {
                command.Result.Status = false;
                command.Result.Msg = check.msg;
                return;
            }
            //加密密码
            sysMember.DesPwd();
            if (memberRepository.Exist(sysMember.Account))
            {
                command.Result.Status = false;
                command.Result.Msg = "用户登录名已存在！";
                return;
            }
            command.RolesCmd.ForEach(r => { sysMember.AddRole(r.id, r.name); });
            var rlt = memberRepository.AddMember(sysMember);
            command.Result.Status = rlt.success;
            command.Result.Msg = rlt.msg;
        }
    }
}
