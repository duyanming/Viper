using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Domain.Member
{
    public interface IMemberService
    {
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        bool ChangePwd(Dto.ChangePwdInputDto args);
    }
}
