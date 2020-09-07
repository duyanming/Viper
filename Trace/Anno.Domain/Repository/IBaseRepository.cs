using System;
using System.Collections.Generic;

namespace Anno.Domain.Repository
{
    public interface IBaseRepository
    {
        /// <summary>
        /// Ioc 获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();
        /// <summary>
        /// 获取聚合根数据
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <param name="id"></param>
        /// <param name="TableName">表明，NULL的时候 取默认</param>
        /// <returns></returns>
        TRoot GetRoot<TRoot>(long id,string TableName=null)where TRoot : BaseModel.AggregateRoot, new();
    }
}
