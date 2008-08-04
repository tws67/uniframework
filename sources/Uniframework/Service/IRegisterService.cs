using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;

namespace Uniframework.Services
{
    /// <summary>
    /// ע������ӿ�
    /// </summary>
    public interface IRegisterService
    {
        /// <summary>
        /// ��ȡ���ýڵ����������
        /// </summary>
        /// <param name="nodePath">�ڵ�·����·��ʹ��"/"���зָ�</param>
        /// <returns>�ڵ�ֵ</returns>
        NameValueCollection GetNodeSubItems(string nodePath);

        /// <summary>
        /// ���ҽڵ�
        /// </summary>
        /// <param name="name">�ڵ�����</param>
        /// <returns>�ڵ�·��</returns>
        string SearchNode(string name);

        /// <summary>
        /// ��ȡָ���ڵ��ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·����·��ʹ��"/"���зָ�</param>
        /// <param name="itemName">����</param>
        /// <returns>�ڵ��ֵ</returns>
        string GetNodeSubItemValue(string nodePath, string itemName);

        /// <summary>
        /// Ϊָ���ڵ�����ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·����·��ʹ��"/"���зָ�</param>
        /// <param name="itemName">�����ýڵ������</param>
        /// <param name="itemValue">�����õ�ֵ</param>
        void SetNodeSubItemValue(string nodePath, string itemName, string itemValue);

        /// <summary>
        /// ���ýڵ��������Ϣ
        /// </summary>
        /// <param name="nodePath">�ڵ�·����·��ʹ��"/"���зָ�</param>
        /// <param name="description">������Ϣ</param>
        void SetNodeDescription(string nodePath, string description);

        /// <summary>
        /// ����һ���ڵ�
        /// </summary>
        /// <param name="parentNodePath">���ڵ�·��</param>
        /// <param name="nodeName">�ڵ�����</param>
        /// <param name="description">������Ϣ</param>
        void AddNode(string parentNodePath, string nodeName, string description);

        /// <summary>
        /// Ϊ�ڵ����һ��������
        /// </summary>
        /// <param name="nodePath">�ڵ�·����·��ʹ��"/"���зָ�</param>
        /// <param name="itemName">�ڵ�����</param>
        /// <param name="itemValue">�����õ�ֵ</param>
        void AddNodeSubItem(string nodePath, string itemName, string itemValue);

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="parentNodePath">�ڵ�·��</param>
        /// <param name="items">�������б�</param>
        void AddNodeSubItemRange(string parentNodePath, NameValueCollection items);

        /// <summary>
        /// ɾ��һ�����ýڵ��Լ��ýڵ��µ�ֵ������ýڵ㲻Ϊ������ͬ�ýڵ��µ��ӽڵ�һ��ɾ����
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        void RemoveNode(string nodePath);

        /// <summary>
        /// ɾ��һ��������ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="itemName">����������</param>
        void RemoveNodeSubItem(string nodePath, string itemName);

        /// <summary>
        /// ɾ���ڵ������е�������
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        void RemoveNodeAllSubItems(string nodePath);

        /// <summary>
        /// �ж��Ƿ���ڸýڵ�
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����ã�/�����ָ�</param>
        /// <returns>����Ϊtrue������Ϊfalse</returns>
        bool Exist(string nodePath);

        /// <summary>
        /// �ж��Ƿ����ĳ�ڵ㲢ȷ���ýڵ����Ƿ���ָ����ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����ã�/�����ָ�</param>
        /// <param name="itemName">ָ��ֵ������</param>
        /// <returns>����Ϊtrue������Ϊfalse</returns>
        bool Exist(string nodePath, string itemName);

        /// <summary>
        /// �Ƿ����ӽڵ�
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        bool HasChild(string nodePath);

        /// <summary>
        /// �Ƿ�����������
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        bool HasSubItem(string nodePath);

        /// <summary>
        /// ��ȡĳ�ڵ������������ӽڵ����ƣ��ã�/�����ָ�
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <returns>�ӽڵ�����</returns>
        string[] GetChildNodesName(string nodePath);

        /// <summary>
        /// ��ȡ�ڵ�����
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <returns>�ڵ�����</returns>
        string GetNodeDescription(string nodePath);

