using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Anno.Const.Attribute;
using Anno.Loader;
using Anno.Log;
using Autofac;

namespace ViperService
{
    using Anno.EngineData;
    using Anno.Rpc.Server;
    using System.Collections.Generic;
    using Anno.Rpc.Storage;
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("-help"))
            {
                Log.WriteLineNoDate(@"
启动参数：
                -p   6659                    设置启动端口
                -xt  200                     设置服务最大线程数
                -t   20000                   设置超时时间（单位毫秒）
                -w   1                       设置权重
                -h   192.168.0.2             设置服务在注册中心的地址
                -tr  false                   设置调用链追踪是否启用
");
                return;
            }
            /**
             * 启动默认DI库为 Autofac 可以切换为微软自带的DI库 DependencyInjection
             */
            Bootstrap.StartUp(args, () =>//服务配置文件读取完成后回调(服务未启动)
            {
                //Anno.Const.SettingService.TraceOnOff = true;

                /*
                 * 功能插件选择是Thrift还是 Grpc
                 * Install-Package Anno.Rpc.Client -Version 1.0.2.6 Thrift
                 * Install-Package Anno.Rpc.ServerGrpc -Version 1.0.1.5 Grpc
                 * 此处为 Thrift
                 */
                var autofac = IocLoader.GetAutoFacContainerBuilder();
                autofac.RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
            }
            , () =>//服务启动后的回调方法
            {
                /**
                 * 服务Api文档写入注册中心
                 */
                Bootstrap.ApiDoc();           
            });
        }
    }
}
