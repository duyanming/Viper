using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.BaseModel;

namespace Anno.Domain.Member
{
    /// <summary>
    /// 用户的角色
    /// </summary>
    public class Role : Entity
    {
        public Role()
        {
        }

        public Role(long id, string name)
        {

        }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string name { get; private set; }
    }
}
