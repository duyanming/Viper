
using System;

namespace Anno.Model
{
    /// <summary>
    /// 实体类bif_company。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class bif_company : BaseModel.Entity
    {
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 企业类型
        /// </summary>
        public int? type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fax { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string zip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string website { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string person { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? state { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime? rdt { get; set; }
    }
}