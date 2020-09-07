using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.Repository
{
    /// <summary>
    /// 领域模型仓储基础类
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    public interface IRepository<TDomain>
    {
        TDomain GetById(long id);

        bool SaveChange(TDomain entity);
    }
}
