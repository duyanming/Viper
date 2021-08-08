using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Anno.Log;

namespace Viper.Test
{
    using Anno.EngineData;
    using Anno.Loader;
    using Anno.Rpc.Client;
    using Anno.Rpc.Server;
    using Autofac;

    public class DLockTest
    {
        public static long counter = 0;
        public void Handle()
        {
            Init();
        To:
            List<Task> ts = new List<Task>();
            Console.WriteLine("请输入线程数:");
            int.TryParse(Console.ReadLine(), out int n);
            for (int i = 0; i < n; i++)
            {
                var task = Task.Factory.StartNew(() => { DLTest1("Viper"); });
                ts.Add(task);
                //var taskXX = Task.Factory.StartNew(() => { DLTest1("Anno"); });
                //ts.Add(taskXX);

                //var taskJJ = Task.Factory.StartNew(() => { DLTest1("DLock"); });
                //ts.Add(taskJJ);
            }

            Task.WaitAll(ts.ToArray());
            Console.WriteLine($"counter:{counter}");
            counter = 0;
            goto To;
        }

        private void DLTest1(string lk = "duyanming")
        {
            try
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss:ffff}  {System.Threading.Thread.CurrentThread.ManagedThreadId} DLTest1开始拉取锁({lk})");
                using (DLock dLock = new DLock(lk, 1000))
                {
                    counter = counter + 1;
                    Log.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId} DLTest1进入锁({lk})");
                    System.Threading.Thread.Sleep(50);
                    Log.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId} DLTest1离开锁进行中({lk})");
                }
                Log.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId} DLTest1离开锁({lk})");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        void Init()
        {
            IocLoader.GetAutoFacContainerBuilder().RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
            IocLoader.Build();
            DefaultConfigManager.SetDefaultConnectionPool(100, Environment.ProcessorCount * 2, 50);
            DefaultConfigManager.SetDefaultConfiguration("DLockTest", "127.0.0.1", 7010, false);
        }
    }
}
