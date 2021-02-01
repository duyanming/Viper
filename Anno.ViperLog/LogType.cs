using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.ViperLog
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 重要
        /// </summary>
        Fatal,
    }
}
