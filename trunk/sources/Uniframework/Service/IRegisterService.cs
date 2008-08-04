using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;

namespace Uniframework.Services
{
    /// <summary>
    /// 注册表服务接口
    /// </summary>
    public interface IRegisterService
    {
        /// <summary>
        /// 获取配置节点的所有子项
        /// </summary>
        /// <param name="nodePath">节点路径，路径使用"/"进行分隔</param>
        /// <returns>节点值</returns>
        NameValueCollection GetNodeSubItems(string nodePath);

        /// <summary>
        /// 查找节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>节点路径</returns>
        string SearchNode(string name);

        /// <summary>
        /// 获取指定节点的值
        /// </summary>
        /// <param name="nodePath">节点路径，路径使用"/"进行分隔</param>
        /// <param name="itemName">名称</param>
        /// <returns>节点的值</returns>
        string GetNodeSubItemValue(string nodePath, string itemName);

        /// <summary>
        /// 为指定节点设置值
        /// </summary>
        /// <param name="nodePath">节点路径，路径使用"/"进行分隔</param>
        /// <param name="itemName">待设置节点的名称</param>
        /// <param name="itemValue">待设置的值</param>
        void SetNodeSubItemValue(string nodePath, string itemName, string itemValue);

        /// <summary>
        /// 设置节点的描述信息
        /// </summary>
        /// <param name="nodePath">节点路径，路径使用"/"进行分隔</param>
        /// <param name="description">描述信息</param>
        void SetNodeDescription(string nodePath, string description);

        /// <summary>
        /// 增加一个节点
        /// </summary>
        /// <param name="parentNodePath">父节点路径</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="description">描述信息</param>
        void AddNode(string parentNodePath, string nodeName, string description);

        /// <summary>
        /// 为节点添加一个数据项
        /// </summary>
        /// <param name="nodePath">节点路径，路径使用"/"进行分隔</param>
        /// <param name="itemName">节点名称</param>
        /// <param name="itemValue">待设置的值</param>
        void AddNodeSubItem(string nodePath, string itemName, string itemValue);

        /// <summary>
        /// 增加子数据项
        /// </summary>
        /// <param name="parentNodePath">节点路径</param>
        /// <param name="items">数据项列表</param>
        void AddNodeSubItemRange(string parentNodePath, NameValueCollection items);

        /// <summary>
        /// 删除一个配置节点以及该节点下的值（假如该节点不为空则连同该节点下的子节点一并删除）
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        void RemoveNode(string nodePath);

        /// <summary>
        /// 删除一个数据项值
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="itemName">数据项名称</param>
        void RemoveNodeSubItem(string nodePath, string itemName);

        /// <summary>
        /// 删除节点下所有的数据项
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        void RemoveNodeAllSubItems(string nodePath);

        /// <summary>
        /// 判断是否存在该节点
        /// </summary>
        /// <param name="nodePath">节点路径，用（/）来分割</param>
        /// <returns>存在为true，否则为false</returns>
        bool Exist(string nodePath);

        /// <summary>
        /// 判断是否存在某节点并确定该节点下是否有指定的值
        /// </summary>
        /// <param name="nodePath">节点路径，用（/）来分割</param>
        /// <param name="itemName">指定值的名称</param>
        /// <returns>存在为true，否则为false</returns>
        bool Exist(string nodePath, string itemName);

        /// <summary>
        /// 是否有子节点
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        bool HasChild(string nodePath);

        /// <summary>
        /// 是否有子数据项
        /// </summary>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        bool HasSubItem(string nodePath);

        /// <summary>
        /// 获取某节点下来的所有子节点名称，用（/）来分割
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <returns>子节点名称</returns>
        string[] GetChildNodesName(string nodePath);

        /// <summary>
        /// 获取节点描述
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <returns>节点描述</returns>
        string GetNodeDescription(string nodePath);

        /// <summary>
        /// 设置配置节点
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="newNodeName">新节点名称</param>
        /// <param name="description">节点描述</param>
        void SetNode(string nodePath, string newNodeName, string description);

