using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.EventBroker;

namespace Uniframework.Switch
{
    /// <summary>
    /// 呼叫会话处理委托
    /// </summary>
    /// <param name="session">当前会话</param>
    /// <returns>返回处理结果，详细的返回结果请参考<seealso cref="SwitchStatus"/></returns>
    public delegate SwitchStatus SessionHandler(ICallSession session);

    /// <summary>
    /// 会话状态事件处理包装类
    /// </summary>
    public sealed class StateHandler
    {
        /// <summary>
        /// 会话初始化事件
        /// </summary>
        public SessionHandler Initialize;
        /// <summary>
        /// 会话振铃事件
        /// </summary>
        public SessionHandler Ring;
        /// <summary>
        /// 脚本或用户自定义接口执行事件
        /// </summary>
        public SessionHandler Execute;
        /// <summary>
        /// 会话挂断事件
        /// </summary>
        public SessionHandler Hangup;
        /// <summary>
        /// 会话Loopback事件
        /// </summary>
        public SessionHandler Loopback;
        /// <summary>
        /// 会话转接事件
        /// </summary>
        public SessionHandler Transfer;
        /// <summary>
        /// 会话保持事件
        /// </summary>
        public SessionHandler Hold;
        /// <summary>
        /// 会话挂起事件
        /// </summary>
        public SessionHandler Hibernate;
        /// <summary>
        /// 通道状态变化事件
        /// </summary>
        [EventPublication(SwitchEventNames.ChannelStatusChangedEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<IChannel>> ChannelStatusChanged;

        #region 事件触发器

        internal void OnChannelStatusChanged(object sender, EventArgs<IChannel> e)
        {
            if (ChannelStatusChanged != null)
                ChannelStatusChanged(sender, e);
        }

        #endregion
    }
}
