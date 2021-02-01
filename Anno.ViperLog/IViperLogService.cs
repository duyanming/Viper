using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.ViperLog
{
    public interface IViperLogService
    {
        void Info(object message, string traceId, string uName, string title = null);

        void Warn(object message, string traceId, string uName, string title = null);
        void Error(object message, string traceId, string uName, string title = null);
        void Fatal(object message, string traceId, string uName, string title = null);
    }
}
