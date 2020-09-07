
using System;

namespace Anno.Model
{
    /// <summary>
    /// 实体类sys_member。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class sys_member : BaseModel.Entity
    {
        /// <summary>
        /// 
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string pwd { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public long coid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// _1 启用 0 禁用
        /// </summary>
        public short? state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? timespan { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? rdt { get; set; }
    }
}