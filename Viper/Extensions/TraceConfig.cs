using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTools;

namespace Viper.GetWay
{
    public class TraceConfig
    {
        public Target Target { get; set; } = new Target();
        public Limit Limit { get; set; } = new Limit();
    }

    public class Target
    {
        public string AppName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool TraceOnOff { get; set; }
    }

    public class Limit
    {
        public bool Enable { get; set; }
        public Taglimit[] TagLimits { get; set; }
        public Iplimit IpLimit { get; set; }
        public string[] White { get; set; }
        private List<IPAddressRange> policyWhite { get; set; }

        public List<IPAddressRange> PolicyWhite
        {
            get
            {
                if (White != null && White.Length > 0&&(policyWhite==null|| policyWhite.Count<=0))
                {
                    policyWhite=new List<IPAddressRange>();
                    foreach (var w in White)
                    {
                        policyWhite.Add(IPAddressRange.Parse(w));
                    }
                }
                return policyWhite;
            }
        }
        public string[] Black { get; set; }
        private List<IPAddressRange> policyBlack { get; set; }

        public List<IPAddressRange> PolicyBlack
        {
            get
            {
                if (Black != null && Black.Length > 0 && (policyBlack == null || policyBlack.Count <= 0))
                {
                    policyBlack = new List<IPAddressRange>();
                    foreach (var w in Black)
                    {
                        policyBlack.Add(IPAddressRange.Parse(w));
                    }
                }
                return policyBlack;
            }
        }
    }

    public class Taglimit : Iplimit
    {
        private string _channel = "*";
        private string _router = "*";
        public string channel
        {
            get { return _channel; }
            set
            {
                if (value == string.Empty || value == null)
                {
                    _channel = "*";
                }
                else
                {
                    _channel = value;
                }
            }
        }
        public string router
        {
            get { return _router; }
            set
            {
                if (value == string.Empty || value == null)
                {
                    _router = "*";
                }
                else
                {
                    _router = value;
                }
            }
        }
    }

    public class Iplimit
    {
        /// <summary>
        /// 时间片段
        /// </summary>
        public uint timeSpan { get; set; }
        /// <summary>
        /// 单位 时间片段 内允许通过的请求个数
        /// </summary>
        public uint rps { get; set; }
        /// <summary>
        /// 令牌桶大小
        /// </summary>
        public uint limitSize { get; set; } = 200;
    }

}
