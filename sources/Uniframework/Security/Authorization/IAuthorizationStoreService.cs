using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统授权存储服务
    /// </summary>
    [RemoteService("系统授权存储服务", ServiceType.Infrustructure, ServiceScope.Global)]
    public interface IAuthorizationStoreService
    {
        /// <summary>
        /// 返回指定用户所有的授权资源信息列表
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>根据用户所属的角色返回其拥有的授权资源信息列表</returns>
        [RemoteMethod]
        List<AuthorizationResource> GetAuthorizationResources(string username);
        /// <summary>
        /// 返回指定角色的授权资源信息
        /// </summary>
        /// <param name="rolename">角色名称</param>
        /// <returns>如果存在指定角色的资源信息则返回，否则返回null</returns>
        [RemoteMethod]
        AuthorizationResource GetAuthorizationResource(string rolename);
        /// <summary>
        /// 保存授权资源信息
        /// </summary>
        /// <param name="ar">需要保存的授权资源信息</param>
        [RemoteMethod]
        void Save(AuthorizationResource ar);
        /// <summary>
        /// 从存储中删除指定角色的授权资源信息
        /// </summary>
        /// <param name="rolename">角色名称</param>
        [RemoteMethod]
        void Delete(string rolename);
        /// <summary>
        /// 系统角色授权信息变化事件，用于及时通知客户端进行更新
        /// </summary>
        [EventPublisher("TOPIC://Authorization/AuthorizationChanged")]
        event EventHandler<EventArgs<string>> AuthorizationChanged;
    }
}