        /// <summary>
        /// �������ýڵ�
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="newNodeName">�½ڵ�����</param>
        /// <param name="description">�ڵ�����</param>
        void SetNode(string nodePath, string newNodeName, string description);

        /// <summary>
        /// ���ýڵ�������
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="originalItemName">ԭʼ����������</param>
        /// <param name="newItemName">������������</param>
        /// <param name="itemValue">������ֵ</param>
        void SetNodeItem(string nodePath, string originalItemName, string newItemName, string itemValue);

        /// <summary>
        /// �����ƶ��ڵ��ƶ���Ŀ��ڵ���
        /// </summary>
        /// <param name="nodePath">���ƶ��Ľڵ�</param>
        /// <param name="destinationNodePath">Ŀ��ڵ�</param>
        void MoveNode(string nodePath, string destinationNodePath);

        /// <summary>
        /// ���ڵ㸴�Ƶ�Ŀ��ڵ���
        /// </summary>
        /// <param name="nodePath">ԭʼ�ڵ�</param>
        /// <param name="destinationNodePath">Ŀ��ڵ�</param>
        void CopyNode(string nodePath, string destinationNodePath);

        /// <summary>
        /// ���������ļ�
        /// </summary>
        void Save();
    }

    #if(Debug)
    public class MockRegisterService : IRegisterService
    {
        private XmlDocument xml = new XmlDocument();
        private const string ROOT = "/configuration";

        public MockRegisterService()
        {
            xml.AppendChild(xml.CreateElement("config"));
        }

    #region IRegisterService Members

        public string SearchNode(string nodeName)
        {
            ArgumentUtility.AssertNotNull<string>(nodeName, "nodeName");
            nodeName = nodeName.Trim();

            XmlNode node = xml.SelectSingleNode("//" + nodeName);
            if (node == null) throw new ArgumentException("�޷��ҵ��ڵ� [" + nodeName + "]");
            return GetXPath(node);
        }

        private string GetXPath(XmlNode node)
        {
            if (node.Name != "config")
                return GetXPath(node.ParentNode) + "/" + node.Name;
            else
                return string.Empty;
        }

        /// <summary>
        /// �������õ��ļ�
        /// </summary>
        public void Save()
        {

        }

        /// <summary>
        /// ��ȡ���ýڵ����������
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����õ㣨/�����ָ�</param>
        /// <returns>�ڵ�ֵ</returns>
        public NameValueCollection GetNodeSubItems(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();

            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            NameValueCollection subItems = new NameValueCollection();
            foreach (XmlNode childNode in node.SelectNodes("Item"))
            {
                subItems.Add(childNode.Attributes["name"].Value, childNode.InnerText);
            }
            return subItems;
        }

