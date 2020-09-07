using System;
using System.Collections.Generic;
using Anno.Domain.Repository;
using SqlSugar;

namespace Anno.Repository
{
    public interface IBaseQueryRepository:IBaseRepository
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        SqlSugarClient Context { get;}
    }
}
