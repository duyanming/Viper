using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Anno.Common;
using Anno.EngineData;
using Anno.Model;
using Anno.Repository;
using Newtonsoft.Json;
using SqlSugar;

namespace Anno.QueryServices.Platform
{
    public class PlatformQuery : IPlatformQuery
    {
        private readonly SqlSugar.SqlSugarClient _db;

        public PlatformQuery(IBaseQueryRepository baseRepository)
        {
            _db = baseRepository.Context;
        }
        #region 用户
        public ActionResult GetAllUser(PageParameter pageParameter)
        {
            var totalNumber = 0;
            if (string.IsNullOrWhiteSpace(pageParameter.SortName))
            {
                pageParameter.SortName = "rdt";
            }
            if (string.IsNullOrWhiteSpace(pageParameter.SortOrder))
            {
                pageParameter.SortOrder = "desc";
            }
            var dt = _db.Queryable<sys_member, bif_company>((m, c) => new object[] { SqlSugar.JoinType.Left, m.coid == c.ID })
                .Where(pageParameter.Where)
                .Select((m, c) => new { m.account, m.name, m.coid, m.ID, m.position, m.rdt, m.state, m.timespan, co_n = c.name, co = c.code })
                .MergeTable()
                .OrderBy($"{pageParameter.SortName} {pageParameter.SortOrder}")
                .ToPageList(pageParameter.Page, pageParameter.Pagesize, ref totalNumber);
            var output = new Dictionary<string, object> { { "#Total", totalNumber }, { "#Rows", dt } };
            return new ActionResult(true, null, output);
        }
        /// <summary>
        /// 公司列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllCompany(PageParameter pageParameter)
        {
            var totalNumber = 0;
            if (string.IsNullOrWhiteSpace(pageParameter.SortName))
            {
                pageParameter.SortName = "rdt";
            }
            if (string.IsNullOrWhiteSpace(pageParameter.SortOrder))
            {
                pageParameter.SortOrder = "desc";
            }
            var dt = _db.Queryable<bif_company>()
                .Where(pageParameter.Where)
                .OrderBy($"{pageParameter.SortName} {pageParameter.SortOrder}")
                .ToPageList(pageParameter.Page, pageParameter.Pagesize, ref totalNumber);
            var output = new Dictionary<string, object> { { "#Total", totalNumber }, { "#Rows", dt } };
            return new ActionResult(true, null, output);
        }
        /// <summary>
        /// 根据用户 ID 获取用户信息
        /// </summary>
        /// <param name="uid">用户 ID</param>
        /// <returns></returns>
        public ActionResult GetUserInfo(long uid)
        {
            var usr = _db.Queryable<sys_member>().Where(m => (m.ID == uid)).First();
            if (usr == null)
            {
                return new ActionResult(false, null, null, "用户 ID 未知！");
            }
            return new ActionResult(true, usr);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public ActionResult Login(string account, string pwd)
        {
            pwd = CryptoHelper.TripleDesEncrypting(pwd.Trim());
            var u = _db.Queryable<sys_member>().Where(m => (m.account == account && m.pwd == pwd)).First();
            if (u == null)
            {
                return new ActionResult(false, null, null, "用户名或密码错误");
            }

            if (u.state == 0)
            {
                return new ActionResult(false, u, null, "用户已经被停用，请联系管理员！");
            }
            u.profile = CryptoHelper.TripleDesEncrypting($"{u.ID},{u.account},{DateTime.Now}");
            u.timespan = DateTime.Now;
            return new ActionResult(true, u);
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="account">用户登录名称</param>
        /// <returns></returns>
        public ActionResult LogOut(string account)
        {
            return new ActionResult(Redis.RedisHelper.Remove($"sys_member:{account}"));
        }
        /// <summary>
        /// 密码修改，启用停用、重置
        /// </summary>
        /// <returns></returns>
        public ActionResult ReSetpwd(Dictionary<string, string> input, sys_member profile)
        {
            sys_member usr = new sys_member();
            if (input.ContainsKey("pwd"))  //修改密码
            {
                if (input["pwd"].Length < 6)
                {
                    return new ActionResult(false, null, null, "新密码至少6位");
                }
                usr = profile;
                string pwd = CryptoHelper.TripleDesEncrypting(input["opwd"]);
                string Npwd = CryptoHelper.TripleDesEncrypting(input["pwd"]);
                if (usr.pwd == pwd)
                {
                    usr.pwd = Npwd;
                    _db.Updateable(usr).ExecuteCommand();
                    return new ActionResult(true);
                }
                else
                {
                    return new ActionResult(false, null, null, "旧密码验证失败：管理员【微信：Anno6295】");
                }

            }
            else if (input.ContainsKey("state"))  //启用停用
            {
                usr.ID = Convert.ToInt32(input["uid"]);
                usr.state = Convert.ToInt16(input["state"]);
                _db.Updateable(usr).UpdateColumns(m => new { m.state }).ExecuteCommand();
            }
            else
            {  //重置密码
                usr.ID = Convert.ToInt32(input["uid"]);
                usr.pwd = CryptoHelper.TripleDesEncrypting(Const.AppSettings.DefaultPwd);
                _db.Updateable(usr).UpdateColumns(m => new { m.pwd }).ExecuteCommand();
            }
            return new ActionResult(true);
        }

        /// <summary>
        /// 获取在线用户数
        /// </summary>
        /// <returns></returns>
        public ActionResult OnLine()
        {
            int online = Redis.RedisHelper.GetLikeKeysCount("sys_member:");
            var output = new Dictionary<string, object> { { "#online", online } };
            return new ActionResult(true, null, output);
        }
        #endregion

        #region 功能配置
        /// <summary>
        /// 获取全部功能
        /// </summary>
        /// <returns></returns>
        public ActionResult Get_all_power()
        {
            var func = _db.Queryable<sys_func>().OrderBy(f => f.forder, SqlSugar.OrderByType.Asc).ToList();
            return new ActionResult(true, func);
        }
        /// <summary>
        /// 返回用户的功能点
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public ActionResult GetFunc(ProfileToken profile)
        {
            var usr = profile;
            if (usr != null)
            {
                object funcList = _db.Ado.SqlQuery<dynamic>(@"SELECT * FROM sys_func f WHERE `show`=1 and f.pid is not null  AND f.id in(
                SELECT DISTINCT frl.fid FROM sys_func_roles_link frl WHERE frl.rid in (
                SELECT rid from sys_member_roles_link lmr WHERE lmr.mid=" + usr.ID + "))  order by forder asc");
                return new ActionResult(true, funcList);
            }
            return new ActionResult(false, null, null, "用户身份不能为空！");
        }
        /// <summary>
        /// 获取权限根节点
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public ActionResult GetUsrFc(ProfileToken profile)
        {
            if (profile == null)
            {
                return new ActionResult(false, null, null, "无权访问，你的IP已经被记录！");
            }
            object rootFunc = _db.Ado.SqlQuery<dynamic>(@"SELECT * FROM sys_func f WHERE `show`=1 and f.pid is  null  AND f.id in(
            SELECT DISTINCT frl.fid FROM sys_func_roles_link frl WHERE frl.rid in (
            SELECT rid from sys_member_roles_link lmr WHERE lmr.mid=" + profile.ID + "))  order by forder asc");
            return new ActionResult(true, rootFunc);
        }

        /// <summary>
        /// 编辑功能
        /// </summary>
        /// <param name="input"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public ActionResult EditFunc(Dictionary<string, string> input, sys_member profile)
        {
            sys_func f = JsonConvert.DeserializeObject<sys_func>(input["inputData"]);
            if (input["type"] == "save")
            {
                _db.Updateable(f).ExecuteCommand();
            }
            else if (input["type"] == "del")
            {
                if (!_db.Queryable<sys_func>().Where(_f => _f.pid == f.ID).Any())
                {
                    _db.Deleteable<sys_func>().Where(d => d.ID == f.ID).ExecuteCommand();
                }
                else
                {
                    return new ActionResult(false, null, null, "请先删除子节点");
                }
            }
            else
            {
                if (f.pid != null && f.pid == 0)
                {
                    f.pid = null;
                }
                if (input.ContainsKey("child") && input["child"] == "child")
                {
                    f.pid = f.ID;
                }
                _db.Insertable(f).ExecuteCommand();
            }
            return new ActionResult(true);
        }
        #endregion

        #region 角色功能配置
        public ActionResult Get_rfs()
        {
            List<sys_roles> lr = _db.Queryable<sys_roles>().ToList();
            List<sys_func_roles_link> lrl = _db.Queryable<sys_func_roles_link>().ToList();
            List<sys_func> lf = _db.Queryable<sys_func>().OrderBy(f => f.forder, SqlSugar.OrderByType.Asc).ToList();
            object ods = new { lr, lrl, lf };
            return new ActionResult(true, ods);
        }
        /// <summary>
        /// 获取当前用户的角色列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetcurRoles(string uid)
        {
            object ods = null;
            List<sys_roles> lr = _db.Queryable<sys_roles>().ToList();
            var cur = _db.Queryable<sys_roles, sys_member_roles_link>((r, l) => new object[] { JoinType.Left, r.ID == l.rid })
                .Select((r, l) => new { r.ID, r.name, lid = l.ID })
                .Where("l.mid=" + uid).ToList();
            ods = new { lr, cur };
            return new ActionResult(true, ods);
        }

        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveSurRoles(Dictionary<string, string> input, sys_member profile)
        {
            sys_member usr = profile;
            if (usr != null)
            {
                List<sys_roles> frl = JsonConvert.DeserializeObject<List<sys_roles>>(input["inputData"]);
                sys_member_roles_link mrl = new sys_member_roles_link();
                _db.Deleteable<sys_member_roles_link>().Where(l => l.mid == Convert.ToInt32(input["uid"])).ExecuteCommand();
                foreach (sys_roles r in frl)
                {
                    mrl = new sys_member_roles_link();
                    mrl.mid = Convert.ToInt32(input["uid"]);
                    mrl.rid = Convert.ToInt32(r.ID);
                    _db.Insertable(mrl).ExecuteCommand();
                }
            }
            return new ActionResult(true);
        }
        public ActionResult AddRoles(Dictionary<string, string> input)
        {
            sys_roles r = JsonConvert.DeserializeObject<sys_roles>(input["inputData"]);
            if (!input.ContainsKey("type"))
            {
                if (!_db.Queryable<sys_roles>().Where(_r => _r.name == r.name).Any())
                {
                    r.ID = _db.Insertable(r).ExecuteReturnBigIdentity();
                }
                else
                {
                    return new ActionResult(false, null, null, "该角色已存在");
                }
            }
            else if (input["type"] == "del")
            {
                _db.Deleteable<sys_roles>().Where(o => o.ID == r.ID).ExecuteCommand();
                _db.Deleteable<sys_func_roles_link>().Where(frl => frl.rid == r.ID).ExecuteCommand();
            }
            return new ActionResult(true, r);
        }

        public ActionResult UpdateRoles_link(Dictionary<string, string> input)
        {
            List<sys_func_roles_link> frl = JsonConvert.DeserializeObject<List<sys_func_roles_link>>(input["inputData"]);
            List<sys_func_roles_link> frl_O = _db.Queryable<sys_func_roles_link>().ToList();

            List<sys_func_roles_link> addfrl = new List<sys_func_roles_link>();
            foreach (sys_func_roles_link f in frl)
            {
                if (frl_O.Find(_f => _f.fid == f.fid && _f.rid == f.rid) == null)
                {
                    addfrl.Add(f);
                }
            }
            if (addfrl.Count > 0)
            {
                _db.Insertable(addfrl).ExecuteCommand();
            }
            var outputData = _db.Queryable<sys_func_roles_link>().ToList();
            return new ActionResult(true, outputData);
        }
        #endregion

        /// <summary>
        /// 身份验证    account=null 的时候去数据库查询
        /// </summary>
        /// <param name="account">用户名</param>
        /// <param name="token">token</param>
        /// <returns></returns>
        public ProfileToken Identity(string token)
        {
            string[] profiles = new string[3];
            try
            {
                /*
                 * profiles[0] 用户ID
                 * profiles[1] 用户Account
                 * profiles[2] token 产生日期
                 */
                profiles = CryptoHelper.TripleDesDecrypting(token).Split(',');
            }
            catch{
                throw new Exception("Illegal token.");
            }
            ProfileToken u = null;
            if (Const.RedisConfigure.Default().Switch)
            {
                u = Redis.RedisHelper.Get<ProfileToken>("sys_member:" + profiles[1]);
                if (u != null)
                {
                    Redis.RedisHelper.KeyExpire("sys_member:" + profiles[1], Convert.ToInt32(Const.RedisConfigure.Default().ExpiryDate.Minutes));
                    if (u.Profile == token)
                    {
                        return u;
                    }
                }
                return null;
            }
            else
            {
                return _db.Queryable<ProfileToken>().AS("sys_member").Where(m => (m.Profile == token && m.State == 1)).First();
            }
        }

    }
}
