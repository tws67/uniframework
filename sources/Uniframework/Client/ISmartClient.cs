using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// 网络连接状态
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// 在线
        /// </summary>
        Online = 0,
        /// <summary>
        /// 离线
        /// </summary>
        Offline = 1,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// 网络连接状态改变的委托
    /// </summary>
    /// <param name="state"></param>
    public delegate void ConnectionStateChangeHandler(ConnectionState state);

    /// <summary>
    /// SmartClient离线特性接口
    /// </summary>
    public interface ISmartClient
    {
        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event ConnectionStateChangeHandler ConnectionStateChanged;
    }
}
