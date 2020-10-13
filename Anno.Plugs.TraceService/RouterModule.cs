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
    using Anno.Rpc;
    using Anno.Rpc.Storage;
    using Thrift.Protocol;
    using Thrift.Transport;

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
            TTransport transport = new TSocket(Const.SettingService.Local.IpAddress, Const.SettingService.Local.Port, 3000);
            try
            {
                TProtocol protocol = new TBinaryProtocol(transport);
                BrokerCenter.Client client = new BrokerCenter.Client(protocol);
                transport.Open();
                var microList = client.GetMicro(string.Empty);
                if (microList != null && microList.Count > 0)
                {
                    foreach (var service in microList)
                    {
                        if (serviceInformations.Any(it => it.Host == service.Ip && it.Port == service.Port))
                        {
                            continue;
                        }
                        ServiceInformation serviceInformation = new ServiceInformation();

                        serviceInformation.Tags = service.Name.Split(new string[] { "," }
                        , StringSplitOptions.RemoveEmptyEntries).Select(t => t.Substring(0, t.Length - 7)).ToList();
                        serviceInformation.Host = service.Ip;
                        serviceInformation.Port = service.Port;
                        serviceInformation.Timeout = service.Timeout;
                        serviceInformation.Weight = service.Weight;
                        serviceInformation.Nickname = service.Nickname;

                        serviceInformations.Add(serviceInformation);
                    }
                }
            }
            finally
            {
                if (transport.IsOpen)
                {
                    transport.Flush();
                    transport.Close();
                }
                transport.Dispose();
            }
            return new ActionResult(true, serviceInformations);
        }
    }
}
