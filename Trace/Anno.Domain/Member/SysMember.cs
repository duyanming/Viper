using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Anno.Domain.BaseModel;

namespace Anno.Domain.Member
{
    public partial class SysMember : AggregateRoot
    {
        public SysMember()
        {
        }
        public SysMember(long id)
        {
            this.ID = id;
        }

        #region 字段
        #endregion
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string Account { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Pwd { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public long Coid { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Position { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public short? State { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Profile { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Timespan { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Rdt { get; private set; }

        /// <summary>
        /// 用户的角色
        /// </summary>

        public List<Role> Roles { get; private set; } = new List<Role>();

        #endregion

        #region 业务
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPwd">新密码</param>
        /// <param name="oldPwd">旧密码</param>
        /// <returns></returns>
        public SysMember ChangePwd(string newPwd, string oldPwd)
        {
            oldPwd = Common.CryptoHelper.TripleDesEncrypting(oldPwd);
            if (this.Pwd != oldPwd)
            {
                throw new ArgumentException("原始密码不正确，请确认后再次修改！");
            }

            if (newPwd.Trim().Length < 6)
            {
                throw new ArgumentException("新密码长度不能小于6位，例如：【Anno12_】");
            }
            Pwd = Common.CryptoHelper.TripleDesEncrypting(newPwd);
            return this;
        }
        /// <summary>
        /// 持久化登录 Token
        /// </summary>
        /// <param name="token">用户登录令牌</param>
        /// <returns></returns>
        public SysMember SetProfile(string token)
        {
            this.Profile = token;
            Timespan = DateTime.Now;
            return this;
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="tripPwd">加密后的密码</param>
        /// <returns></returns>
        public SysMember ReSetPwd(string tripPwd)
        {
            this.Pwd = tripPwd;
            return this;
        }
        /// <summary>
        /// 用户启用停用
        /// </summary>
        /// <param name="state">1,启用，0停用</param>
        /// <returns></returns>
        public SysMember Enable(short state)
        {
            this.State = state;
            return this;
        }
        /// <summary>
        /// 添加一个角色
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SysMember AddRole(long rid, string name)
        {
            if (!Roles.Exists(r => r.ID == rid))
            {
                var role = new Role();
                role.Mapp(new { ID = rid, name });
                Roles.Add(role);
                // this.AddChange();
            }
            else
            {
                throw new ArgumentException("角色已经存在！");
            }
            return this;
        }

        public SysMember RemoveRole(long rid)
        {
            var role = Roles.Find(r => r.ID == rid);
            if (role != null)
            {
                Roles.Remove(role);
            }
            else
            {
                throw new ArgumentException("没有需要移除的对象！");
            }
            return this;
        }
        /// <summary>
        /// 移除全部角色
        /// </summary>
        /// <returns></returns>
        public SysMember RemoveAllRole()
        {
            this.Roles = new List<Role>();
            return this;
        }
        /// <summary>
        /// 密码加密前校验（否则密码加密错误）
        /// </summary>
        /// <returns></returns>
        public (bool success, string msg) ValidateUserInfo()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "用户名不能为空");
            }
            if (string.IsNullOrWhiteSpace(Account))
            {
                return (false, "登录名不能为空");
            }
            if (string.IsNullOrWhiteSpace(Pwd))
            {
                return (false, "密码不能为空");
            }
            return (true, null);
        }
        /// <summary>
        /// 加密用户密码 加过密的不会重复加密
        /// </summary>
        /// <returns></returns>
        public bool DesPwd()
        {
            try
            {
                /*
                 * 如果解密成功说明已经加密过，不需要重复加密；
                 */
                Common.CryptoHelper.TripleDesDecrypting(Pwd);
            }
            catch
            {
                Pwd = Common.CryptoHelper.TripleDesEncrypting(Pwd);
            }
            return true;
        }

        #endregion
    }
}
