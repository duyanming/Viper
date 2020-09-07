using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.BaseModel;

namespace Anno.Domain.Function
{
    public class SysFunc : AggregateRoot
    {
        public SysFunc()
        {
        }

        public SysFunc(long id)
        {
            this.ID = id;
        }

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string Fname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Fcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public float Forder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? Pid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public short? Show { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        #endregion

    }
}
