using System;

namespace Viper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Viper.Test";
            //new DLockTest().Handle();//分布式锁
            //new KvStorageTest().Handle();//AnnoCenter KV键值存取
            //new RpcTest().Handle();//Rpc 测试
            new RedisTest().Handle1();//Anno.Redis 测试
            Console.WriteLine("测试结束---------------------End");
            Console.ReadLine();
        }
    }
}
