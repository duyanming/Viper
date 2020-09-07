using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using SqlSugar;
using Anno.Domain.BaseModel;
using System.Linq;
using Anno.Infrastructure;

namespace Anno.Repository
{
    public class BaseRepository : IBaseQueryRepository
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public SqlSugarClient Context =>  DbInstance.Db;

        /// <summary>
        /// 获取类型实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return Loader.IocLoader.Resolve<T>();
        }
        /// <summary>
        /// 获取聚合跟
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual TRoot GetRoot<TRoot>(long id, string tableName = null) where TRoot : AggregateRoot, new()
        {
            var t = typeof(TRoot);
            TRoot tRoot = (TRoot)t.Assembly.CreateInstance(t.FullName);
            var sql = tableName == null ? Context.Queryable<TRoot>().Where(m => m.ID == id).ToSql() : Context.Queryable<TRoot>().AS(tableName).Where(m => m.ID == id).ToSql();
            var rooTable = Context.Ado.GetDataTable(sql.Key, sql.Value);
            List<PropertyInfo> targetProps = t.GetProperties().Where(p => p.CanWrite).ToList();
            if (targetProps.Count > 0 && rooTable.Rows.Count > 0)
            {
                var rows = rooTable.Rows[0];
                foreach (DataColumn col in rooTable.Columns)
                {
                    var targetProp = targetProps.Find(p =>
                        string.Equals(p.Name, col.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                    if (targetProp != null)
                    {
                        Type[] types = targetProp.PropertyType.GenericTypeArguments;
                        var value = rows[col.ColumnName];
                        if (types.Length > 0 && col.DataType.FullName.IndexOf("System", StringComparison.Ordinal) != -1)
                        {
                            targetProp.SetValue(tRoot, Convert.ChangeType(value, types[0]), null);
                        }
                        else
                        {
                            targetProp.SetValue(tRoot, value, null);
                        }
                    }
                }
            }
            return tRoot;
        }
    }
}
