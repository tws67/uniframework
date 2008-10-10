using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 浏览器接口
    /// </summary>
    public interface IBrowserService
    {
        //void Register(IBrowseHandle handle);
        /// <summary>
        /// Gotoes the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        void Goto(string address);
        /// <summary>
        /// Gets the home URI.
        /// </summary>
        /// <value>The home URI.</value>
        string HomeUri { get; }
    }

    /// <summary>
    /// 浏览器
    /// </summary>
    public interface IBrowser
    {
        /// <summary>
        /// 激活浏览器事件
        /// </summary>
        event EventHandler Actived;
        /// <summary>
        /// 浏览器由活动状态转入非活动状态事件
        /// </summary>
        event EventHandler Deactived;
        /// <summary>
        /// 浏览器销毁事件
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Gets a value indicating whether this instance can back.
        /// </summary>
        /// <value><c>true</c> if this instance can back; otherwise, <c>false</c>.</value>
        bool CanBack { get; }
        /// <summary>
        /// Backs this instance.
        /// </summary>
        void Back();
        /// <summary>
        /// Gets a value indicating whether this instance can forward.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can forward; otherwise, <c>false</c>.
        /// </value>
        bool CanForward { get; }
        /// <summary>
        /// Forwards this instance.
        /// </summary>
        void Forward();
        /// <summary>
        /// Gets a value indicating whether this instance can stop.
        /// </summary>
        /// <value><c>true</c> if this instance can stop; otherwise, <c>false</c>.</value>
        bool CanStop { get; }
        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
        /// <summary>
        /// Gets a value indicating whether this instance can home.
        /// </summary>
        /// <value><c>true</c> if this instance can home; otherwise, <c>false</c>.</value>
        bool CanHome { get; }
        /// <summary>
        /// Homes this instance.
        /// </summary>
        void Home();
        /// <summary>
        /// Gets a value indicating whether this instance can refresh.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can refresh; otherwise, <c>false</c>.
        /// </value>
        bool CanRefresh { get; }
        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        void Refresh();
    }
}
