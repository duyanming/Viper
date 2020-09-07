using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Anno.Common
{
    /// <summary>
    /// Http 帮助类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// Http请求POST
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">数据  （"id=777&wxType=1"）</param>
        /// <returns>结果字符串 string retString = reader.ReadToEnd()</returns>
        public Task<StreamReader> HttpPostAsync(string url, string postDataStr)
        {
            return Task.Run(() =>
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (string.IsNullOrEmpty(encoding))
                {
                    encoding = "UTF-8"; //默认编码  
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                //string retString = reader.ReadToEnd();
                return reader;
            });
        }

        /// <summary>
        /// Http请求GET
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postDataStr">数据  （"id=777&wxType=1"）</param>
        /// <returns></returns>
        public Task<StreamReader> HttpGetAsync(string url)
        {
            return Task.Run(() =>
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                string encoding = response.ContentEncoding;
                if (string.IsNullOrEmpty(encoding))
                {
                    encoding = "UTF-8"; //默认编码  
                }
                var reader = new StreamReader(stream, Encoding.GetEncoding(encoding));
                return reader;
                //string retString = reader.ReadToEnd();
            });
        }

        internal static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;

        }
    }
}