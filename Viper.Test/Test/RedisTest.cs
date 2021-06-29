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

        public void Handle1()
        {
            List<User> users = new List<User>();
            users.Add(new User()
            {
                Id = 1,
                Name = "1",
                Age = 12,
                balance = 12.12M,
                Password = "sdfsdf"
            });
            users.Add(new User()
            {
                Id = 2,
                Name = "2",
                Age = 22,
                balance = 22.12M,
                Password = "2222"
            });
            users.Add(new User()
            {
                Id = 3,
                Name = "3",
                Age = 32,
                balance = 32.12M,
                Password = "333333"
            });
            RedisHelper.Set("users", users);

            var _users = RedisHelper.Get<List<User>>("users");

        }
        void Init()
        {

            IocLoader.GetAutoFacContainerBuilder().RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
            IocLoader.Build();
            DefaultConfigManager.SetDefaultConnectionPool(1000, Environment.ProcessorCount * 2, 50);
            DefaultConfigManager.SetDefaultConfiguration("DLockTest", "127.0.0.1", 7010, false);
        }
    }

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public decimal balance { get; set; }
    }
}
