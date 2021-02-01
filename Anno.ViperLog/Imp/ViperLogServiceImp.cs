using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.ViperLog.Imp
{
    public class ViperLogServiceImp : IViperLogService
    {
        private readonly ILogService service;
        public ViperLogServiceImp(ILogService logService)
        {
            service = logService;
        }
        public void Error(object message, string traceId, string uName, string title = null)
        {
            var log = builderSysLog(message, traceId, uName, LogType.Error,title);
            service.LogBatch(new List<sys_log>() { log });
        }

        public void Fatal(object message, string traceId, string uName, string title = null)
        {
            var log = builderSysLog(message, traceId, uName, LogType.Fatal, title);
            service.LogBatch(new List<sys_log>() { log });
        }

        public void Info(object message, string traceId, string uName,  string title = null)
        {
            var log = builderSysLog(message, traceId, uName, LogType.Info, title);
            service.LogBatch(new List<sys_log>() { log });
        }

        public void Warn(object message, string traceId, string uName,  string title = null)
        {
            var log = builderSysLog(message, traceId, uName, LogType.Warn, title);
            service.LogBatch(new List<sys_log>() { log });
        }
        private sys_log builderSysLog(object message, string traceId, string uName, LogType logType, string title = null)
        {
            sys_log log = new sys_log();
            log.LogType = (int)logType;
            log.Content = LogMsg(message);
            log.AppName = Const.SettingService.AppName;
            log.Title = title;
            log.Uname = uName;
            log.TraceId = traceId;
            return log;
        }

        private string LogMsg(object logStr)
        {
            var msg = string.Empty;
            try
            {
                msg = Newtonsoft.Json.JsonConvert.SerializeObject(logStr);
            }
            catch
            {
                msg = logStr.ToString();
            }

            return msg;
        }
    }
}
