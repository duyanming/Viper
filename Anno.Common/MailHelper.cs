using System;
using System.Net.Mail;
using System.Xml;
using System.IO;

namespace Anno.Common
{
    /// <summary>
    /// 电子邮件帮助类
    /// </summary>
    public class MailHelper
    {
        string uid = "Anno6295@163.com", pwd = "Anno62958", smtp = "smtp.163.com";
        /// <summary>
        /// 构造函数 Anno6295@163.com
        /// </summary>
        public MailHelper()
        {
        }

        /// <summary>
        /// < Mail name = "Mail" smtp = "smtp.163.com" uid = "Anno6295@163.com" pwd = "Anno62958" ></Mail >
        /// </summary>
        /// <param name="dic">配置文件路径</param>
        public MailHelper(string dic)
        {
            XmlHelper xh = new XmlHelper();
            string xmlpath = Path.Combine(Directory.GetCurrentDirectory(), "Anno.config");
            XmlNode mail = xh.GetNodes(xmlpath, "//*[@name='Mail']");
            uid = mail.Attributes["uid"].Value;
            pwd = mail.Attributes["pwd"].Value;
            smtp = mail.Attributes["smtp"].Value;
        }
        /// <summary>
        /// 设置发送人
        /// </summary>
        /// <param name="uid">发件人邮箱</param>
        /// <param name="pwd">发件人地址</param>
        /// <param name="smtp">发件邮箱 smtp</param>
        public MailHelper(string uid,string pwd,string smtp)
        {
            this.uid = uid;
            this.pwd = pwd;
            this.smtp = smtp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">邮件主题</param>
        /// <param name="Tmail">收件人邮箱</param>
        /// <param name="body">邮件内容</param>
        /// <param name="Sname">发送人姓名</param>
        /// <param name="Smail">发送人邮件地址</param>
        /// <param name="CC">抄送，为Null 不抄送</param>
        /// <returns></returns>
        public bool SendMail(string title, string Tmail, string body, string Sname, string Smail, string CC = null)
        {
            try
            {
                MailAddress from = new MailAddress(Smail);
                MailAddress to = null;
                MailAddress copy = null;
                to = new MailAddress(Tmail);

                MailMessage message = new MailMessage(from, to);
                if (CC != null)
                {
                    copy = new MailAddress(CC);//抄送给
                    message.CC.Add(copy);
                }
                message.Subject = title;
                message.Body = body;
                message.IsBodyHtml = true;

                message.BodyEncoding = System.Text.Encoding.GetEncoding("gb2312");// "Unicode8";   
                SmtpClient client = new SmtpClient(smtp);

                client.Credentials = new System.Net.NetworkCredential(uid, pwd);
                client.Send(message);
                return true;
            }
            catch
            {
                return false;

            }
        }
    }
}
