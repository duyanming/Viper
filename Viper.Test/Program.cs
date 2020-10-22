using System;

namespace Viper.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Viper.Test";
            new DLockTest().Handle();//分布式锁

            Console.WriteLine("测试结束---------------------End");
            Console.ReadLine();
        }
    }
}
