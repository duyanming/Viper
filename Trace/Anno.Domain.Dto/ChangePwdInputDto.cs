using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.Dto
{
    public class ChangePwdInputDto : BaseDto
    {
        public string pwd { get; set; }
        public string oldPwd { get; set; }
    }
}
