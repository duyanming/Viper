using SqlSugar;

namespace Anno.Infrastructure
{
    /// <summary>
    /// 数据库实例
    /// </summary>
    public static class DbInstance
    {
        /// <summary>
        /// SqlSugarClient
        /// </summary>
        public static SqlSugarClient Db => new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = Const.AppSettings.ConnStr, //必填
            DbType = DbType.MySql, //必填
            IsAutoCloseConnection = true, //默认false
            InitKeyType = InitKeyType.SystemTable //默认SystemTable
        });
    }
}
