using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Anno.Domain.Repository;
using Anno.Domain.Role;
using Anno.Model;

namespace Anno.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IBaseQueryRepository _baseQueryRepository;
        private readonly MemoryCacheService _cacheService = new MemoryCacheService();

        public RoleRepository(IBaseQueryRepository baseQueryRepository)
        {
            this._baseQueryRepository = baseQueryRepository;
        }

        public Role GetById(long id)
        {
            var dataSet = _cacheService.Get<DataSet>(id);
            #region 从库读取数据
            if (dataSet == null)
            {
                StringBuilder querySql = new StringBuilder();
                querySql.AppendFormat($"SELECT * FROM sys_roles WHERE id=@id;");
                querySql.AppendFormat(@"SELECT l.* FROM   sys_func_roles_link l WHERE l.rid=@id; ");
                dataSet = _baseQueryRepository.Context.Ado.GetDataSetAll(querySql.ToString(), new { id });
            }
            #endregion
            #region DataSet 转化为实体
            var domain = Dtoer.Mapper<Role>(dataSet.Tables[0]).FirstOrDefault();
            var func = Dtoer.Mapper<FuncLink>(dataSet.Tables[1]);
            if (func != null)
            {
                domain.Mapp(new { Funcs = func });
            }
            #endregion
            #region 缓存
            _cacheService.SetChacheValue(id, dataSet);
            #endregion
            return domain;
        }

        public bool SaveChange(Role entity)
        {
            var snapshot = GetById(entity.ID);
            var rlt = _baseQueryRepository.Context.Ado.UseTran(() =>
            {
                var role = new sys_roles();
                role.Mapp(entity);
                _baseQueryRepository.Context.Updateable(role).Where(r => r.ID == entity.ID)
                    .ExecuteCommand();
                snapshot.Funcs.Where(f => !entity.Funcs.Exists(ff => ff.Fid == f.Fid)).ToList().ForEach(f =>
                      {
                          _baseQueryRepository.Context.Ado.ExecuteCommand("delete from sys_func_roles_link where fid=@fid and rid=@rid;"
                              , new { rid = entity.ID, fid = f.Fid });
                      });
                entity.Funcs.Where(f => !snapshot.Funcs.Exists(ff => ff.Fid == f.Fid)).ToList().ForEach(f =>
                {
                    var frl = new sys_func_roles_link();
                    frl.Mapp(f);
                    _baseQueryRepository.Context.Insertable(frl).ExecuteCommand();
                });
            });
            return rlt.IsSuccess;
        }

        public bool Exist(long id)
        {
            return _baseQueryRepository.Context.Queryable<sys_roles>().Where(r => r.ID == id).Any();
        }

        public bool Exist(string roleName)
        {
            return _baseQueryRepository.Context.Queryable<sys_roles>().Where(r => r.name == roleName.Trim()).Any();
        }

        public (Boolean success, string msg) Add(Role role)
        {
            if (!Exist(role.Name))
            {
                var _role = new sys_roles();
                _role.Mapp(role);
                _baseQueryRepository.Context.Insertable(_role).ExecuteReturnIdentity();
            }
            else
            {
                role.ID = -1;
                return (false, $"角色【{role.Name}】已经存在");
            }
            return (true, null);
        }

        public (bool success, string msg) Remove(long id)
        {
            var success = _baseQueryRepository.Context.Ado.UseTran(() =>
            {
                _baseQueryRepository.Context.Deleteable<sys_roles>().Where(it => it.ID == id).ExecuteCommand();
                _baseQueryRepository.Context.Ado.ExecuteCommand("DELETE from sys_func_roles_link WHERE rid=@rid;", new { rid = id });
            });
            if (!success.IsSuccess)
            {
                return (false, "要删除的角色不存在！");
            }
            return (true, null);
        }
    }
}
