using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Anno.Domain.Member;
using Anno.Domain.Repository;
using Anno.Model;

namespace Anno.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly IBaseQueryRepository _baseQueryRepository;
        private readonly MemoryCacheService _cacheService = new MemoryCacheService();
  
        public MemberRepository(IBaseQueryRepository baseQueryRepository)
        {
            this._baseQueryRepository = baseQueryRepository;
        }

        public SysMember GetById(long id)
        {
            var dataSet = _cacheService.Get<DataSet>(id);
            #region 从数据库读取状态
            if (dataSet == null)
            {
                StringBuilder querySql = new StringBuilder();
                querySql.AppendFormat("SELECT * FROM sys_member WHERE id=@id;");
                querySql.AppendFormat(@"SELECT r.* FROM sys_roles r 
                INNER JOIN sys_member_roles_link rl on rl.rid = r.id
                WHERE rl.mid = @id;");

                dataSet = _baseQueryRepository.Context.Ado.GetDataSetAll(querySql.ToString(), new { id });
            }
            #endregion
            #region DataSet 转化为实体
            var domain = Dtoer.Mapper<SysMember>(dataSet.Tables[0]).FirstOrDefault();

            var roles = Dtoer.Mapper<Role>(dataSet.Tables[1]);
            if (roles != null)
            {
                domain.Mapp(new { Roles = roles });
            } 
            #endregion
            #region 缓存
            _cacheService.SetChacheValue(id, dataSet);
            #endregion
            return domain;
        }

        public bool SaveChange(SysMember entity)
        {
            var snapshot = GetById(entity.ID);
            var rlt = _baseQueryRepository.Context.Ado.UseTran(() =>
            {
                var userTemp = new sys_member();
                userTemp.Mapp(entity);
                var success = _baseQueryRepository.Context.Updateable(userTemp)
                      .Where(it => it.ID == entity.ID)
                      .ExecuteCommand();//主表
                  snapshot.Roles.Where(r => !entity.Roles.Exists(rr => rr.ID == r.ID)).ToList().ForEach(r =>
                        {
                            _baseQueryRepository.Context.Ado.ExecuteCommand("delete from sys_member_roles_link where mid=@mid and rid=@rid;"
                                , new { mid = entity.ID, rid = r.ID });
                        });

                  entity.Roles.Where(r => !snapshot.Roles.Exists(rr => rr.ID == r.ID)).ToList().ForEach(r =>
                  {
                      var smrlink = new sys_member_roles_link()
                      {
                          ID = IdWorker.NextId(),
                          mid = entity.ID,
                          rid = r.ID
                      };
                      _baseQueryRepository.Context.Insertable(smrlink).ExecuteCommand();
                  });
              });
            return true;
        }

        public (bool success, string msg) AddMember(SysMember sysMember)
        {
            var rlt = _baseQueryRepository.Context.Ado.UseTran(() =>
            {
                var userTemp = new sys_member();
                userTemp.Mapp(sysMember);
                _baseQueryRepository.Context.Insertable(userTemp).ExecuteCommand();
                sysMember.Roles.ForEach(role =>
                {
                    var mrlink = new Model.sys_member_roles_link
                    {
                        ID = IdWorker.NextId(),
                        mid = userTemp.ID,
                        rid = role.ID
                    };
                    _baseQueryRepository.Context.Insertable(mrlink).ExecuteCommand();
                });
            });
            return (rlt.IsSuccess, rlt.IsSuccess ? null : rlt.ErrorMessage);
        }

        public bool Exist(string account)
        {
            return _baseQueryRepository.Context.Queryable<Model.sys_member>().Where(r => r.account == account.Trim()).Any();
        }
    }
}