        /// <summary>
        /// 设置节点数据项
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="originalItemName">原始数据项名称</param>
        /// <param name="newItemName">新数据项名称</param>
        /// <param name="itemValue">数据项值</param>
        void SetNodeItem(string nodePath, string originalItemName, string newItemName, string itemValue);

        /// <summary>
        /// 将被移动节点移动到目标节点下
        /// </summary>
        /// <param name="nodePath">被移动的节点</param>
        /// <param name="destinationNodePath">目标节点</param>
        void MoveNode(string nodePath, string destinationNodePath);

        /// <summary>
        /// 将节点复制到目标节点下
        /// </summary>
        /// <param name="nodePath">原始节点</param>
        /// <param name="destinationNodePath">目标节点</param>
        void CopyNode(string nodePath, string destinationNodePath);

        /// <summary>
        /// 保存配置文件
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
            if (node == null) throw new ArgumentException("无法找到节点 [" + nodeName + "]");
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
        /// 保存配置到文件
        /// </summary>
        public void Save()
        {

        }

        /// <summary>
        /// 获取配置节点的所有子项
        /// </summary>
        /// <param name="nodePath">节点路径，用点（/）来分割</param>
        /// <returns>节点值</returns>
        public NameValueCollection GetNodeSubItems(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();

            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            NameValueCollection subItems = new NameValueCollection();
            foreach (XmlNode childNode in node.SelectNodes("Item"))
            {
                subItems.Add(childNode.Attributes["name"].Value, childNode.InnerText);
            }
            return subItems;
        }

        /// <summary>
        /// 获取节点的指定值
        /// </summary>
        /// <param name="nodePath">节点路径，用点（/）来分割</param>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public string GetNodeSubItemValue(string nodePath, string itemName)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem == null) throw new ArgumentException("无法找到节点" + nodePath + "的数据项" + itemName);
            return subItem.InnerText;
        }

