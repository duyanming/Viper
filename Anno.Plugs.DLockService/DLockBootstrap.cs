using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.DLockService
{
    using CronNET;
    using EngineData;

    public class DLockBootstrap: IPlugsConfigurationBootstrap
    {
        private static readonly CronDaemon CronDaemon = new CronDaemon();
        public void ConfigurationBootstrap()
        {
            //分布式锁启动配置
            /*
             * 每个一段时间检测是否有锁超时，超时则释放锁
             */
            CronDaemon.AddJob("* * * * * ? *", DLockCenter.Detection);
            if (CronDaemon.Status == DaemonStatus.Stop)
            {
                CronDaemon.Start();
            }
        }

        public void PreConfigurationBootstrap()
        {
            //throw new NotImplementedException();
        }
    }
}
