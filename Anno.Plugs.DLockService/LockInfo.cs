using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Anno.Plugs.DLockService
{
    public class LockInfo
    {
        public string Owner { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// 进入时间
        /// </summary>
        public DateTime EnterTime { get; set; } = DateTime.Now;

        public int Time { get; set; }
        public AutoResetEvent ResetEvent { get; set; } = new AutoResetEvent(true);

        /// <summary>
        /// 是否已经超时
        /// </summary>
        public bool IsTimeOut => (DateTime.Now-EnterTime).TotalMilliseconds > Time;

        public ProcessType Type { get; set; }
    }
}