        /// <summary>
        /// ��ȡ�ڵ��ָ��ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����õ㣨/�����ָ�</param>
        /// <param name="name">����</param>
        /// <returns>ֵ</returns>
        public string GetNodeSubItemValue(string nodePath, string itemName)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem == null) throw new ArgumentException("�޷��ҵ��ڵ�" + nodePath + "��������" + itemName);
            return subItem.InnerText;
        }

        /// <summary>
        /// �������ýڵ�
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����õ㣨/�����ָ�</param>
        /// <param name="value">�����õĽڵ�ֵ</param>
        /// <param name="description">�����õĽڵ�����</param>
        public void SetNodeSubItemValue(string nodePath, string itemName, string itemValue)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            if (itemName == "") throw new ArgumentException("���������Ʋ���Ϊ�հ��ַ�", "itemName");
            ArgumentUtility.AssertNotNull<string>(itemValue, "itemValue");
            itemValue = itemValue.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem == null) throw new ArgumentException("�޷��ҵ��ڵ�" + nodePath + "��������" + itemName);

            subItem.InnerText = itemValue;

        }

        /// <summary>
        /// ���ýڵ������
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="description">�ڵ�����</param>
        public void SetNodeDescription(string nodePath, string description)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(description, "description");
            description = description.Trim();
            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            node.Attributes["description"].Value = description;

        }

        /// <summary>
        /// ����һ�����ýڵ�
        /// </summary>
        /// <param name="parentNodePath">���ڵ�·�����õ㣨/�����ָ�</param>
        /// <param name="nodeName">�ڵ�����</param>
        public void AddNode(string parentNodePath, string nodeName, string description)
        {
            ArgumentUtility.AssertNotNull<string>(parentNodePath, "parentNodePath");
            parentNodePath = parentNodePath.Trim();
            string parentPath = EncodePath(parentNodePath);
            ArgumentUtility.AssertNotNull<string>(nodeName, "nodeName");
            nodeName = nodeName.Trim();
            if (nodeName == "") throw new ArgumentException("���ýڵ����Ʋ���Ϊ�հ��ַ�", "nodeName");
            string name = XmlConvert.EncodeName(nodeName);

            string desc = description == null ? "" : description;
            description.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + parentPath);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + parentNodePath, "parentNodePath");

            if (xml.SelectSingleNode(ROOT + parentPath + "/" + name) != null)
                throw new ArgumentException("�ڵ�ǰ�ڵ�·��" + parentNodePath + "�²�������ظ��Ľڵ�����" + nodeName);

            XmlNode newNode = xml.CreateElement(name);
            XmlAttribute descAttr = xml.CreateAttribute("description");
            descAttr.Value = description;
            newNode.Attributes.Append(descAttr);

            node.AppendChild(newNode);

        }

        /// <summary>
        /// ���һ��������
        /// </summary>
        /// <param name="nodePath">���ڵ�·�����õ㣨/�����ָ�</param>
        /// <param name="itemName">����</param>
        /// <param name="itemValue">ֵ</param>
        public void AddNodeSubItem(string nodePath, string itemName, string itemValue)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            if (itemName == "") throw new ArgumentException("���ýڵ����������Ʋ���Ϊ�հ��ַ�", "itemName");
            ArgumentUtility.AssertNotNull<string>(itemValue, "itemValue");
            itemValue = itemValue.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            if (node.SelectSingleNode("Item[@name='" + itemName + "']") != null)
                throw new ArgumentException("�ڵ�ǰ�ڵ�" + nodePath + "�²�������ظ�����������" + itemName);

            XmlNode newNode = xml.CreateElement("Item");
            newNode.InnerText = itemValue;
            XmlAttribute nodeAttr = xml.CreateAttribute("name");
            nodeAttr.Value = itemName;
            newNode.Attributes.Append(nodeAttr);

            node.AppendChild(newNode);

        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="parentNodePath">�ڵ�·��</param>
        /// <param name="items">�������б�</param>
        public void AddNodeSubItemRange(string parentNodePath, NameValueCollection items)
        {
            ArgumentUtility.AssertNotNull<string>(parentNodePath, "parentNodePath");
            parentNodePath = parentNodePath.Trim();

            string parentPath = EncodePath(parentNodePath);
            ArgumentUtility.AssertNotNull<NameValueCollection>(items, "items");

            XmlNode node = xml.SelectSingleNode(ROOT + parentPath);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + parentNodePath, "parentNodePath");

            foreach (string key in items.AllKeys)
            {
                if (node.SelectSingleNode("Item[@name='" + key.Trim() + "']") != null)
                    throw new ArgumentException("�ڵ�ǰ�ڵ�" + parentNodePath + "�²�������ظ�����������" + items[key]);
            }

            foreach (string key in items.AllKeys)
            {
                string value = items[key];
                XmlNode newNode = xml.CreateElement("Item");
                newNode.InnerText = value.Trim();

                XmlAttribute attr = xml.CreateAttribute("name");
                attr.Value = key.Trim();

                newNode.Attributes.Append(attr);
                node.AppendChild(newNode);
            }

        }

        /// <summary>
        /// ɾ��һ�����ýڵ��Լ��ýڵ��µ�ֵ������ýڵ㲻Ϊ������ͬ�ýڵ��µ��ӽڵ�һ��ɾ����
        /// </summary>
        /// <param name="nodePath"></param>
        public void RemoveNode(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            if (path == "") throw new ArgumentException("�޷�ɾ�����ڵ�", "nodePath");

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node != null)
            {
                node.ParentNode.RemoveChild(node);

            }
        }

        /// <summary>
        /// ɾ��һ��������ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <param name="itemName">����������</param>
        public void RemoveNodeSubItem(string nodePath, string itemName)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem != null)
            {
                subItem.ParentNode.RemoveChild(subItem);

            }
        }

        /// <summary>
        /// ɾ���ڵ������е�������
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        public void RemoveNodeAllSubItems(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath.Trim();
            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node != null)
            {
                XmlNodeList subItems = node.SelectNodes("Item");

                foreach (XmlNode subItem in subItems)
                {
                    node.RemoveChild(subItem);
                }

            }
        }

        /// <summary>
        /// �ж��Ƿ���ڸýڵ�
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����õ㣨/�����ָ�</param>
        /// <returns>����Ϊtrue������Ϊfalse</returns>
        public bool Exist(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node != null)
                return true;
            return false;
        }

        /// <summary>
        /// �ж��Ƿ����ĳ�ڵ㲢ȷ���ýڵ����Ƿ���ָ����ֵ
        /// </summary>
        /// <param name="nodePath">�ڵ�·�����õ㣨/�����ָ�</param>
        /// <param name="itemName">ָ��ֵ������</param>
        /// <returns>����Ϊtrue������Ϊfalse</returns>
        public bool Exist(string nodePath, string itemName)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) return false;

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem != null) return true;
            return false;
        }

        /// <summary>
        /// ��ȡĳ�ڵ������������ӽڵ����ƣ��õ㣨/�����ָ�
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <returns>�ӽڵ�����</returns>
        public string[] GetChildNodesName(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNodeList subItems = node.SelectNodes("*[name()!='Item']");

            string[] names = new string[subItems.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = DecodeNodeName(subItems[i].Name);
            }
            return names;

        }

        /// <summary>
        /// ��ȡ�ڵ�����
        /// </summary>
        /// <param name="nodePath">�ڵ�·��</param>
        /// <returns>�ڵ�����</returns>
        public string GetNodeDescription(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            if (path == "") return "";


            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            return node.Attributes["description"].Value;
        }

        /// <summary>
        /// �������ýڵ�
        /// </summary>
        /// <param name="nodePath">ԭʼ�ڵ�����</param>
        /// <param name="newNodeName">�½ڵ�����</param>
        /// <param name="description">�ڵ�˵��</param>
        public void SetNode(string nodePath, string newNodeName, string description)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();

            ArgumentUtility.AssertNotNull<string>(newNodeName, "newNodeName");
            newNodeName = newNodeName.Trim();
            if (newNodeName == "") throw new ArgumentException("���ýڵ����Ʋ���Ϊ�հ��ַ�", "newNodeName");
            if (description == null) description = "";
            description = description.Trim();


            string path = EncodePath(nodePath);

            string name = EncodeNodeName(newNodeName);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            if (node.Name == name)
            {
                SetNodeDescription(nodePath, description);
                return;
            }

            foreach (XmlNode childNode in node.ParentNode.ChildNodes)
            {
                if (childNode.Name == newNodeName) throw new ArgumentException("�½ڵ�����������нڵ����Ʋ����ظ�");
            }

            XmlNode newNode = xml.CreateElement(name);
            XmlAttribute descAttr = xml.CreateAttribute("description");
            descAttr.Value = description;
            newNode.Attributes.Append(descAttr);

            foreach (XmlNode subNode in node.ChildNodes)
            {
                newNode.AppendChild(subNode.CloneNode(true));
            }

            XmlNode parentNode = node.ParentNode;
            parentNode.RemoveChild(node);
            parentNode.AppendChild(newNode);



        }

        /// <summary>
        /// ���ýڵ�������
        /// </summary>
        /// <param name="nodePath">�ڵ�����</param>
        /// <param name="originalItemName">ԭʼ����������</param>
        /// <param name="newItemName">������������</param>
        /// <param name="itemValue">������ֵ</param>
        public void SetNodeItem(string nodePath, string originalItemName, string newItemName, string itemValue)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            ArgumentUtility.AssertNotNull<string>(originalItemName, "originalItemName");
            originalItemName = originalItemName.Trim();
            ArgumentUtility.AssertNotNull<string>(newItemName, "newItemName");
            newItemName = newItemName.Trim();
            if (newItemName == "") throw new ArgumentException("���ýڵ����Ʋ���Ϊ�հ��ַ�", "newItemName");
            ArgumentUtility.AssertNotNull<string>(itemValue, "itemValue");
            itemValue = itemValue.Trim();

            if (originalItemName == newItemName)
            {
                SetNodeSubItemValue(nodePath, newItemName, itemValue);
                return;
            }

            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + originalItemName + "']");
            if (subItem == null) throw new ArgumentException("�޷��ҵ��ڵ�" + nodePath + "��������" + originalItemName);

            XmlNodeList itemList = node.SelectNodes("Item");

            foreach (XmlNode item in itemList)
            {
                if (item.Attributes["name"].Value == newItemName) throw new ArgumentException("����������������������������Ʋ����ظ�");
            }

            subItem.Attributes["name"].Value = newItemName;
            subItem.InnerText = itemValue;

        }

        /// <summary>
        /// �����ƶ��ڵ��ƶ���Ŀ��ڵ���
        /// </summary>
        /// <param name="nodePath">���ƶ��Ľڵ�</param>
        /// <param name="destinationNodePath">Ŀ��ڵ�</param>
        public void MoveNode(string nodePath, string destinationNodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            ArgumentUtility.AssertNotNull<string>(destinationNodePath, "destinationNodePath");
            destinationNodePath = destinationNodePath.Trim();

            if (nodePath == destinationNodePath)
                return;

            string path = EncodePath(nodePath);
            string destPath = EncodePath(destinationNodePath);

            XmlNode sourceNode = xml.SelectSingleNode(ROOT + path);
            if (sourceNode == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNode destNode = xml.SelectSingleNode(ROOT + destPath);
            if (destNode == null) throw new ArgumentException("�޷������������ҵ�Ŀ��ڵ�" + destinationNodePath, "destinationNodePath");

            foreach (XmlNode childNode in destNode.ChildNodes)
            {
                if (childNode.Name == sourceNode.Name) throw new ArgumentException("���ƶ��ڵ����������λ�õĽڵ����Ʋ����ظ�");
            }

            sourceNode.ParentNode.RemoveChild(sourceNode);
            destNode.AppendChild(sourceNode);

        }

        /// <summary>
        /// ���ڵ㸴�Ƶ�Ŀ��ڵ���
        /// </summary>
        /// <param name="nodePath">ԭʼ�ڵ�</param>
        /// <param name="destinationNodePath">Ŀ��ڵ�</param>
        public void CopyNode(string nodePath, string destinationNodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            ArgumentUtility.AssertNotNull<string>(destinationNodePath, "destinationNodePath");
            destinationNodePath = destinationNodePath.Trim();

            if (nodePath == destinationNodePath)
                return;

            string path = EncodePath(nodePath);
            string destPath = EncodePath(destinationNodePath);

            XmlNode sourceNode = xml.SelectSingleNode(ROOT + path);
            if (sourceNode == null) throw new ArgumentException("�޷������������ҵ��ڵ�" + nodePath, "nodePath");

            XmlNode destNode = xml.SelectSingleNode(ROOT + destPath);
            if (destNode == null) throw new ArgumentException("�޷������������ҵ�Ŀ��ڵ�" + destinationNodePath, "destinationNodePath");

            foreach (XmlNode childNode in destNode.ChildNodes)
            {
                if (childNode.Name == sourceNode.Name) throw new ArgumentException("�����ƽڵ����������λ�õĽڵ����Ʋ����ظ�");
            }

            destNode.AppendChild(sourceNode.CloneNode(true));

        }

        public bool HasChild(string nodePath)
        {
            return GetChildNodesName(nodePath).Length > 0;
        }

        public bool HasSubItem(string nodePath)
        {
            return GetNodeSubItems(nodePath).Count > 0;
        }

        #endregion

        private string EncodePath(string path)
        {
            if (path == "/" || path == "") return "";
            if (path.StartsWith("/")) path = path.Substring(1);//ȥ����ͷ��"/"
            if (path.EndsWith("/")) path = path.Remove(path.Length - 1);//ȥ����β��"/"
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
    }
    #endif
}
