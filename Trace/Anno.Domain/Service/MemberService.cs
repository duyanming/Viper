using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Anno.Domain.Member;

using Anno.Common;
using Anno.Domain.Repository;

namespace Anno.Domain.Service
{
    public class MemberService : IMemberService
    {
        //private readonly SqlSugar.SqlSugarClient _db;
        private readonly IBaseRepository _baseRepository;
        public MemberService(IBaseRepository baseRepository)
        {
            //_db = baseRepository.Context;
            _baseRepository = baseRepository;
        }
        public bool ChangePwd(Dto.ChangePwdInputDto args)
        {
            //Sys_member sysMember = new Sys_member(args.ID);
            //sysMember.ChangePwd(args.pwd);
            //int x = _db.Updateable(sysMember).UpdateColumns(t => new { t.pwd }).Where(t=>t.pwd==args.oldPwd).ExecuteCommand();
            //if (x <= 0)
            //{
            //    throw new ArgumentException("要修改的用户不存在！请重新赋值 Dto.ChangePwdInputDto.ID ");
            //}
            return true;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="m">用户基本信息</param>
        /// <param name="lr">用户角色</param>
        /// <returns></returns>
        //public bool AddUser(Model.sys_member m, List<Model.sys_roles> lr)
        //{
        //    if (string.IsNullOrWhiteSpace(m.name))
        //    {
        //        throw  new ArgumentException("用户名不能为空");
        //    }
        //    if (_db.Queryable<Model.sys_member>().Where(_m => _m.account == m.account).Any())
        //    {
        //        throw new ArgumentException("用户已存在");
        //    }
        //    if (string.IsNullOrWhiteSpace(m.account))
        //    {
        //        throw new ArgumentException("登录名不能为空");
        //    }
        //    if (string.IsNullOrWhiteSpace(m.pwd))
        //    {
        //        throw new ArgumentException("密码不能为空");
        //    }
        //    m.pwd = CryptoHelper.TripleDesEncrypting(m.pwd);
        //    m.ID = _db.Insertable(m).ExecuteReturnBigIdentity();
        //    List<Model.sys_member_roles_link> lmr = new List<Model.sys_member_roles_link>();
        //    foreach (Model.sys_roles r in lr)
        //    {
        //        Model.sys_member_roles_link l = new Model.sys_member_roles_link
        //        {
        //            mid = m.ID,
        //            rid = r.ID
        //        };
        //        lmr.Add(l);
        //    }
        //    _db.Insertable(lmr).ExecuteCommand();
        //    return true;
        //}

    }
}
