using Anno.Const;
using SqlSugar;
using System;

namespace Anno.Infrastructure
{
    /// <summary>
    /// 数据库实例
    /// </summary>
    public static class DbInstance
    {
        /// <summary>
        /// 数据库默认类型为MySql
        /// </summary>
        private static DbType DbType = DbType.MySql;
        static DbInstance()
        {
            if (CustomConfiguration.Settings.ContainsKey("DbType"))
            {
                DbType = (DbType)Enum.Parse(typeof(DbType), CustomConfiguration.Settings["DbType"]);
            }
        }
        /// <summary>
        /// SqlSugarClient
        /// </summary>
        public static SqlSugarClient Db => new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = AppSettings.ConnStr, //必填
            DbType = DbType, //必填
            IsAutoCloseConnection = true, //默认false
            InitKeyType = InitKeyType.SystemTable //默认SystemTable
        });
    }
}
