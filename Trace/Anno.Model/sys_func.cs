
using System;

namespace Anno.Model
{
    /// <summary>
    /// 实体类sys_func。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class sys_func : BaseModel.Entity
    {
		/// <summary>
		/// 
		/// </summary>
		public string fname { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string fcode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public float forder { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public long? pid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string furl { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public short? show { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string icon { get; set; }
	}
}