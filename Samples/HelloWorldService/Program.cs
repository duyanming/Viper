using System;

namespace HelloWorldService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Anno.Const.Attribute;
    using Anno.EngineData;
    using Anno.Loader;
    using Anno.Log;
    using Anno.Rpc.Server;
    using Anno.Rpc.Storage;
    using Autofac;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("-help"))
            {
                Log.ConsoleWriteLine(@"
启动参数：
	-p 6659		设置启动端口
	-xt 200		设置服务最大线程数
	-t 20000		设置超时时间（单位毫秒）
	-w 1		设置权重
	-h 192.168.0.2	设置服务在注册中心的地址
	-tr false		设置调用链追踪是否启用");
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
                ApiDoc();
            });
        }

        /// <summary>
        ///服务启动后将服务Api文档写入注册中心
        ///
        ///增加自己的服务的时候只用复制下面的代码就可以不用做修改
        /// </summary>
        static void ApiDoc()
        {
            List<AnnoData> routings = new List<AnnoData>();
            foreach (var item in Anno.EngineData.Routing.Routing.Router)
            {
                if (item.Value.RoutMethod.Name == "get_ActionResult")
                {
                    continue;
                }
                var parameters = item.Value.RoutMethod.GetParameters().ToList().Select(it =>
                {
                    var parameter = new ParametersValue
                    { Name = it.Name, Position = it.Position, ParameterType = it.ParameterType.FullName };
                    var pa = it.GetCustomAttributes<AnnoInfoAttribute>().ToList();
                    if (pa.Any())
                    {
                        parameter.Desc = pa.First().Desc;
                    }
                    return parameter;
                }).ToList();
                string methodDesc = String.Empty;
                var mAnnoInfoAttributes = item.Value.RoutMethod.GetCustomAttributes<AnnoInfoAttribute>().ToList();
                if (mAnnoInfoAttributes.Count > 0)
                {
                    methodDesc = mAnnoInfoAttributes.First().Desc;
                }
                routings.Add(new AnnoData()
                {
                    App = Anno.Const.SettingService.AppName,
                    Id = $"{Anno.Const.SettingService.AppName}:{item.Key}",
                    Value = Newtonsoft.Json.JsonConvert.SerializeObject(new DataValue { Desc = methodDesc, Name = item.Value.RoutMethod.Name, Parameters = parameters })
                });
            }
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add(CONST.Opt, CONST.DeleteByApp);
            input.Add(CONST.App, Anno.Const.SettingService.AppName);
            var del = Newtonsoft.Json.JsonConvert.DeserializeObject<AnnoDataResult>(StorageEngine.Invoke(input));
            if (del.Status == false)
            {
                Log.Error(del);
            }
            input.Clear();
            input.Add(CONST.Opt, CONST.UpsertBatch);
            input.Add(CONST.Data, Newtonsoft.Json.JsonConvert.SerializeObject(routings));
            var rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<AnnoDataResult>(StorageEngine.Invoke(input));
            if (rlt.Status == false)
            {
                Log.Error(rlt);
            }
        }
    }
}
