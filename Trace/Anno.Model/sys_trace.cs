
using System;

namespace Anno.Model
{
    /// <summary>
    /// 实体类sys_trace。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class sys_trace : BaseModel.Entity
    {
        /// <summary>
        /// 调用链全局唯一标识
        /// </summary>
        public string GlobalTraceId { get; set; }
        /// <summary>
        /// 调用链唯一标识
        /// </summary>
        public string TraceId { get; set; }
		/// <summary>
		/// 上级调用链唯一标识
		/// </summary>
		public string PreTraceId { get; set; }
		/// <summary>
		/// App名称
		/// </summary>
		public string AppName { get; set; }
        /// <summary>
        /// 目标App名称
        /// </summary>
        public string AppNameTarget { get; set; }
        /// <summary>
        /// 跳转次数
        /// </summary>
        public int? TTL { get; set; }
		/// <summary>
		/// 请求参数
		/// </summary>
		public string Request { get; set; }
		/// <summary>
		/// 响应参数
		/// </summary>
		public string Response { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>
        public bool Rlt { get; set; }
        /// <summary>
        /// 操作人IP
        /// </summary>
        public string Ip { get; set; }
		/// <summary>
		/// 目标地址
		/// </summary>
		public string Target { get; set; }
		/// <summary>
		/// 操作人ID
		/// </summary>
		public long? UserId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string Uname { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime? Timespan { get; set; }
		/// <summary>
		/// 请求管道
		/// </summary>
		public string Askchannel { get; set; }
		/// <summary>
		/// 请求路由
		/// </summary>
		public string Askrouter { get; set; }
		/// <summary>
		/// 业务方法
		/// </summary>
		public string Askmethod { get; set; }
        /// <summary>
        /// 耗时单位毫秒
        /// </summary>
        public double UseTimeMs { get; set; }
    }
}