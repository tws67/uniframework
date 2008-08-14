using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 客户端异步事件分配服务
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// 获取指定名称的事件分配器
        /// </summary>
        /// <param name="eventName">事件分配器名称</param>
        /// <returns>返回指定名称的事件分配器，否则返回null</returns>
        EventDispatcher GetEventDispatcher(string eventName);
        /// <summary>
        /// 向系统注册事件分配器
        /// </summary>
        /// <param name="eventName">事件分配器名称</param>
        /// <param name="eventDispatcher">事件分配器</param>
        void RegisterEventDispatcher(string eventName, EventDispatcher eventDispatcher);
        /// <summary>
        /// 向系统注销事件分配器
        /// </summary>
        /// <param name="eventName">事件分配器名称</param>
        void UnRegisterEventDispatcher(string eventName);
        /// <summary>
        /// 清除所有的事件分配器
        /// </summary>
        void ClearEventDispatcher();
        /// <summary>
        /// 启动指定的事件分配器
        /// </summary>
        /// <param name="eventName">事件分配器名称</param>
        void StartEventDispatcher(string eventName);
        /// <summary>
        /// 停止指定的事件分配器
        /// </summary>
        /// <param name="eventName">事件分配器名称</param>
        void StopEventDispatcher(string eventName);
        /// <summary>
        /// 注册一个事件订阅者
        /// </summary>
        /// <param name="subscripter">事件订阅者</param>
        void RegisterSubscripter(ISubscripterRegister subscripter);
        /// <summary>
        /// 注销事件订阅者
        /// </summary>
        /// <param name="subscripterName">事件订阅者名称</param>
        void UnRegisterSubscripter(string subscripterName);
    }
}
