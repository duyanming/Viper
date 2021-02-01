using Anno.ViperLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.EngineData
{
    public class LogBaseModule : BaseModule
    {
        private readonly IViperLogService logService;
        public LogBaseModule() : base()
        {
            logService = this.Resolve<IViperLogService>();
        }
        protected void Info(object message, string title = null)
        {
            var traceId = this.RequestString("TraceId");
            var uName = this.RequestString("uname");
            logService.Info(message, traceId, uName, title);
        }

        protected void Warn(object message, string title = null)
        {
            var traceId = this.RequestString("TraceId");
            var uName = this.RequestString("uname");
            logService.Warn(message, traceId, uName, title);
        }
        protected void Error(object message, string title = null)
        {
            var traceId = this.RequestString("TraceId");
            var uName = this.RequestString("uname");
            logService.Error(message, traceId, uName, title);
        }
        protected void Fatal(object message, string title = null)
        {
            var traceId = this.RequestString("TraceId");
            var uName = this.RequestString("uname");
            logService.Fatal(message, traceId, uName, title);
        }
    }
}
