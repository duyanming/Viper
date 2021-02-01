using Anno.EngineData;
using Anno.Rpc.Client.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.ViperLog
{
    [AnnoProxy(Channel = "Anno.Plugs.Trace", Router = "Trace")]
    public interface ILogService
    {
        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        [AnnoProxy(Method = "LogBatch")]
        ActionResult LogBatch(List<sys_log> logs);
    }
}
