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
    /// ϵͳע�������XMLʵ����
    /// </summary>
    public class XMLConfigurationService : IConfigurationService
    {
        private string filename = string.Empty;
        private XmlDocument xml = new XmlDocument();
        private readonly static string RootPath = "/configuration";

        /// <summary>
        /// ����XML���÷���
        /// </summary>
        /// <param name="filename">�����ļ�����</param>
        public XMLConfigurationService(string filename)
        {
            this.filename = FileUtility.ConvertToFullPath(filename);
            if (!File.Exists(this.filename))
                throw new ArgumentException(String.Format("ϵͳ�����ļ� \"{0}\" δ�ҵ���"));

            FileStream fs = null;

            try {
                if (HttpContext.Current != null) {
                    filename = (filename.IndexOf("~/") == -1) ? "~/" + filename : filename; // ������·��
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
            if (path.StartsWith("/")) path = path.Substring(1); // ȥ��·����ͷ��"/"
            if (path.EndsWith("/")) path = path.Remove(path.Length - 1, 1); // ȥ��·����β��"/"
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
        /// �ж�·���Ƿ����
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����true�����򷵻�false</returns>
        public bool Exists(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            return node != null;
        }

        /// <summary>
        /// ���ָ��·���Ľڵ��Ƿ����
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="item">�ڵ�����</param>
        /// <returns>������ڵĻ�����true�����򷵻�false</returns>
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
        /// ���ָ��·�����Ƿ����ӽڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����true�����򷵻�false</returns>
        public bool HasChildren(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = path.Trim();
            string nodepath = EncodePath(path);

            XmlNode node = xml.SelectSingleNode(RootPath + nodepath);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + path, "path");

            XmlNodeList subItems = node.SelectNodes("*");
            return subItems.Count > 0;

        }

        /// <summary>
        /// ��ȡָ��·���Ľڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����ָ��·���µĽڵ㣬���򷵻�null</returns>
        public XmlNode GetItem(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = EncodePath(path);
            return xml.SelectSingleNode(RootPath + path);
        }

        /// <summary>
        /// ��ȡָ��·����ָ���ڵ����ƵĽڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="item">��������</param>
        /// <returns>������ڵĻ�����ָ��·���µĽڵ㣬���򷵻�null</returns>
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
        /// ��ȡָ��·���¶�Ӧ�����Ľڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="index">������</param>
        /// <returns>������ڵĻ�����ָ��·���µĽڵ㣬���򷵻�null</returns>
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
        /// ��ȡָ��·���Ľڵ��ֵ
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����ָ��·���ڵ��µ�ֵ�����򷵻ؿ��ַ���</returns>
        public string GetItemValue(string path)
        {
            ArgumentHelper.AssertNotNull<string>(path, "path");
            path = EncodePath(path);
            XmlNode node = xml.SelectSingleNode(RootPath + path);
            return node == null ? string.Empty : node.InnerText;
        }

        /// <summary>
        /// ��ȡָ��·���Ľڵ��������Ϣ
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="attribute">��������</param>
        /// <returns>������ڵĻ�����ָ�����Ե�ֵ�����򷵻ؿ��ַ���</returns>
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
        /// ��ȡָ��·�������е��ӽڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����ָ��·���µ����нڵ��б����򷵻�null</returns>
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
        /// ��ȡָ���ڵ����Ƶ�·����Ϣ
        /// </summary>
        /// <param name="item">�ڵ�����</param>
        /// <returns>����ҵ��ڵ㷵�ؽڵ��·�������򷵻�null</returns>
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
