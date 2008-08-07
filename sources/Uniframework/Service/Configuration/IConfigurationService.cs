using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统注册表服务接口
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// 判断路径是否存在
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回true，否则返回false</returns>
        bool Exists(string path);

        /// <summary>
        /// 判断指定路径中的某个节点是否存在
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="item">节点名称</param>
        /// <returns>如果存在的话返回true，否则返回false</returns>
        bool Exists(string path, string item);

        /// <summary>
        /// 判断指定路径中的是否存在子节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回true，否则返回false</returns>
        bool HasChildren(string path);

        /// <summary>
        /// 获取指定路径的节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回指定路径下的节点，否则返回null</returns>
        XmlNode GetItem(string path);

        /// <summary>
        /// 获取指定路径、指定节点名称的节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="item">属性名称</param>
        /// <returns>如果存在的话返回指定路径下的节点，否则返回null</returns>
        XmlNode GetItem(string path, KeyValuePair<string, string> item);

        /// <summary>
        /// 获取指定路径下对应索引的节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="index">索引号</param>
        /// <returns>如果存在的话返回指定路径下的节点，否则返回null</returns>
        XmlNode GetItem(string path, int index);

        /// <summary>
        /// 获取指定路径下节点的值
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回指定路径节点下的值，否则返回空字符串</returns>
        string GetItemValue(string path);

        /// <summary>
        /// 获取指定路径节点的属性值
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <param name="attribute">属性名称</param>
        /// <returns>如果存在的话返回指定属性的值，否则返回空字符串</returns>
        string GetItemAttribute(string path, string attribute);

        /// <summary>
        /// 获取指定路径下的所有子节点
        /// </summary>
        /// <param name="path">路径名称</param>
        /// <returns>如果存在的话返回指定路径下的所有节点列表，否则返回null</returns>
        XmlNodeList GetChildren(string path);

        /// <summary>
        /// 查询指定名称的节点路径
        /// </summary>
        /// <param name="item">节点名称</param>
        /// <returns>如果找到节点返回节点的路径，否则返回null</returns>
        string SearchNode(string item);
    }
}
