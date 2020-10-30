/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/10/30 13:15:24 
Functional description： HelloWorldViperModule
******************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.HelloWorldService
{
    using Anno.Const.Attribute;
    using Anno.EngineData;
    using HelloWorldDto;

    public class HelloWorldViperModule: BaseModule
    {
        [AnnoInfo(Desc = "世界你好啊SayHi")]
        public dynamic SayHello([AnnoInfo(Desc = "称呼")] string name, [AnnoInfo(Desc = "年龄")] int age) {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("vName",name);
            input.Add("vAge", age.ToString());
            var soEasyMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<string>>(this.InvokeProcessor("Anno.Plugs.SoEasy", "AnnoSoEasy", "SayHi", input)).OutputData;
            return new { HelloWorldViperMsg = $"{name}你好啊，今年{age}岁了", SoEasyMsg= soEasyMsg };
        }

        [AnnoInfo(Desc = "两个整数相减等于几？我来帮你算（x-y=?）")]
        public int Subtraction([AnnoInfo(Desc = "整数X")] int x, [AnnoInfo(Desc = "整数Y")] int y)
        {
            return x - y;
        }
        [AnnoInfo(Desc = "买个商品吧，双十一马上就来了")]
        public ProductDto BuyProduct([AnnoInfo(Desc = "商品名称")] string productName, [AnnoInfo(Desc = "商品数量")] int number)
        {
            double price = new Random().Next(2, 90);
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add("productName", productName);
            input.Add("number", number.ToString());
            var product = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionResult<ProductDto>>(this.InvokeProcessor("Anno.Plugs.SoEasy", "AnnoSoEasy", "BuyProduct", input)).OutputData;
            product.CountryOfOrigin = $"中国北京中转--{ product.CountryOfOrigin}";
            return product;
        }
    }
}
