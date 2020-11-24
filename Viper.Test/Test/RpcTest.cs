using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viper.Test
{
    using Anno.Rpc.Client;
    public class RpcTest
    {
        public void Handle()
        {
            Init();
            Dictionary<string, string> input = new Dictionary<string, string>();

            input.Add("channel", "Anno.Plugs.HelloWorld");
            input.Add("router", "HelloWorldViper");
            input.Add("method", "BuyProduct");

            input.Add("productName", "手机");

            input.Add("number", "3");
            var rlt = Connector.BrokerDns(input);
            Console.WriteLine(rlt);
        }
        void Init()
        {
            DefaultConfigManager.SetDefaultConfiguration("RpcTest", "127.0.0.1", 7010, true);
        }
    }
}
