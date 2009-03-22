using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 列表布局服务
    /// </summary>
    [RemoteService(ServiceType.Business)]
    public interface ILayoutService
    {
        /// <summary>
        /// 保存列表布局到数据库
        /// </summary>
        /// <param name="layout">布局信息</param>
        [RemoteMethod]
        void StoreLayout(Layout layout);
        /// <summary>
        /// 从数据库恢复列表布局信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="module">模块</param>
        /// <param name="appUri">应用程序路径</param>
        /// <returns>如果存在列表布局信息则返回其布局信息否则为空</returns>
        [RemoteMethod]
        Layout RestoreLayout(string user, string appUri);
    }
}
