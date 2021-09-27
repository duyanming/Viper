using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.MonitorService
{
    /// <summary>
    /// 系统资源信息
    /// </summary>
    internal class SysWatchUtil
    {
        /// <summary>
        /// 系统资源信息获取
        /// </summary>
        internal static readonly EngineData.SysInfo.UseSysInfoWatch Usi = new EngineData.SysInfo.UseSysInfoWatch();
    }
}
