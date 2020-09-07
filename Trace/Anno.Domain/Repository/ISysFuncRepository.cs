using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.Function;

namespace Anno.Domain.Repository
{
    public interface ISysFuncRepository : IRepository<SysFunc>
    {
        /// <summary>
        /// 移除系统功能
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        (Boolean success, string msg) Remove(long id);
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="sysFunc"></param>
        /// <returns></returns>
        (Boolean success, string msg) Add(SysFunc sysFunc);

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Exist(long id);
    }
}