        /// <summary>
        /// 设置配置节点
        /// </summary>
        /// <param name="nodePath">节点路径，用点（/）来分割</param>
        /// <param name="value">欲设置的节点值</param>
        /// <param name="description">欲设置的节点描述</param>
        public void SetNodeSubItemValue(string nodePath, string itemName, string itemValue)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            if (itemName == "") throw new ArgumentException("数据项名称不能为空白字符", "itemName");
            ArgumentUtility.AssertNotNull<string>(itemValue, "itemValue");
            itemValue = itemValue.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem == null) throw new ArgumentException("无法找到节点" + nodePath + "的数据项" + itemName);

            subItem.InnerText = itemValue;

        }

        /// <summary>
        /// 设置节点的描述
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="description">节点描述</param>
        public void SetNodeDescription(string nodePath, string description)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(description, "description");
            description = description.Trim();
            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            node.Attributes["description"].Value = description;

        }

        /// <summary>
        /// 增加一个配置节点
        /// </summary>
        /// <param name="parentNodePath">父节点路径，用点（/）来分割</param>
        /// <param name="nodeName">节点名称</param>
        public void AddNode(string parentNodePath, string nodeName, string description)
        {
            ArgumentUtility.AssertNotNull<string>(parentNodePath, "parentNodePath");
            parentNodePath = parentNodePath.Trim();
            string parentPath = EncodePath(parentNodePath);
            ArgumentUtility.AssertNotNull<string>(nodeName, "nodeName");
            nodeName = nodeName.Trim();
            if (nodeName == "") throw new ArgumentException("配置节点名称不能为空白字符", "nodeName");
            string name = XmlConvert.EncodeName(nodeName);

            string desc = description == null ? "" : description;
            description.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + parentPath);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + parentNodePath, "parentNodePath");

            if (xml.SelectSingleNode(ROOT + parentPath + "/" + name) != null)
                throw new ArgumentException("在当前节点路径" + parentNodePath + "下不能添加重复的节点名称" + nodeName);

            XmlNode newNode = xml.CreateElement(name);
            XmlAttribute descAttr = xml.CreateAttribute("description");
            descAttr.Value = description;
            newNode.Attributes.Append(descAttr);

            node.AppendChild(newNode);

        }

        /// <summary>
        /// 添加一个数据项
        /// </summary>
        /// <param name="nodePath">父节点路径，用点（/）来分割</param>
        /// <param name="itemName">名称</param>
        /// <param name="itemValue">值</param>
        public void AddNodeSubItem(string nodePath, string itemName, string itemValue)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            if (itemName == "") throw new ArgumentException("配置节点数据项名称不能为空白字符", "itemName");
            ArgumentUtility.AssertNotNull<string>(itemValue, "itemValue");
            itemValue = itemValue.Trim();

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            if (node.SelectSingleNode("Item[@name='" + itemName + "']") != null)
                throw new ArgumentException("在当前节点" + nodePath + "下不能添加重复的数据项名" + itemName);

            XmlNode newNode = xml.CreateElement("Item");
            newNode.InnerText = itemValue;
            XmlAttribute nodeAttr = xml.CreateAttribute("name");
            nodeAttr.Value = itemName;
            newNode.Attributes.Append(nodeAttr);

            node.AppendChild(newNode);

        }

        /// <summary>
        /// 增加子数据项
        /// </summary>
        /// <param name="parentNodePath">节点路径</param>
        /// <param name="items">数据项列表</param>
        public void AddNodeSubItemRange(string parentNodePath, NameValueCollection items)
        {
            ArgumentUtility.AssertNotNull<string>(parentNodePath, "parentNodePath");
            parentNodePath = parentNodePath.Trim();

            string parentPath = EncodePath(parentNodePath);
            ArgumentUtility.AssertNotNull<NameValueCollection>(items, "items");

            XmlNode node = xml.SelectSingleNode(ROOT + parentPath);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + parentNodePath, "parentNodePath");

            foreach (string key in items.AllKeys)
            {
                if (node.SelectSingleNode("Item[@name='" + key.Trim() + "']") != null)
                    throw new ArgumentException("在当前节点" + parentNodePath + "下不能添加重复的数据项名" + items[key]);
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
        /// 删除一个配置节点以及该节点下的值（假如该节点不为空则连同该节点下的子节点一并删除）
        /// </summary>
        /// <param name="nodePath"></param>
        public void RemoveNode(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            if (path == "") throw new ArgumentException("无法删除根节点", "nodePath");

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node != null)
            {
                node.ParentNode.RemoveChild(node);

            }
        }

        /// <summary>
        /// 删除一个数据项值
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <param name="itemName">数据项名称</param>
        public void RemoveNodeSubItem(string nodePath, string itemName)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            ArgumentUtility.AssertNotNull<string>(itemName, "itemName");
            itemName = itemName.Trim();
            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + itemName + "']");
            if (subItem != null)
            {
                subItem.ParentNode.RemoveChild(subItem);

            }
        }

        /// <summary>
        /// 删除节点下所有的数据项
        /// </summary>
        /// <param name="nodePath">节点路径</param>
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
        /// 判断是否存在该节点
        /// </summary>
        /// <param name="nodePath">节点路径，用点（/）来分割</param>
        /// <returns>存在为true，否则为false</returns>
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
        /// 判断是否存在某节点并确定该节点下是否有指定的值
        /// </summary>
        /// <param name="nodePath">节点路径，用点（/）来分割</param>
        /// <param name="itemName">指定值的名称</param>
        /// <returns>存在为true，否则为false</returns>
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
        /// 获取某节点下来的所有子节点名称，用点（/）来分割
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <returns>子节点名称</returns>
        public string[] GetChildNodesName(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNodeList subItems = node.SelectNodes("*[name()!='Item']");

            string[] names = new string[subItems.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = DecodeNodeName(subItems[i].Name);
            }
            return names;

        }

        /// <summary>
        /// 获取节点描述
        /// </summary>
        /// <param name="nodePath">节点路径</param>
        /// <returns>节点描述</returns>
        public string GetNodeDescription(string nodePath)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            string path = EncodePath(nodePath);
            if (path == "") return "";


            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            return node.Attributes["description"].Value;
        }

        /// <summary>
        /// 设置配置节点
        /// </summary>
        /// <param name="nodePath">原始节点名称</param>
        /// <param name="newNodeName">新节点名称</param>
        /// <param name="description">节点说明</param>
        public void SetNode(string nodePath, string newNodeName, string description)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();

            ArgumentUtility.AssertNotNull<string>(newNodeName, "newNodeName");
            newNodeName = newNodeName.Trim();
            if (newNodeName == "") throw new ArgumentException("配置节点名称不能为空白字符", "newNodeName");
            if (description == null) description = "";
            description = description.Trim();


            string path = EncodePath(nodePath);

            string name = EncodeNodeName(newNodeName);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            if (node.Name == name)
            {
                SetNodeDescription(nodePath, description);
                return;
            }

            foreach (XmlNode childNode in node.ParentNode.ChildNodes)
            {
                if (childNode.Name == newNodeName) throw new ArgumentException("新节点的名称与现有节点名称产生重复");
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
        /// 设置节点数据项
        /// </summary>
        /// <param name="nodePath">节点名称</param>
        /// <param name="originalItemName">原始数据项名称</param>
        /// <param name="newItemName">新数据项名称</param>
        /// <param name="itemValue">数据项值</param>
        public void SetNodeItem(string nodePath, string originalItemName, string newItemName, string itemValue)
        {
            ArgumentUtility.AssertNotNull<string>(nodePath, "nodePath");
            nodePath = nodePath.Trim();
            ArgumentUtility.AssertNotNull<string>(originalItemName, "originalItemName");
            originalItemName = originalItemName.Trim();
            ArgumentUtility.AssertNotNull<string>(newItemName, "newItemName");
            newItemName = newItemName.Trim();
            if (newItemName == "") throw new ArgumentException("配置节点名称不能为空白字符", "newItemName");
            ArgumentUtility.AssertNotNull<string>(itemValue, "itemValue");
            itemValue = itemValue.Trim();

            if (originalItemName == newItemName)
            {
                SetNodeSubItemValue(nodePath, newItemName, itemValue);
                return;
            }

            string path = EncodePath(nodePath);

            XmlNode node = xml.SelectSingleNode(ROOT + path);
            if (node == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNode subItem = node.SelectSingleNode("Item[@name='" + originalItemName + "']");
            if (subItem == null) throw new ArgumentException("无法找到节点" + nodePath + "的数据项" + originalItemName);

            XmlNodeList itemList = node.SelectNodes("Item");

            foreach (XmlNode item in itemList)
            {
                if (item.Attributes["name"].Value == newItemName) throw new ArgumentException("新数据项的名称与现有数据项名称产生重复");
            }

            subItem.Attributes["name"].Value = newItemName;
            subItem.InnerText = itemValue;

        }

        /// <summary>
        /// 将被移动节点移动到目标节点下
        /// </summary>
        /// <param name="nodePath">被移动的节点</param>
        /// <param name="destinationNodePath">目标节点</param>
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
            if (sourceNode == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNode destNode = xml.SelectSingleNode(ROOT + destPath);
            if (destNode == null) throw new ArgumentException("无法在配置树中找到目标节点" + destinationNodePath, "destinationNodePath");

            foreach (XmlNode childNode in destNode.ChildNodes)
            {
                if (childNode.Name == sourceNode.Name) throw new ArgumentException("被移动节点的名称与新位置的节点名称产生重复");
            }

            sourceNode.ParentNode.RemoveChild(sourceNode);
            destNode.AppendChild(sourceNode);

        }

        /// <summary>
        /// 将节点复制到目标节点下
        /// </summary>
        /// <param name="nodePath">原始节点</param>
        /// <param name="destinationNodePath">目标节点</param>
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
            if (sourceNode == null) throw new ArgumentException("无法在配置树中找到节点" + nodePath, "nodePath");

            XmlNode destNode = xml.SelectSingleNode(ROOT + destPath);
            if (destNode == null) throw new ArgumentException("无法在配置树中找到目标节点" + destinationNodePath, "destinationNodePath");

            foreach (XmlNode childNode in destNode.ChildNodes)
            {
                if (childNode.Name == sourceNode.Name) throw new ArgumentException("被复制节点的名称与新位置的节点名称产生重复");
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
            if (path.StartsWith("/")) path = path.Substring(1);//去掉开头的"/"
            if (path.EndsWith("/")) path = path.Remove(path.Length - 1);//去掉结尾的"/"
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
