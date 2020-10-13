/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/10/13 16:29:58 
Functional description： ServiceInformation
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.TraceService
{
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
}
