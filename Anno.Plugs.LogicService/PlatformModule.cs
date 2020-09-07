using System;
using System.Collections.Generic;
using Anno.Model;
using System.Linq.Expressions;
using Anno.Command;
using Anno.Const.Attribute;
using Anno.Domain.Dto;
using Anno.Domain.Member;
using Anno.EngineData;
using Anno.QueryServices.Member;
using Anno.Const.Enum;
using Anno.Infrastructure;
using Anno.QueryServices.Platform;

namespace Anno.Plugs.LogicService
{
    using Anno.CommandBus;

    public class PlatformModule : BaseModule
    {
        private readonly IMemberQuery _memberQuery;
        private readonly IMemberService _memberService;
        private readonly IPlatformQuery _platformQuery;
        public PlatformModule()
        {
            _memberQuery = this.Resolve<IMemberQuery>();
            _memberService = this.Resolve<IMemberService>();
            _platformQuery = this.Resolve<IPlatformQuery>();
        }
        /// <summary>
        /// 获取首页数据集
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取会员列表信息")]
        public ActionResult GetList_IndexViewModel()
        {
            var list = _memberQuery.GetMemberListModel();
            return new ActionResult(true, list, null, null);
        }
        /// <summary>
        /// 根据会员ID 获取会员信息
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "根据会员ID 获取会员信息")]
        public ActionResult GetMemberModel()
        {
            var dto = this.Request<GetMemberModelInputDto>("dto");
            var outputDto = _memberQuery.GetMemberModel(dto);
            return new ActionResult(true, outputDto, null, null);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "修改密码")]
        public ActionResult ChangePwd()
        {
            var dto = this.Request<ChangePwdInputDto>("dto");
            var rlt = false;
            try
            {
                var command = new ChangePwdCommand(dto.ID, 1, dto.pwd, dto.oldPwd);
                CommandBus.Instance.Send(command);
                rlt = command.Result.Status;
            }
            catch (Exception ex)
            {
                return new ActionResult(false, null, null, ex.Message);
            }
            return new ActionResult(true, rlt, null, null);
        }
        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "重置用户密码")]
        public ActionResult ReSetpwd()
        {
            if (Profile != null)
            {
                var id = this.RequestInt64("ID") ?? -1;
                var command = new ReSetPwdCommand(id, 1);
                CommandBus.Instance.Send(command);
                return new ActionResult(command.Result.Status, null, null, command.Result.Msg);
            }
            else
            {
                return new ActionResult(false, null, null, "无权操作，你的IP我们已经记录！");
            }
        }
        /// <summary>
        /// 用户启用禁用
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "用户启用禁用")]
        public ActionResult EditState()
        {
            if (Profile != null)
            {
                var id = this.RequestInt64("ID") ?? -1;
                var state = (UserState)(this.RequestInt16("state") ?? 0);
                var command = new EnableUserCommand(id, 1, state);
                CommandBus.Instance.Send(command);
                return new ActionResult(command.Result.Status, null, null, command.Result.Msg);
            }
            else
            {
                return new ActionResult(false, null, null, "无权操作，你的IP我们已经记录！");
            }
        }
        [AnnoInfo(Desc = "保存用户角色")]
        public ActionResult SaveCurRoles()
        {
            if (Profile != null)
            {
                var uid = RequestInt64("uid");
                var link = Request<List<UserRoleDto>>("inputData");
                SaveUserRoleCommand command = new SaveUserRoleCommand(uid ?? -1, link, 1);
                CommandBus.Instance.Send(command);
                return new ActionResult(command.Result.Status, null, null, command.Result.Msg);
            }
            return new ActionResult(false, null, null, "无权操作，你的IP我们已经记录！");
        }
        [AnnoInfo(Desc = "添加系统角色")]
        public ActionResult AddRole()
        {
            if (Profile != null)
            {
                var name = RequestString("Name");
                AddRoleCommand command = new AddRoleCommand(name, -1, 1);
                CommandBus.Instance.Send(command);
                return new ActionResult(command.Result.Status, new { ID = command.Id }, null, command.Result.Msg);
            }
            return new ActionResult(false, null, null, "无权操作，你的IP我们已经记录！");
        }
        [AnnoInfo(Desc = "删除系统角色")]
        public ActionResult DelRole()
        {
            if (Profile != null)
            {
                var rid = RequestInt64("ID");
                DelRoleCommand command = new DelRoleCommand(rid ?? -1, 1);
                CommandBus.Instance.Send(command);
                return new ActionResult(command.Result.Status, null, null, command.Result.Msg);
            }
            return new ActionResult(false, null, null, "无权操作，你的IP我们已经记录！");
        }
        [AnnoInfo(Desc = "修改功能角色关系")]
        public ActionResult ModifyRoleLink()
        {
            if (Profile != null)
            {
                var frl = Request<List<sys_func_roles_link>>("inputData");
                var rid = RequestInt64("rid");
                if (frl.Count > 0)
                {
                    ModifyRoleFuncLinkCommand command = new ModifyRoleFuncLinkCommand(rid ?? -1, 1);
                    frl.ForEach(f =>
                    {
                        if (f.fid != null)
                        {
                            command.FidList.Add(f.fid ?? -1);
                        }
                    });
                    CommandBus.Instance.Send(command);
                    return new ActionResult(command.Result.Status, DbInstance.Db.Queryable<sys_func_roles_link>().ToList(), null, command.Result.Msg);
                }
                else
                {
                    return new ActionResult(false, null, null, "角色功能列表至少1条！");
                }
            }
            return new ActionResult(false, null, null, "无权操作，你的IP我们已经记录！");
        }
        [AnnoInfo(Desc = "服务InvokeTest调用服务InvokeTest1")]
        public ActionResult InvokeTest()
        {
            /*测试全局函数调用*/
            var gRlt = InvokeProcessor(RequestString(Eng.NAMESPACE), RequestString(Eng.CLASS), "InvokeTest1",
                Input);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult>(gRlt);
        }
        [AnnoInfo(Desc = "服务InvokeTest1多线程同时调用多个服务接口")]
        public ActionResult InvokeTest1()
        {
            /*测试全局函数调用*/
            var gRlt = InvokeProcessorAsync(RequestString(Eng.NAMESPACE), RequestString(Eng.CLASS), "GetList_IndexViewModel",
                Input);
            var sdaNum = InvokeProcessorAsync("Anno.Plugs.SerialRule", "RuleFactory", "CreateSdaNum", Input);
            var action = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult>(gRlt.Result);
            action.Msg = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult>(sdaNum.Result).OutputData.ToString();
            return action;
        }
        /// <summary>
        /// 修改功能树
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "修改系统功能")]
        public ActionResult EditFunc()
        {
            var func = Request<sys_func>("inputData");
            var type = RequestInt32("type") ?? 4;
            SysFuncCommand command = new SysFuncCommand(func.ID, 1);
            command.Mapp(func);
            Enum.TryParse(type.ToString(), out SysType systype);
            command.SysType = systype;
            CommandBus.Instance.Send(command);
            return new ActionResult(command.Result.Status, null, null, command.Result.Msg);
        }
        [AnnoInfo(Desc = "添加系统用户")]
        public ActionResult AddUser()
        {
            AddUserCommand command = new AddUserCommand(IdWorker.NextId(), 1);
            #region 接收数据
            var m = Request<sys_member>("ubase");
            var lr = Request<List<sys_roles>>("uroles");
            command.Mapp(m);
            command.rdt = DateTime.Now;
            lr?.ForEach(r =>
            {
                command.RolesCmd.Add(new RoleCommand()
                {
                    id = r.ID,
                    name = r.name
                });
            });
            #endregion
            CommandBus.Instance.Send(command);
            return new ActionResult(command.Result.Status, null, null, command.Result.Msg);
        }

        #region 20180215
        [AnnoInfo(Desc = "登录")]
        public ActionResult Login()
        {
            var rlt = _platformQuery.Login(this.RequestString("account"), this.RequestString("pwd"));
            if (rlt.Status)
            {
                var sysMembe = rlt.OutputData as sys_member;

                if (Const.RedisConfigure.Default().Switch)
                {
                    //Redis.RedisHelper.Set("sys_member:" + sysMembe.account, sysMembe, Const.RedisConfigure.Default().ExpiryDate.Minutes);
                }

                var command = new LoginRecordCommand(sysMembe.ID, sysMembe.profile, 1);
                CommandBus.Instance.Send(command);
            }
            return rlt;
        }
        /// <summary>
        /// 获取权限根节点
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取用户功能根")]
        public ActionResult GetUsrFc()
        {

            return _platformQuery.GetUsrFc(Profile);
        }
        /// <summary>
        /// 获取用户功能
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取用户功能")]
        public ActionResult GetFunc()
        {
            return _platformQuery.GetFunc(Profile);
        }
        /// <summary>
        /// 会员信息
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "会员信息")]
        public ActionResult PCenter()
        {
            if (Profile != null)
            {
                sys_member member = null;
                if (RequestContainsKey("type") && RequestContainsKey("id"))
                {
                    member = DbInstance.Db.Queryable<sys_member>().Where(m => (m.ID == RequestInt64("id"))).First();
                }
                else
                {
                    member = DbInstance.Db.Queryable<sys_member>().Where(m => (m.ID == Profile.ID)).First();
                }
                return new ActionResult(true, member);
            }
            else
            {
                return new ActionResult(false, null, null, "未授权的访问");
            }
        }
        [AnnoInfo(Desc = "获取公司信息")]
        public ActionResult GetAllbif_company()
        {
            PageParameter pageParameter = new PageParameter
            {
                Page = RequestInt32("page") ?? 1,
                Pagesize = RequestInt32("pagesize") ?? 20,
                SortName = RequestString("sortname") ?? "rdt",
                SortOrder = RequestString("sortorder") ?? "desc",
                Where = Filter()
            };
            var allCompany = _platformQuery.GetAllCompany(pageParameter);
            return allCompany;
        }
        [AnnoInfo(Desc = "获取系统会员信息")]
        public ActionResult GetAllsys_member()
        {
            PageParameter pageParameter = new PageParameter
            {
                Page = RequestInt32("page") ?? 1,
                Pagesize = RequestInt32("pagesize") ?? 20,
                SortName = RequestString("sortname") ?? "rdt",
                SortOrder = RequestString("sortorder") ?? "desc",
                Where = Filter()
            };
            var allUser = _platformQuery.GetAllUser(pageParameter);
            return allUser;
        }
        /// <summary>
        /// 获取所有功能点
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取所有功能点")]
        public ActionResult Get_all_power()
        {
            return _platformQuery.Get_all_power();
        }
        /// <summary>
        /// 获取用户角色列表
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取用户角色列表")]
        public ActionResult GetcurRoles()
        {
            return _platformQuery.GetcurRoles(RequestString("uid") ?? Profile.ID.ToString());
        }
        /// <summary>
        /// 获取功能集合
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取功能集合")]
        public ActionResult Get_rfs()
        {
            return _platformQuery.Get_rfs();
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 修改个人密码
        /// </summary>
        /// <returns></returns>
        public ActionResult EditPersonalPwd()
        {
            var changePwdCommand = new Anno.Command.ChangePwdCommand(Profile.ID, 1, RequestString("pwd"), RequestString("opwd"));
            CommandBus.Instance.Send(changePwdCommand);
            return new ActionResult(changePwdCommand.Result.Status);
        }
        #endregion

        #region Module 初始化
        /// <summary>
        /// 身份认证服务
        /// </summary>
        /// <returns></returns>
        public ActionResult Identity()
        {
            return new ActionResult(true, Profile, null, null);
        }
        public override bool Init(Dictionary<string, string> input)
        {
            base.Init(input);
            if (RequestString("method") != "Login")
            {
                Profile = _platformQuery.Identity(this.RequestString("profile"));
            }
            return true;
        }
        #endregion
    }
}
