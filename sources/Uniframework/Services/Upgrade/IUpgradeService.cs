using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统更新服务
    /// </summary>
    [RemoteService("系统更新服务", ServiceType.Infrustructure, ServiceScope.SmartClient)]
    public interface IUpgradeService
    {
        /// <summary>
        /// 获取指定软件项目的最新版本
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <returns>返回更新项目配置情况</returns>
        [RemoteMethod]
        UpgradeProject GetUpgradeProject(string product);
        /// <summary>
        /// 获取指定软件项目指定版本的更新配置信息
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <param name="version">软件版本</param>
        /// <returns>返回更新项目配置情况</returns>
        [RemoteMethod]
        UpgradeProject GetUpgradeProject(string product, string version);
        /// <summary>
        /// 获取指定版本的升级项目的升级URL
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <param name="version">版本号</param>
        /// <returns>返回指定版本升级项目的升级URL，如果服务器端不存在其路径返回空</returns>
        [RemoteMethod]
        string GetUpgradeUrl(string product, string version);
        /// <summary>
        /// 获取指定软件项目的更新历史信息
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <returns>返回所有更新配置情况列表</returns>
        [RemoteMethod]
        IList<UpgradeProject> GetUpgradeHistory(string product);
        /// <summary>
        /// 创建一个软件更新项目
        /// </summary>
        /// <param name="project">软件更新项目配置信息</param>
        [RemoteMethod]
        void CreateUpgradeProject(UpgradeProject project);
        /// <summary>
        /// 软件更新项目创建事件用于通知目前在线的客户端可以及时的进行系统更新
        /// </summary>
        [EventPublisher("TOPIC://Upgrade/UpgradeProjectCreated", EventPublisherScope.Global)]
        event EventHandler<EventArgs<UpgradeProject>> UpgradeProjectCreated;
        /// <summary>
        /// 删除一个软件更新项目
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <param name="version">软件版本</param>
        [RemoteMethod]
        void DeleteUpgradeProject(string product, string version);
        /// <summary>
        /// 删除指定名称软件的所有更新项目
        /// </summary>
        /// <param name="product">项目名称</param>
        [RemoteMethod]
        void DeleteUpgradeProjects(string product);
        /// <summary>
        /// 创建更新服务所用的虚拟目录
        /// </summary>
        /// <param name="server">服务器名称</param>
        /// <param name="proj">更新项目</param>
        /// <returns>如果创建成功则返回true，否则为false</returns>
        [RemoteMethod]
        string CreateVirtualDirectory(string server, UpgradeProject proj);
        /// <summary>
        /// 删除指定的虚拟目录
        /// </summary>
        /// <param name="server">服务器名称</param>
        /// <param name="vdName">虚拟目录</param>
        [RemoteMethod]
        void DeleteVirtualDirectory(string server, string vdName);
    }
}
