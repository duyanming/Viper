using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anno.Domain.BaseModel;

namespace Anno.Domain
{
    /*
    internal static class AggregateRootExtensions
    {
        #region 增删改
        /// <summary>
        /// 根据主键更新
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool Updateable<TAggregateRoot>(this TAggregateRoot aggregateRoot) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Updateable(aggregateRoot).WhereColumns(it => new { it.ID }).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 更新指定的数据列
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        internal static bool Updateable<TAggregateRoot>(this TAggregateRoot aggregateRoot, Expression<Func<TAggregateRoot, object>> columns) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Updateable(aggregateRoot).UpdateColumns(columns).WhereColumns(it => new { it.ID }).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 列表根据主键更新
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool Updateable<TAggregateRoot>(this List<TAggregateRoot> aggregateRoot) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Updateable(aggregateRoot).WhereColumns(it => new { it.ID }).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool Insertable<TAggregateRoot>(this TAggregateRoot aggregateRoot) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            aggregateRoot = repository.Context.Insertable(aggregateRoot).ExecuteReturnEntity();
            return true;
        }
        /// <summary>
        /// 插入指定的数据列
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        internal static bool Insertable<TAggregateRoot>(this TAggregateRoot aggregateRoot, Expression<Func<TAggregateRoot, object>> columns) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            aggregateRoot = repository.Context.Insertable(aggregateRoot).InsertColumns(columns).ExecuteReturnEntity();
            return true;
        }
        /// <summary>
        /// 列表插入数据库
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool Insertable<TAggregateRoot>(this List<TAggregateRoot> aggregateRoot) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            aggregateRoot.ForEach(t =>
            {
                t = repository.Context.Insertable(t).ExecuteReturnEntity();
            });
            return true;
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool Deleteable<TAggregateRoot>(this TAggregateRoot aggregateRoot) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Deleteable(aggregateRoot).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 根据表达式删除
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static bool Deleteable<TAggregateRoot>(this TAggregateRoot aggregateRoot
            , Expression<Func<TAggregateRoot, bool>> expression) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Deleteable(expression).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 根据实体批量删除
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool Deleteable<TAggregateRoot>(this List<TAggregateRoot> aggregateRoot) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Deleteable(aggregateRoot).ExecuteCommand() > 0;
        }
        /// <summary>
        /// 保存 数据库存在 更新 不存在 添加
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <returns></returns>
        internal static bool SaveRootable<TAggregateRoot>(this TAggregateRoot aggregateRoot) where TAggregateRoot : AggregateRoot, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            if (repository.Context.Queryable<TAggregateRoot>().Where(T => T.ID == aggregateRoot.ID).Any())
            {
                repository.Context.Updateable(aggregateRoot).ExecuteCommand();
            }
            else
            {
                repository.Context.Insertable(aggregateRoot).ExecuteCommand();
            }
            return true;
        }
        #endregion

        #region 查询
        /// <summary>
        /// 返回聚合
        /// </summary>
        /// <typeparam name="TAggregateRoot">聚合</typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression">条件查询</param>
        /// <returns></returns>
        internal static TAggregateRoot Queryable<TAggregateRoot>(this TAggregateRoot aggregateRoot, Expression<Func<TAggregateRoot, bool>> expression) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            aggregateRoot = repository.Context.Queryable<TAggregateRoot>().Where(expression).First();
            return aggregateRoot;
        }
        /// <summary>
        /// 记录是否存在
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static bool Any<TAggregateRoot>(this TAggregateRoot aggregateRoot, Expression<Func<TAggregateRoot, bool>> expression) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Queryable<TAggregateRoot>().Any(expression);
        }
        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression">字段</param>
        /// <param name="whereObj">条件</param>
        /// <returns></returns>
        internal static TResult Sum<TAggregateRoot, TResult>(this TAggregateRoot aggregateRoot, Expression<Func<TAggregateRoot, TResult>> expression, Expression<Func<TAggregateRoot, bool>> whereObj) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Queryable<TAggregateRoot>().Where(whereObj).Sum(expression);
        }
        /// <summary>
        /// 最大值
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        internal static TResult Max<TAggregateRoot, TResult>(this TAggregateRoot aggregateRoot
            , Expression<Func<TAggregateRoot, TResult>> expression
            , Expression<Func<TAggregateRoot, bool>> whereObj) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Queryable<TAggregateRoot>().Where(whereObj).Max(expression);
        }
        /// <summary>
        /// 最小值
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        internal static TResult Min<TAggregateRoot, TResult>(this TAggregateRoot aggregateRoot
           , Expression<Func<TAggregateRoot, TResult>> expression
           , Expression<Func<TAggregateRoot, bool>> whereObj) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Queryable<TAggregateRoot>().Where(whereObj).Min(expression);
        }
        /// <summary>
        /// 平均值
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="expression"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        internal static TResult Avg<TAggregateRoot, TResult>(this TAggregateRoot aggregateRoot
          , Expression<Func<TAggregateRoot, TResult>> expression
          , Expression<Func<TAggregateRoot, bool>> whereObj) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Queryable<TAggregateRoot>().Where(whereObj).Avg(expression);
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TAggregateRoot"></typeparam>
        /// <param name="aggregateRoot"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="OrderBy"></param>
        /// <param name="orderByType"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        internal static dynamic ToPageList<TAggregateRoot>(this TAggregateRoot aggregateRoot
            , int pageIndex
            , int pageSize
            , ref int totalCount
            , Expression<Func<TAggregateRoot, object>> OrderBy
            , SqlSugar.OrderByType orderByType
            , Expression<Func<TAggregateRoot, bool>> whereObj) where TAggregateRoot : class, IEntity, new()
        {
            Repository.IBaseRepository repository = new Repository.BaseRepository();
            return repository.Context.Queryable<TAggregateRoot>().Where(whereObj).OrderBy(OrderBy, orderByType).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        /// <summary>
        /// 获取聚合数据
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <param name="tRoot"></param>
        /// <param name="expression"></param>
        /// <returns>聚合对象</returns>
        public static TRoot GetData<TRoot>(this TRoot tRoot, Expression<Func<TRoot, bool>> expression) where TRoot : class, IEntity, new()
        {
            var db = new EngineData.BaseModule().db;
            var t = typeof(TRoot);
            if (tRoot == null)
            {
                tRoot = (TRoot)t.Assembly.CreateInstance(t.FullName);
            }
            var sql = db.Queryable<TRoot>().Where(expression).ToSql();
            var rooTable = db.Ado.GetDataTable(sql.Key, sql.Value);
            List<PropertyInfo> targetProps = t.GetProperties().Where(p => p.CanWrite).ToList();
            if(targetProps.Count > 0 && rooTable.Rows.Count > 0)
            {
                var rows = rooTable.Rows[0];
                foreach (DataColumn col in rooTable.Columns)
                {
                    var targetProp = targetProps.Find(p => p.Name.ToLower() == col.ColumnName.ToLower());
                    if (targetProp != null)
                    {
                        var types = targetProp.PropertyType.GenericTypeArguments;
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
            return  tRoot;
        }

        /// <summary>
        /// List  获取聚合数据
        /// </summary>
        /// <typeparam name="TRoot">聚合</typeparam>
        /// <param name="tList"></param>
        /// <param name="expression"></param>
        /// <returns>聚合列表</returns>
        public static List<TRoot> GetData<TRoot>(this List<TRoot> tList, Expression<Func<TRoot, bool>> expression) where TRoot : class, IEntity, new()
        {
            var db = new EngineData.BaseModule().db;
            var t = typeof(TRoot);
            if (tList == null)
            {
                tList = new List<TRoot>();
            }
            var sql = db.Queryable<TRoot>().Where(expression).ToSql();
            var rooTable = db.Ado.GetDataTable(sql.Key, sql.Value);
            List<PropertyInfo> targetProps = t.GetProperties().Where(p => p.CanWrite).ToList();
            if (targetProps.Count > 0 && rooTable.Rows.Count > 0)
            {
                foreach (DataRow row in rooTable.Rows)
                {
                    TRoot tRoot = new TRoot();
                    foreach (DataColumn col in rooTable.Columns)
                    {
                        var targetProp = targetProps.Find(p => p.Name.ToLower() == col.ColumnName.ToLower());
                        if (targetProp != null)
                        {
                            Type[] types = targetProp.PropertyType.GenericTypeArguments;
                            var value = row[col.ColumnName];
                            if (types.Length > 0 && col.DataType.FullName.IndexOf("System", StringComparison.Ordinal) != -1)
                            {
                                targetProp.SetValue(tRoot, Convert.ChangeType(value, types[0]), null);
                            }
                            else
                            {
                                targetProp.SetValue(tRoot, value, null);
                            }
                        }
                        tList.Add(tRoot);
                    }
                }
            }
            return tList;
        }
        #endregion
    }
    */
}
