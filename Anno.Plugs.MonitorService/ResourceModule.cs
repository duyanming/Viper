/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/10/12 9:19:12 
Functional description： Resource
******************************************************/
using Anno.Const.Attribute;
using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.MonitorService
{
    public class ResourceModule : BaseModule
    {
        private static readonly EngineData.SysInfo.UseSysInfoWatch Usi = new EngineData.SysInfo.UseSysInfoWatch();
        /// <summary>
        /// 服务资源信息
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "服务资源信息CPU、Memory")]
        public ActionResult GetServerStatus()
        {
            return new ActionResult(true, Usi.GetServerStatus());
        }
    }
}
