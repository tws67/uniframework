using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 授权节点信息，用于表示可用于授权的一个节点
    /// </summary>
    public interface IAuthorizationNode
    {
        /// <summary>
        /// 授权节点路径，此路径与AuthorizationAttribute属性中的路径一致，由从根节点到当前节点的所有标识组成节点的路径
        /// </summary>
        /// <value>授权节点路径</value>
        string AuthorizationUri { get; set; }
        /// <summary>
        /// 授权节点标识
        /// </summary>
        /// <value>节点标识</value>
        string Id { get; set; }
        /// <summary>
        /// 授权节点名称.
        /// </summary>
        /// <value>节点名称</value>
        string Name { get; set; }
        /// <summary>
        /// 授权节点所要执行的命令
        /// </summary>
        /// <value>节点命令</value>
        string Command { get; set; }
        /// <summary>
        /// 授权节点命令所使用的图标文件名称
        /// </summary>
        /// <value>图标文件</value>
        string ImageFile { get; set; }
    }
}
