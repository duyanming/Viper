using Anno.CronNET;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore
{
    public static class UtilJob
    {
        /// <summary>
        /// 需要执行Gc任务
        /// </summary>
        internal static volatile bool NeedGc = false;
        /// <summary>
        /// 每个10秒检测一遍服务
        /// </summary>
        internal static CronDaemon cron_daemon = new CronDaemon();
        /// <summary>
        /// 启动任务
        /// </summary>
        internal static void Start()
        {
            lock (cron_daemon)
            {
                cron_daemon.Start();
            }
        }
        /// <summary>
        /// 停止任务
        /// </summary>
        internal static void Stop()
        {
            lock (cron_daemon)
            {
                cron_daemon.Stop();
            }
        }

        /// <summary>
        /// 根据信号GC
        /// </summary>
        internal static void GcTask()
        {
            if (NeedGc)
            {
                GC.Collect();
                Task.Delay(1000).Wait();
                GC.Collect();
                NeedGc = false;
            }
        }
    }
}
