using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 框架上下文菜单服务，用于注册、注销和获取指定路径上下文菜单
    /// </summary>
    public interface IContentMenuService
    {
        /// <summary>
        /// Registers the content menu.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        void RegisterContentMenu(string name, object content);
        /// <summary>
        /// Uns the register content menu.
        /// </summary>
        /// <param name="name">The name.</param>
        void UnRegisterContentMenu(string name);
        /// <summary>
        /// Gets the content menu.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        object GetContentMenu(string name);
    }
}
