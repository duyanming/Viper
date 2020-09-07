using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Anno.Domain.Function;
using Anno.Domain.Repository;
using Anno.Model;

namespace Anno.Repository
{
    public class SysFuncRepository : ISysFuncRepository
    {
        private readonly IBaseQueryRepository _baseQueryRepository;
        private readonly MemoryCacheService _cacheService = new MemoryCacheService();

        public SysFuncRepository(IBaseQueryRepository baseQueryRepository)
        {
            _baseQueryRepository = baseQueryRepository;
        }

        public (bool success, string msg) Add(SysFunc sysFunc)
        {
            var func = new sys_func();
            func.Mapp(sysFunc);
            var success = _baseQueryRepository.Context.Insertable(func).ExecuteCommand() > 0;
            return (success, success ? null : "添加失败！");
        }
        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(long id)
        {
            return _baseQueryRepository.Context.Queryable<sys_func>().Where(r => r.pid == id).Any();
        }

        public SysFunc GetById(long id)
        {
            var dataSet = _cacheService.Get<DataSet>(id);
            #region 从库读取数据
            if (dataSet == null)
            {
                StringBuilder querySql = new StringBuilder();
                querySql.AppendFormat($"SELECT * FROM sys_func WHERE id=@id;");
                dataSet = _baseQueryRepository.Context.Ado.GetDataSetAll(querySql.ToString(), new { id });
            }
            #endregion
            #region DataSet 转化为实体
            var domain = Dtoer.Mapper<SysFunc>(dataSet.Tables[0]).FirstOrDefault();
            #endregion
            #region 缓存
            _cacheService.SetChacheValue(id, dataSet);
            #endregion
            return domain;
        }

        public (bool success, string msg) Remove(long id)
        {
            if (Exist(id))
            {
                return (false, "请先删除子节点！");
            }
            else
            {
                _baseQueryRepository.Context.Deleteable<sys_func>().Where(it => it.ID == id).ExecuteCommand();
            }
            return (true, null);
        }

        public bool SaveChange(SysFunc entity)
        {
            var func = new sys_func();
            func.Mapp(entity);
            return _baseQueryRepository.Context.Updateable(func).Where(r => r.ID == entity.ID).ExecuteCommand() > 0;
        }
    }
}
