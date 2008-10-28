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

        /// <summary>
        /// 显示状态栏的帮助信息.
        /// </summary>
        /// <param name="info">信息</param>
        void ShowHint(string info);
        /// <summary>
        /// 显示自定义面板1信息.
        /// </summary>
        /// <param name="info">信息</param>
        void ShowCustomPanel1(string info);
        /// <summary>
        /// 显示自定义面板2信息
        /// </summary>
        /// <param name="info">信息</param>
        void ShowCustomPanel2(string info);
        /// <summary>
        /// 显示进度条
        /// </summary>
        /// <param name="position">进度</param>
        void ChangeProgress(int position);
    }
}
