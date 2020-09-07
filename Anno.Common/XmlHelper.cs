using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;

namespace Anno.Common
{
    /// <summary>
    /// xml 帮助类
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 读取制定文件的 节点
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public XmlNode GetNodes(string xmlPath, string nodename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);
            XmlNode nodes = xml.SelectSingleNode(nodename);
            return nodes;
        }
    }
}
