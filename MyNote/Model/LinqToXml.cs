using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace MyNote
{
    /// <summary>
    /// XDocment文档对象模型拓展
    /// </summary>
    static class LinqToXml
    {
        /// <summary>
        /// 获取节点指定名称的第一个子节点
        /// </summary>
        /// <param name="xElement">节点</param>
        /// <param name="nodeName">子节点名称</param>
        /// <returns></returns>
        public static IXmlNode Element(this IXmlNode xElement, string nodeName)
        {
            foreach (var node in xElement.ChildNodes)
            {
                if (node.NodeName == nodeName)
                    return node;
            }
            return null;
        }
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="doc">xml文档</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeValue">节点值</param>
        /// <returns></returns>
        public static IXmlNode CreateElement(this XmlDocument doc, string nodeName, string nodeValue)
        {
            var node = doc.CreateElement(nodeName);
            if (nodeValue != null)
                node.InnerText = nodeValue;
            return node;
        }
    }
}
