using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Web;
using System.Windows.Forms;

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
            this.filename = FileUtility.ConvertToFullPath(filename);
            if (!File.Exists(this.filename))
                throw new ArgumentException(String.Format("系统配置文件 \"{0}\" 未找到。"));

            FileStream fs = null;

            try {
                if (HttpContext.Current != null) {
                    filename = (filename.IndexOf("~/") == -1) ? "~/" + filename : filename; // 添加相对路径
                    fs = new FileStream(HttpContext.Current.Server.MapPath(filename), FileMode.Open);
                }
                else {
                    fs = new FileStream(filename, FileMode.Open);
                }

                xml.Load(fs);
            }
            catch {
                xml.CreateElement("configuration");
            }
            finally {
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
        /// 判断路径是否存在
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回true，否则返回false</returns>
        public bool Exists(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            return node != null;
        }

        /// <summary>
        /// 检查指定路径的节点是否存在
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="item">节点名称</param>
        /// <returns>如果存在的话返回true，否则返回false</returns>
        public bool Exists(string path, string item)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);
            ArgumentHelper.AssertNotNull<string>(item, "item");
            item = item.Trim();
            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            if (node == null) return false;

            XmlNode subItem = node.SelectSingleNode("*[name()='" + item + "']");
            return subItem != null;
        }

        /// <summary>
        /// 检查指定路径下是否含有子节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回true，否则返回false</returns>
        public bool HasChildren(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
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
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回指定路径下的节点，否则返回null</returns>
        public XmlNode GetItem(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = EncodePath(path);
            return xml.SelectSingleNode(RootPath + path);
        }

        /// <summary>
        /// 获取指定路径、指定节点名称的节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="item">属性名称</param>
        /// <returns>如果存在的话返回指定路径下的节点，否则返回null</returns>
        public XmlNode GetItem(string path, KeyValuePair<string, string> item)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            ArgumentHelper.AssertNotNull<string>(item.Key, "item.Key");
            ArgumentHelper.AssertNotNull<string>(item.Value, "item.Value");
            path = EncodePath(path);
            XmlNode node = xml.SelectSingleNode(RootPath + path);
            return node == null ? null : node.SelectSingleNode("[" + item.Key + "='" + item.Value + "']");
        }

        /// <summary>
        /// 获取指定路径下对应索引的节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="index">索引号</param>
        /// <returns>如果存在的话返回指定路径下的节点，否则返回null</returns>
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
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回指定路径节点下的值，否则返回空字符串</returns>
        public string GetItemValue(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = EncodePath(path);
            XmlNode node = xml.SelectSingleNode(RootPath + path);
            return node == null ? string.Empty : node.InnerText;
        }

        /// <summary>
        /// 获取指定路径的节点的属性信息
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="attribute">属性名称</param>
        /// <returns>如果存在的话返回指定属性的值，否则返回空字符串</returns>
        public string GetItemAttribute(string path, string attribute)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            ArgumentHelper.AssertNotNull<string>(attribute, "attribute");

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
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回指定路径下的所有节点列表，否则返回null</returns>
        public XmlNodeList GetChildren(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + path);
            if (node == null)
                return null;
            return node.SelectNodes("*");
        }

        /// <summary>
        /// 获取指定节点名称的路径信息
        /// </summary>
        /// <param name="item">节点名称</param>
        /// <returns>如果找到节点返回节点的路径，否则返回null</returns>
        public string SearchNode(string item)
        {
            ArgumentHelper.AssertNotNull<string>(item, "item");
            item = EncodeNodeName(item.Trim());

            XmlNode node = xml.SelectSingleNode("//" + item);
            if (node == null) return null;
            return GetXPath(node);
        }

        #endregion
    }
}
