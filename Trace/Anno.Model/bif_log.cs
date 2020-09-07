
using System;

namespace Anno.Model
{
    /// <summary>
    /// 实体类bif_log。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class bif_log : BaseModel.Entity
    {
		/// <summary>
		/// 日志名称
		/// </summary>
		public string title { get; set; }
		/// <summary>
		/// 操作人
		/// </summary>
		public long? user { get; set; }
		/// <summary>
		/// 操作类型
		/// </summary>
		public int? type { get; set; }
		/// <summary>
		/// 操作人IP
		/// </summary>
		public string ip { get; set; }
		/// <summary>
		/// 操作内容
		/// </summary>
		public string content { get; set; }
		/// <summary>
		/// 记录时间
		/// </summary>
		public DateTime? timespan { get; set; }
	}
}