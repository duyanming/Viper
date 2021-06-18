using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno.EngineData;
using Anno.Loader;
using Anno.Redis;
using Anno.Rpc.Client;
using Anno.Rpc.Server;
using Autofac;

namespace Viper.Test
{
    public class RedisTest
    {
        public void Handle()
        {
            #region RedisHelper 预热初始化资源
            Init();
            RedisHelper.Set("Anno", 0);
            var rlt = RedisHelper.Get<int>("Anno");
            #endregion
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10000; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using (DLock dLock = new DLock("Anno"))
                    {
                        var rlt = RedisHelper.Get<int>("Anno");
                        RedisHelper.Set("Anno", ++rlt);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            var endRlt = RedisHelper.Get<int>("Anno");
            Console.WriteLine(endRlt);
        }
        void Init()
        {

            IocLoader.GetAutoFacContainerBuilder().RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
            IocLoader.Build();
            DefaultConfigManager.SetDefaultConnectionPool(1000, Environment.ProcessorCount * 2, 50);
            DefaultConfigManager.SetDefaultConfiguration("DLockTest", "127.0.0.1", 7010, false);
        }
    }
}
