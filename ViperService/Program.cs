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
            Bootstrap.StartUp(args, () =>
            {
                //Anno.Const.SettingService.TraceOnOff = true;
                var autofac = IocLoader.GetAutoFacContainerBuilder();
                autofac.RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
            }, () =>
            { //startUp  CallBack
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
                    Anno.Log.Log.Error(del);
                }
                input.Clear();
                input.Add(CONST.Opt, CONST.UpsertBatch);
                input.Add(CONST.Data, Newtonsoft.Json.JsonConvert.SerializeObject(routings));
                var rlt = Newtonsoft.Json.JsonConvert.DeserializeObject<AnnoDataResult>(StorageEngine.Invoke(input));
                if (rlt.Status == false)
                {
                    Anno.Log.Log.Error(rlt);
                }
            });
        }
    }
}
