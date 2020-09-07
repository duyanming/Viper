using System;
using System.Collections.Generic;
using System.Text;
using Anno.Domain.BaseModel;

namespace Anno.Domain.Role
{
    public class Role:AggregateRoot
    {
        public Role()
        {
        }

        public Role(long id)
        {
            this.ID = id;
        }

        #region 属性
        public string Name { get; protected set; }
        public List<FuncLink> Funcs { get; protected set; }=new List<FuncLink>();

        #endregion
        #region 业务
        public  (bool success,string msg)Add(string name)
        {
            if (name == null || name.Trim() == string.Empty)
            {
                return (false, "名称不能为空！");
            }

            if (name.Length < 3 || name.Length > 15)
            {
                return (false,"角色名称长度必须在3-15之间！");
            }
            this.Name = name;
            return (true,null);
        }

        public bool RemoveFunc(long fid)
        {
            Funcs.RemoveAll(f => f.Fid == fid);
            return true;
        }
        public bool RemoveAllFunc()
        {
            Funcs=new List<FuncLink>();
            return true;
        }
        public bool AddFunc(long fid)
        {

            if (!Funcs.Exists(f => f.Fid == fid))
            {
                var flink = new FuncLink(IdWorker.NextId());
                flink.SetFuncLink(fid, this.ID);
                Funcs.Add(flink);
            }
            return true;
        }
        #endregion

    }
}
