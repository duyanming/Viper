/****************************************************** 
Writer:Du YanMing
Mail:dym880@163.com
Create Date:2020/10/30 13:16:23 
Functional description： AnnoSoEasyModule
******************************************************/
using Anno.EngineData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anno.Plugs.SoEasyService
{
    using Anno.Const.Attribute;
    using Anno.EngineData;
    using HelloWorldDto;

    public class AnnoSoEasyModule : BaseModule
    {
        [AnnoInfo(Desc = "AnnoSoEasy你好啊SayHi")]
        public dynamic SayHi([AnnoInfo(Desc = "称呼")] string vname, [AnnoInfo(Desc = "年龄")] int vage)
        {
            var msg = string.Empty;
            if (vage < 12)
            {
                msg = "小朋友年纪轻轻就就开始玩编程了啊！加油Baby！";
            }else if (vage < 23)
            {
                msg = "小兄弟，找女朋友了吗？没有的话赶紧找一个吧。别把心思都放在写代码上！";
            }
            else if (vage < 30)
            {
                msg = "兄弟，你家小孩几岁了？开始学编程了吗？";
            }
            else if (vage < 45)
            {
                msg = "大哥，你好能给我介绍个对象吗？";
            }
            else if (vage < 55)
            {
                msg = "大叔，你家邻居有小妹妹介绍吗？";
            }
            else
            {
                msg = "还不退休？别写代码了！";
            }
            return $"{vname}:你好，我是SoEasy，{msg}";
        }

        [AnnoInfo(Desc = "两个整数相加等于几？我来帮你算")]
        public int Add([AnnoInfo(Desc = "整数X")] int x, [AnnoInfo(Desc = "整数Y")] int y)
        {
            return x + y;
        }
        [AnnoInfo(Desc = "买个商品吧，双十一马上就来了")]
        public ProductDto BuyProduct([AnnoInfo(Desc = "商品名称")] string productName, [AnnoInfo(Desc = "商品数量")] int number)
        {
            double price = new Random().Next(2, 90);
            return new ProductDto() { Name=productName,Price=price ,Number=number, CountryOfOrigin="中国台湾"};
        }
    }
}
