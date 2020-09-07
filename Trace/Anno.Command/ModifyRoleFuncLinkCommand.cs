using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Command
{
    public class ModifyRoleFuncLinkCommand : CommandBus.Command
    {
        public ModifyRoleFuncLinkCommand(long id, int version) : base(id, version)
        {
        }

        public List<long> FidList { get; set; } = new List<long>();
    }
}
