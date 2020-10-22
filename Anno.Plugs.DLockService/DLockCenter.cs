using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Anno.Plugs.DLockService
{
    public static class DLockCenter
    {
        private static List<LockerQueue> _lockerQueues = new List<LockerQueue>();
        private static readonly object Lock = new object();

        public static EngineData.ActionResult Enter(LockInfo info)
        {
            var locker = _lockerQueues.Find(l => l.MLoker.Key == info.Key);
            if (locker == null)
            {
                lock (Lock)//不同锁共锁---有问题Q
                {
                    locker = _lockerQueues.Find(l => l.MLoker.Key == info.Key);
                    if (locker == null)
                    {
                        locker = new LockerQueue
                        {
                            MLoker =
                            {
                                Owner = info.Owner,
                                Type = info.Type,
                                Key = info.Key,
                                Time = info.Time,
                                EnterTime = DateTime.Now
                            }
                        };
                        _lockerQueues.Add(locker); 
                    }
                }
            }
            return locker.Enter(info);
        }
        public static void Free(LockInfo info)
        {
            _lockerQueues.Find(l => l.MLoker.Owner == info.Owner)?.Free();
        }
        /// <summary>
        ///  超时直接抛弃
        /// </summary>
        public static void Detection()
        {
            if (_lockerQueues.Count > 0)
            {
                _lockerQueues.Where(l=>l.MLoker.IsTimeOut&&l.MLoker.Type!=ProcessType.Free).ToList().ForEach(l=>l.Detection());
            }

        }
    }
}
