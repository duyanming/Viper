using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Anno.EngineData;
using Anno.Log;

namespace Anno.Plugs.DLockService
{
    internal class LockerQueue
    {
        private readonly object _lock = new object();
        public LockerQueue()
        {
        }
        public LockInfo MLoker = new LockInfo();
        public ActionResult Enter(LockInfo info)
        {
            bool result = MLoker.ResetEvent.WaitOne(info.Time);
            if (result)
            {
                MLoker.EnterTime = DateTime.Now;
                MLoker.Time = info.Time;
                MLoker.Owner = info.Owner;
                MLoker.Type = ProcessType.Enter;
                return new ActionResult(true, null, null, "enter");
            }
            return new ActionResult(false, null, null, $"{info.Owner}/{info.Key} timeOut");
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Free()
        {
            lock (_lock)
            {
                if (MLoker.Type != ProcessType.Free)
                {
                    MLoker.EnterTime = DateTime.Now;
                    MLoker.Type = ProcessType.Free;
                    MLoker.ResetEvent.Set();
                }
            }

        }  /// <summary>
           /// 超时的时候才释放
           /// </summary>
        public void Detection()
        {
            lock (_lock)
            {
                if (MLoker.IsTimeOut && MLoker.Type != ProcessType.Free)
                {
                    MLoker.EnterTime = DateTime.Now;
                    MLoker.Type = ProcessType.Free;
                    MLoker.ResetEvent.Set();
                }
            }
        }
    }
}
