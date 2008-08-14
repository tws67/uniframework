using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 事件订阅者注册接口，实现订阅者自己向事件分配器注册或注销服务
    /// </summary>
    public interface ISubscripterRegister
    {
        /// <summary>
        /// 订阅者名字
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 向事件分配器注册自己
        /// </summary>
        void Register();
        /// <summary>
        /// 从事件分配器注销自己
        /// </summary>
        void UnRegister();
    }
}
