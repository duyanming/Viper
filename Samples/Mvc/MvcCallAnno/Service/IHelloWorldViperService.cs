using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCallAnno.Service
{
    using Anno.Rpc.Client.DynamicProxy;

    /// <summary>
    /// 对应Anno.Plugs.HelloWorldService 插件的 HelloWorldViperModule 模块
    /// 接口名称和接口方法和 AnnoService端的 名称不一样的时候使用AnnoProxy 指定别名
    /// </summary>
    [AnnoProxy(Channel = "Anno.Plugs.HelloWorld", Router = "HelloWorldViper")]
    public interface IHelloWorldViperService
    {
        /// <summary>
        /// 名称不一致
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        [AnnoProxy(Method = "SayHello")]
        dynamic SayHello(string name, int age);
        /// <summary>
        /// 名称一致
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int Subtraction(int x, int y);

        ProductDto BuyProduct(string productName, int number);
    }


    public class ProductDto
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public double Price { get; set; }
        public double Amount { get { return Price * Number; } }
        public string CountryOfOrigin { get; set; }
    }
}
