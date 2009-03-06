using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 框架授权信息服务器端存储服务接口
    /// </summary>
    [RemoteService("授权信息存储接口", ServiceType.Infrustructure, ServiceScope.Global)]
    public interface IAuthorizationStoreService
    {
        #region 角色授权操作

        /// <summary>
        /// 获取指定用户的授权信息
        /// </summary>
        /// <param name="username">用户名称</param>
        /// <returns>返回特定用户的授权信息</returns>
        [RemoteMethod]
        IList<AuthorizationStore> GetAuthorizationsByUser(string username);
        /// <summary>
        /// 获取指定角色的授权信息
        /// </summary>
        /// <param name="role">角色名称</param>
        /// <returns>特定角色的授权信息</returns>
        [RemoteMethod]
        IList<AuthorizationStore> GetAuthorizationsByRole(string role);
        /// <summary>
        /// 保存授权信息
        /// </summary>
        /// <param name="authorizationStore">授权信息</param>
        [RemoteMethod]
        void SaveAuthorization(AuthorizationStore authorizationStore);
        /// <summary>
        /// 从后台存储中删除指定角色的授权信息
        /// </summary>
        /// <param name="role">The role.</param>
        [RemoteMethod]
        void DeleteAuthorization(string role);
        /// <summary>
        /// 当某角色所拥有的授权信息发生变化时触发此事件，用于通知客户端实时处理用户的权限
        /// </summary>
        [EventPublisher(GlobalEventNames.AuthorizationChanged, EventPublisherScope.Global)]
        event EventHandler<EventArgs<string>> AuthorizationChanged;

        #endregion
    }
}
