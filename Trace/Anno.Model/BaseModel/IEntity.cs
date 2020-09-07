using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Model.BaseModel
{
    public interface IEntity : IEntity<long>
    {       
    }
    public interface IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 唯一
        /// </summary>
        TPrimaryKey ID { get; set; }
        /// <summary>
        /// 是否为临时实体
        /// </summary>
        /// <returns></returns>
        bool IsTransient();
        /// <summary>
        /// 对象之间属性Copy
        /// </summary>
        /// <typeparam name="T">目标</typeparam>
        /// <typeparam name="T1">来源</typeparam>
        /// <param name="objSource">来源实例</param>
        /// <returns></returns>
        T Mapp<T, T1>(T1 objSource);
        /// <summary>
        /// 对象之间属性Copy
        /// </summary>
        /// <typeparam name="T">目标</typeparam>
        /// <typeparam name="T1">来源</typeparam>
        /// <param name="objSourceList">来源实例</param>
        /// <returns></returns>
        List<T> Mapp<T, T1>(List<T1> objSourceList);
    }
}
