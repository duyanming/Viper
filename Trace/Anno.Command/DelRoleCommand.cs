using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command
{
    /// <summary>
    /// 删除系统角色
    /// </summary>
    public class DelRoleCommand : CommandBus.Command
    {
        public DelRoleCommand(long id, int version) : base(id, version)
        {

        }
    }
}
