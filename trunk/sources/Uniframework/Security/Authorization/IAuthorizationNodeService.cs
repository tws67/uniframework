using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 授权节点服务接口
    /// </summary>
    [RemoteService(ServiceType.Infrustructure)]
    public interface IAuthorizationNodeService
    {
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
        IList<AuthorizationNode> GetAll();
    }
}
