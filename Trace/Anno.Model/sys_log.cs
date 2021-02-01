
using System;

namespace Anno.Model
{
    /// <summary>
    /// 实体类bif_log。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class sys_log : BaseModel.Entity
    {
        /// <summary>
        /// 调用链唯一标识
        /// </summary>
        public string TraceId { get; set; }
        /// <summary>
        /// 日志名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Uname { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public int? LogType { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime? timespan { get; set; }
    }
}