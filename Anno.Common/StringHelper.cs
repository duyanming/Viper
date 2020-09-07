using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Anno.Common
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// 获取Guid字符串值
        /// </summary>
        /// <returns></returns>
        public static string GetGuidString()
        {
            /*
                Guid.NewGuid().ToString("N") 结果为：
                   38bddf48f43c48588e0d78761eaa1ce6
                Guid.NewGuid().ToString("D") 结果为：
                   57d99d89-caab-482a-a0e9-a0a803eed3ba
                Guid.NewGuid().ToString("B") 结果为：
                   {09f140d5-af72-44ba-a763-c861304b46f8}
                Guid.NewGuid().ToString("P") 结果为：
                   (778406c2-efff-4262-ab03-70a77d09c2b5)
             */
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 删除字符串结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar, StringComparison.Ordinal));
        }

        /// <summary>
        /// 把字符串按照指定分隔符装成 List 去除重复
        /// </summary>
        /// <param name="oStr"></param>
        /// <param name="sepeater"></param>
        /// <returns></returns>
        public static List<string> GetSubStringList(string oStr, char sepeater)
        {
            var ss = oStr.Split(sepeater);
            return ss.Where(s => !string.IsNullOrEmpty(s) && s != sepeater.ToString(CultureInfo.InvariantCulture)).ToList();
        }

        /// <summary>
        /// 得到字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString">参数字符串</param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            var ascii = new ASCIIEncoding();
            var tempLen = 0;
            var s = ascii.GetBytes(inputString);
            foreach (var t in s)
            {
                if (t == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }

        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <param name="len">指定长度</param>
        /// <returns>返回处理后的字符串</returns>
        public static string CutString(string inputString, int len)
        {
            var isShowFix = false;
            if (len % 2 == 1)
            {
                isShowFix = true;
                len--;
            }
            var ascii = new ASCIIEncoding();
            var tempLen = 0;
            var tempString = "";
            var s = ascii.GetBytes(inputString);
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }

            var mybyte = Encoding.Default.GetBytes(inputString);
            if (isShowFix && mybyte.Length > len)
                tempString += "…";
            return tempString;
        }

        /// <summary>
        /// 生成指定长度随机字符串(默认为6)
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string GetRandomString(int codeCount = 6)
        {
            var str = string.Empty;
            var rep = 0;
            var num2 = DateTime.Now.Ticks + rep;
            rep++;
            var random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (var i = 0; i < codeCount; i++)
            {
                char ch;
                var num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch;
            }
            return str;
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            return GetTimeStamp(DateTimeKind.Utc);
        }
        public static string GetTimeStamp(DateTimeKind kind)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        /// <summary>
        /// 过滤html
        /// </summary>
        /// <param name="HTMLStr"></param>
        /// <returns></returns>
        public static string FilterHTML(string HTMLStr)
        {
            if (!string.IsNullOrEmpty(HTMLStr))
                return System.Text.RegularExpressions.Regex.Replace(HTMLStr, "<[^>]*>|&nbsp;", "");
            else
                return "";
        }
        /// <summary>
        /// 压缩string，用于压缩html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Compress(string text)
        {
            Regex reg = new Regex(@"\s*(</?[^\s/>]+[^>]*>)\s+(</?[^\s/>]+[^>]*>)\s*");
            text = reg.Replace(text, m => m.Groups[1].Value + m.Groups[2].Value);
            reg = new Regex(@"(?<=>)\s|\n|\t(?=<)");
            text = reg.Replace(text, string.Empty);
            return text;
        }
    }
}