/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/7/3 16:36:39 
Functional description： RouterModule
******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anno.Const.Attribute;
using Anno.EngineData;

namespace Anno.Plugs.TraceService
{
    using Anno.Rpc.Storage;

    public class RouterModule : BaseModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取服务路由信息")]
        public ActionResult GetRoutingInfo([AnnoInfo(Desc = "服务名称")] string appName)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add(StorageCommand.COMMAND, StorageCommand.APIDOCCOMMAND);
            input.Add(CONST.Opt, CONST.FindByApp);
            input.Add(CONST.App, appName);
            string rlt = StorageEngine.Invoke(input);
            List<AnnoDataOutPut> annoDataOutPuts = new List<AnnoDataOutPut>();
            RoutingInfomationDto routing = Newtonsoft.Json.JsonConvert.DeserializeObject<RoutingInfomationDto>(rlt);
            if (routing.Data != null && routing.Data.Count > 0)
            {
                routing.Data.ForEach(d =>
                {
                    annoDataOutPuts.Add(new AnnoDataOutPut()
                    {
                        App = d.App,
                        Channel = d.Channel,
                        Router = d.Router,
                        Method = d.Method,
                        Desc = d.ValueObj.Desc,
                        Value = d.ValueObj
                    });

                });
            }
            return new ActionResult(true, annoDataOutPuts.OrderBy(d => d.Channel).ThenBy(d => d.Router).ThenBy(d => d.Method));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AnnoInfo(Desc = "获取服务实例集合")]
        public ActionResult GetServiceInstances()
        {
            List<ServiceInformation> serviceInformations = new List<ServiceInformation>();
            try
            {
                var micros = Rpc.Client.Connector.Micros;
                if (micros != null && micros.Count > 0)
                {
                    foreach (var service in micros)
                    {
                        if (serviceInformations.Any(it => it.Host == service.Mi.Ip && it.Port == service.Mi.Port))
                        {
                            continue;
                        }
                        ServiceInformation serviceInformation = new ServiceInformation();

                        serviceInformation.Tags = service.Tags;
                        serviceInformation.Host = service.Mi.Ip;
                        serviceInformation.Port = service.Mi.Port;
                        serviceInformation.Timeout = service.Mi.Timeout;
                        serviceInformation.Weight = service.Mi.Weight;
                        serviceInformation.Nickname = service.Mi.Nickname;

                        serviceInformations.Add(serviceInformation);
                    }
                }
            }
            catch { }
            return new ActionResult(true, serviceInformations);
        }
    }
}
