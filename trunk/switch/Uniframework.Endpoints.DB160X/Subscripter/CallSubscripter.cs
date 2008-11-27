using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Uniframework.SmartClient;

namespace Uniframework.Switch.Endpoints.DB160X
{
    /// <summary>
    /// 呼叫事件订阅者
    /// </summary>
    internal class CallSubscripter : ISubscripterRegister, ICallHandler
    {
        private readonly static string EventDispatcherName = "Call";
        private string name = string.Empty;
        private readonly IChannel channel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallSubscripter"/> class.
        /// </summary>
        /// <param name="chnl">The CHNL.</param>
        public CallSubscripter(AbstractChannel chnl) 
        {
            name = "Call Subscripter";
            channel = chnl;
        }

        #region ICallHandler Members

        /// <summary>
        /// 响应呼入/出事件，当有呼入/出时分配器将会通知所有的通道，对于订阅者可以通过通道编号判断是否是
        /// 自己所关心的事件，不关心的事件只需要简单的抛弃就可以了。
        /// </summary>
        /// <param name="chnlno">通道编号</param>
        /// <param name="callNumber">主叫电话号码</param>
        public void Call(int chnlno, string callNumber)
        {
            // 抛弃不属于通道的呼入/出事件
            if (chnlno != channel.ChannelID || channel == null 
                || (channel.CurrentStatus <= ChannelStatus.INIT || channel.CurrentStatus >= ChannelStatus.DONE))
                return;

            MethodInfo method = typeof(AbstractChannel).GetMethod("OnCall", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null && channel.CurrentStatus == ChannelStatus.IDLE)
                try
                {
                    DynamicInvokerHandler invoker = DynamicInvoker.GetMethodInvoker(method);
                    channel.CurrentStatus = ChannelStatus.EXECUTE;
                    channel.Logger.Debug(String.Format("反射目标通道 {0} 的呼入/呼出事件处理程序，OnCall", channel.ChannelID));
                    //method.Invoke(channel, new object[] { channel, 
                    //    new CallEventArgs(channel.ChannelType == ChannelType.TRUNK ? CallType.In : CallType.Out, callNumber) });
                    invoker.Invoke(channel, 
                        new object[] { new CallEventArgs(channel.ChannelType == ChannelType.TRUNK ? CallType.In : CallType.Out, callNumber) });
                }
                catch (Exception ex)
                {
                    channel.ResetChannel();
                    channel.Logger.Error(String.Format("反射目标通道 {0} 的呼入/呼出事件处理程序失败", channel.ChannelID), ex);
                    throw ex;
                }
        }

        #endregion

        #region ISubscripterRegister Members

        /// <summary>
        /// 向事件分配器注册自己
        /// </summary>
        public void Register()
        {
            IEventService eventService = channel.Driver.WorkItem.Services.Get<IEventService>();
            if (eventService != null)
            {
                EventDispatcher dispatcher = eventService.GetEventDispatcher(EventDispatcherName);
                if (dispatcher != null)
                {
                    dispatcher.Subject.Register(this);
                }
            }
        }

        /// <summary>
        /// 从事件分配器注销自己
        /// </summary>
        public void UnRegister()
        {
            IEventService eventService = channel.Driver.WorkItem.Services.Get<IEventService>();
            if (eventService != null)
            {
                EventDispatcher dispatcher = eventService.GetEventDispatcher(EventDispatcherName);
                if (dispatcher != null)
                {
                    dispatcher.Subject.UnRegister(this);
                }
            }
        }

        /// <summary>
        /// 订阅者名字
        /// </summary>
        /// <value></value>
        public string Name
        {
            get { return name; }
        }

        #endregion
    }
}
