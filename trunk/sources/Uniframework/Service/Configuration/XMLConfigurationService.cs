using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统注册表服务的XML实现类
    /// </summary>
    public class XMLConfigurationService : IConfigurationService
    {
        private string filename = string.Empty;
        private XmlDocument xml = new XmlDocument();
        private readonly static string RootPath = "/configuration";

        /// <summary>
        /// 创建XML配置服务
        /// </summary>
        /// <param name="filename">配置文件名称</param>
        public XMLConfigurationService(string filename)
        {
            this.filename = filename;
            FileStream fs = new FileStream(filename, FileMode.Open);
            try
            {
                xml.Load(fs);
            }
            catch
            {
                xml.CreateElement("configuration");
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        #region Assistant function

        private string EncodePath(string path)
        {
            if (path == "/" || path == "") return "";
            if (path.StartsWith("/")) path = path.Substring(1); // 去掉路径开头的"/"
            if (path.EndsWith("/")) path = path.Remove(path.Length - 1, 1); // 去掉路径结尾的"/"
            string[] nodes = path.Split('/');
            string[] ns = new string[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                ns[i] = XmlConvert.EncodeName(nodes[i]);
            }
            return "/" + string.Join("/", ns);
        }

        private string EncodeNodeName(string nodeName)
        {
            return XmlConvert.EncodeName(nodeName);
        }

        private string DecodeNodeName(string nodeName)
        {
            return XmlConvert.DecodeName(nodeName);
        }

        private string GetXPath(XmlNode node)
        {
            if (node.Name != "configuration")
                return GetXPath(node.ParentNode) + "/" + DecodeNodeName(node.Name);
            else
                return string.Empty;
        }

        #endregion

        #region IConfigurationService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Exist(string path)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            return node != null;
        }

        /// <summary>
        /// 检查指定路径的节点是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Exist(string path, string item)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);
            ArgumentUtility.AssertNotNull<string>(item, "item");
            item = item.Trim();
            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            if (node == null) return false;

            XmlNode subItem = node.SelectSingleNode("*[name()='" + item + "']");
            return subItem != null;
        }

        /// <summary>
        /// 检查指定路径下是否含有子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool HasChildren(string path)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + path, "path");

            XmlNodeList subItems = node.SelectNodes("*");
            return subItems.Count > 0;

        }

        /// <summary>
        /// 获取指定路径的节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlNode GetItem(string path)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            path = EncodePath(path);
            return xml.SelectSingleNode(RootPath + path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public XmlNode GetItem(string path, KeyValuePair<string, string> item)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            ArgumentUtility.AssertNotNull<string>(item.Key, "item.Key");
            ArgumentUtility.AssertNotNull<string>(item.Value, "item.Value");
            path = EncodePath(path);
            XmlNode node = xml.SelectSingleNode(RootPath + path);
            return node == null ? null : node.SelectSingleNode("[" + item.Key + "='" + item.Value + "']");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public XmlNode GetItem(string path, int index)
        {
            XmlNodeList nodes = GetChildren(path);
            if (nodes != null)
            {
                return index >= 0 && index < nodes.Count ? nodes.Item(index) : null;
            }

            return null;
        }

        /// <summary>
        /// 获取指定路径的节点的值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetItemValue(string path)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            path = EncodePath(path);
            XmlNode node = xml.SelectSingleNode(RootPath + path);
            return node == null ? string.Empty : node.InnerText;
        }

        /// <summary>
        /// 获取指定路径的节点的属性信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public string GetItemAttribute(string path, string attribute)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            ArgumentUtility.AssertNotNull<string>(attribute, "attribute");

            path = EncodePath(path);
            XmlNode node = xml.SelectSingleNode(RootPath + path);
            if (node == null)
                return string.Empty;

            for (int i = 0; i < node.Attributes.Count; i++)
            {
                if (node.Attributes.Item(i).Name.ToLower() == attribute.ToLower())
                {
                    return node.Attributes.Item(i).Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取指定路径下所有的子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public XmlNodeList GetChildren(string path)
        {
            ArgumentUtility.AssertNotNull<string>(path, "path");
            path = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + path);
            if (node == null)
                return null;
            return node.SelectNodes("*");
        }

        /// <summary>
        /// 获取指定节点名称的路径信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string SearchNode(string item)
        {
            ArgumentUtility.AssertNotNull<string>(item, "item");
            item = EncodeNodeName(item.Trim());

            XmlNode node = xml.SelectSingleNode("//" + item);
            if (node == null) return null;
            return GetXPath(node);
        }

        #endregion
    }
}
