using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Uniframework.Security
{
    /// <summary>
    /// 授权操作服务
    /// </summary>
    [RemoteService(ServiceType.Infrustructure)]
    public interface IAuthorizationCommandService
    {
        /// <summary>
        /// 保存操作命令
        /// </summary>
        /// <param name="command">命令</param>
        [RemoteMethod]
        void Save(AuthorizationCommand command);
        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="command">命令</param>
        [RemoteMethod]
        void Delete(AuthorizationCommand command);
        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="commandUri">命令Uri</param>
        [RemoteMethod]
        void Delete(string commandUri);
        /// <summary>
        /// 清除所有的操作命令
        /// </summary>
        [RemoteMethod]
        void Clear();
        /// <summary>
        /// 检查指定的授权操作是否存在
        /// </summary>
        /// <param name="command">授权操作</param>
        /// <returns>如果存在返回<see cref="true"/>否则为<see cref="false"/></returns>
        [RemoteMethod]
        bool Exists(AuthorizationCommand command);
        /// <summary>
        /// 检查指定名称和操作项的授权操作是否存在于当前服务中
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="commandUri">操作项</param>
        /// <returns>如果存在返回<see cref="true"/>否则为<see cref="false"/></returns>
        [RemoteMethod]
        bool Exists(string name, string commandUri);
        /// <summary>
        /// 获取操作命令
        /// </summary>
        /// <param name="name">命令名称</param>
        /// <param name="commandUri">命令URI.</param>
        /// <returns></returns>
        [RemoteMethod]
        AuthorizationCommand Get(string name, string commandUri);
        /// <summary>
        /// 获取命令列表
        /// </summary>
        /// <param name="category">命令分组</param>
        /// <returns>命令列表</returns>
        [RemoteMethod]
        IList<AuthorizationCommand> Get(string category);
        /// <summary>
        /// 获取所有命令列表
        /// </summary>
        /// <returns>命令列表</returns>
        [RemoteMethod]
        IList<AuthorizationCommand> GetAll();
    }
}
