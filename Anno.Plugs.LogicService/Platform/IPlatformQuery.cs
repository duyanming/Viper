using System;
using System.Collections.Generic;
using System.Text;
using Anno.EngineData;
using Anno.Model;

namespace Anno.QueryServices.Platform
{
    public interface IPlatformQuery
    {
        /// <summary>
        /// 获取系统所有用户 input["pagesize"] sortname  sortorder page
        /// </summary>
        /// <param name="pageParameter"></param>
        /// <returns></returns>
        ActionResult GetAllUser(PageParameter pageParameter);

        /// <summary>
        /// 获取系统所有公司 input["pagesize"] sortname  sortorder page
        /// </summary>
        /// <param name="pageParameter"></param>
        /// <returns></returns>
        ActionResult GetAllCompany(PageParameter pageParameter);
        /// <summary>
        /// 根据用户 ID 获取用户信息
        /// </summary>
        /// <param name="uid">用户 ID</param>
        /// <returns></returns>
        ActionResult GetUserInfo(long uid);
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        ActionResult Login(string account, string pwd);
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        ActionResult LogOut(string account);
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        ActionResult ReSetpwd(Dictionary<string, string> input, sys_member profile);
        /// <summary>
        /// 获取在线用户数量
        /// </summary>
        /// <returns></returns>
        ActionResult OnLine();
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        ActionResult Get_all_power();
        /// <summary>
        /// 返回用户的功能点
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        ActionResult GetFunc(ProfileToken profile);
        /// <summary>
        /// 获取权限根节点
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        ActionResult GetUsrFc(ProfileToken profile);
        /// <summary>
        /// 编辑功能
        /// </summary>
        /// <param name="input"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        ActionResult EditFunc(Dictionary<string, string> input, sys_member profile);
        /// <summary>
        /// 角色功能配置
        /// </summary>
        /// <returns></returns>
        ActionResult Get_rfs();
        /// <summary>
        /// 获取当前用户的角色列表
        /// </summary>
        /// <returns></returns>
        ActionResult  GetcurRoles(string uid);
        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <returns></returns>
        ActionResult SaveSurRoles(Dictionary<string, string> input, sys_member profile);
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ActionResult AddRoles(Dictionary<string, string> input);
        /// <summary>
        /// 更新角色权限
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ActionResult UpdateRoles_link(Dictionary<string, string> input);
        /// <summary>
        /// 身份验证
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        ProfileToken Identity(string token);
    }
}
