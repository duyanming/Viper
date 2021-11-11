using Anno.Rpc.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNetCore
{
    public static class UtilService
    {
        public static List<ServiceInformation> GetServiceInstances()
        {
            List<ServiceInformation> serviceInformations = new List<ServiceInformation>();
            try
            {
                var micros = Anno.Rpc.Client.Connector.Micros;
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
            return serviceInformations;
        }
        public static List<DeployAgent> GetDeployServices()
        {
            List<DeployAgent> deployAgents = new List<DeployAgent>();
            var microList = Anno.Rpc.Client.Connector.Micros.Where(m => m.Tags.Contains("Anno.Plugs.Deploy")).Select(m => m.Mi).ToList();
            if (microList != null && microList.Count > 0)
            {
                foreach (var service in microList)
                {
                    if (deployAgents.Any(it => it.Host == service.Ip && it.Port == service.Port))
                    {
                        continue;
                    }
                    DeployAgent deployAgent = new DeployAgent();
                    deployAgent.Host = service.Ip;
                    deployAgent.Port = service.Port;
                    deployAgent.Timeout = service.Timeout;
                    deployAgent.Weight = service.Weight;
                    deployAgent.Nickname = service.Nickname;

                    deployAgents.Add(deployAgent);
                }
            }
            return deployAgents;
        }
    }

    public class ServiceInformation
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 功能列表
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 超时时间毫秒
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 服务权重
        /// </summary>
        public int Weight { get; set; }

    }
    public class RoutingInfomationDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public Boolean Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<AnnoDataObj> Data { get; set; }
    }

    public class AnnoDataObj
    {
        /// <summary>
        /// 来自哪个App
        /// </summary>
        public string App { get; set; }
        /// <summary>
        /// 键(Key)
        /// </summary>
        public string Id { get; set; }

        public string Channel
        {
            get
            {
                string channel = String.Empty;
                if (!String.IsNullOrWhiteSpace(Id))
                {
                    var idInfo = Id.Split(':');
                    if (idInfo.Length == 2)
                    {
                        channel = idInfo[1].Split('/')[0];
                        int index = channel.LastIndexOf('.');
                        if (index > -1)
                        {
                            channel = channel.Substring(0, index);
                        }
                    }
                }
                return channel;
            }
        }

        public string Router
        {
            get
            {
                string router = String.Empty;
                if (!String.IsNullOrWhiteSpace(Id))
                {
                    var idInfo = Id.Split(':');
                    if (idInfo.Length == 2)
                    {
                        router = idInfo[1].Split('/')[0];
                        int index = router.LastIndexOf('.');
                        if (index > -1)
                        {
                            router = router.Substring(index + 1);
                        }
                    }
                }
                return router;
            }
        }

        public string Method
        {
            get
            {
                string method = String.Empty;
                if (!String.IsNullOrWhiteSpace(Id))
                {
                    var idInfo = Id.Split('/');
                    if (idInfo.Length == 2)
                    {
                        method = idInfo[1];
                    }
                }
                return method;
            }
        }

        public string Desc
        {
            get
            {
                if (ValueObj != null)
                {
                    return ValueObj.Desc;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        public DataValue ValueObj
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Value))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<DataValue>(this.Value);
                }
                return null;
            }
        }
        public string Value { get; set; }
    }

    public class AnnoDataOutPut
    {
        /// <summary>
        /// 来自哪个App
        /// </summary>
        public string App { get; set; }

        public string Channel { get; set; }
        public string Router { get; set; }
        public string Method { get; set; }

        public string Desc { get; set; }
        public DataValue Value { get; set; }
    }


    public class DeployAgent
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 超时时间毫秒
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 服务权重
        /// </summary>
        public int Weight { get; set; }
    }
}
