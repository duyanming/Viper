using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.BaseModel;

namespace Anno.Domain.Role
{
    public class FuncLink : Entity
    {
        public FuncLink()
        {
        }

        public FuncLink(long id)
        {
            this.ID = id;
        }

        public long Fid { get; protected set; }

        public long Rid { get; protected set; }

        public bool SetFuncLink(long fid,long rid)
        {
            this.Fid = fid;
            this.Rid = rid;
            return true;
        }

    }
}
