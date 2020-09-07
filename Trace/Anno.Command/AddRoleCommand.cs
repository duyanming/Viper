using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command
{
    /// <summary>
    /// 添加系统角色
    /// </summary>
    public class AddRoleCommand : CommandBus.Command
    {
        public AddRoleCommand(string name,long id, int version) : base(id, version)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
