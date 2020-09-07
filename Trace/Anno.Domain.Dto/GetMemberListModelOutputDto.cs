using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.Dto
{
    public class GetMemberListModelOutputDto : BaseDto
    {

        public string account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long coid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        public string position { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? rdt { get; set; }
    }
}
