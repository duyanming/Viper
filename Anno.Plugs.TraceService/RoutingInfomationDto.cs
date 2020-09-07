/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/7/3 16:57:37 
Functional description： RoutingInfomationDto
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using Anno.Rpc.Storage;

namespace Anno.Plugs.TraceService
{
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
                            channel = channel.Substring(0,index);
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
                            router = router.Substring(index+1);
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
        public DataValue ValueObj {
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
}
