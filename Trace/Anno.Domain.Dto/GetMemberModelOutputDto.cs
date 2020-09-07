using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.Dto
{
    public class GetMemberModelOutputDto : BaseDto
    {
        /// <summary>
        /// 员工名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CName { get; set; }
        public string CCode { get; set; }
        public string CEmail { get; set; }
        public string CAddress { get; set; }
        public DateTime? CRdt { get; set; }

    }
}
