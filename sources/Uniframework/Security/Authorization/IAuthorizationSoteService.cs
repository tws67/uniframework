﻿using System;
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

        #region 授权节点操作

        /// <summary>
        /// 保存授权节点
        /// </summary>
        /// <param name="node">授权节点</param>
        [RemoteMethod]
        void Save(AuthorizationNode node);
        /// <summary>
        /// 删除授权节点
        /// </summary>
        /// <param name="node">授权节点</param>
        [RemoteMethod]
        void Delete(AuthorizationNode node);
        /// <summary>
        /// 删除所有的授权节点信息
        /// </summary>
        [RemoteMethod]
        void Clear();
        /// <summary>
        /// 获取系统中所有的授权节点信息
        /// </summary>
        /// <returns>授权节点列表</returns>
        [RemoteMethod]
        IList<AuthorizationNode> GetAuthorizationNodes();

        #endregion

        #region 命令操作
        /// <summary>
        /// 保存操作命令
        /// </summary>
        /// <param name="command">命令</param>
        [RemoteMethod]
        void SaveCommand(AuthorizationCommand command);
        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="command">命令</param>
        [RemoteMethod]
        void DeleteCommand(AuthorizationCommand command);
        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="commandUri">命令Uri</param>
        [RemoteMethod]
        void DeleteCommand(string commandUri);
        /// <summary>
        /// 清除所有的操作命令
        /// </summary>
        [RemoteMethod]
        void ClearCommand();
        /// <summary>
        /// 获取操作命令
        /// </summary>
        /// <param name="name">命令名称</param>
        /// <param name="commandUri">命令URI.</param>
        /// <returns></returns>
        [RemoteMethod]
        AuthorizationCommand GetCommand(string name, string commandUri);
        /// <summary>
        /// 获取命令列表
        /// </summary>
        /// <param name="category">命令分组</param>
        /// <returns>命令列表</returns>
        [RemoteMethod]
        IList<AuthorizationCommand> GetCommand(string category);
        /// <summary>
        /// 获取所有命令列表
        /// </summary>
        /// <returns>命令列表</returns>
        [RemoteMethod]
        IList<AuthorizationCommand> GetCommands();
        #endregion
    }
}
