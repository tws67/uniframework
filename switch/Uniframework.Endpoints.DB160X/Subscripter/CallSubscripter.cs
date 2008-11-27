using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Uniframework.SmartClient;

namespace Uniframework.Switch.Endpoints.DB160X
{
    /// <summary>
    /// �����¼�������
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
        /// ��Ӧ����/���¼������к���/��ʱ����������֪ͨ���е�ͨ�������ڶ����߿���ͨ��ͨ������ж��Ƿ���
        /// �Լ������ĵ��¼��������ĵ��¼�ֻ��Ҫ�򵥵������Ϳ����ˡ�
        /// </summary>
        /// <param name="chnlno">ͨ�����</param>
        /// <param name="callNumber">���е绰����</param>
        public void Call(int chnlno, string callNumber)
        {
            // ����������ͨ���ĺ���/���¼�
            if (chnlno != channel.ChannelID || channel == null 
                || (channel.CurrentStatus <= ChannelStatus.INIT || channel.CurrentStatus >= ChannelStatus.DONE))
                return;

            MethodInfo method = typeof(AbstractChannel).GetMethod("OnCall", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null && channel.CurrentStatus == ChannelStatus.IDLE)
                try
                {
                    DynamicInvokerHandler invoker = DynamicInvoker.GetMethodInvoker(method);
                    channel.CurrentStatus = ChannelStatus.EXECUTE;
                    channel.Logger.Debug(String.Format("����Ŀ��ͨ�� {0} �ĺ���/�����¼��������OnCall", channel.ChannelID));
                    //method.Invoke(channel, new object[] { channel, 
                    //    new CallEventArgs(channel.ChannelType == ChannelType.TRUNK ? CallType.In : CallType.Out, callNumber) });
                    invoker.Invoke(channel, 
                        new object[] { new CallEventArgs(channel.ChannelType == ChannelType.TRUNK ? CallType.In : CallType.Out, callNumber) });
                }
                catch (Exception ex)
                {
                    channel.ResetChannel();
                    channel.Logger.Error(String.Format("����Ŀ��ͨ�� {0} �ĺ���/�����¼��������ʧ��", channel.ChannelID), ex);
                    throw ex;
                }
        }

        #endregion

        #region ISubscripterRegister Members

        /// <summary>
        /// ���¼�������ע���Լ�
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
        /// ���¼�������ע���Լ�
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
        /// ����������
        /// </summary>
        /// <value></value>
        public string Name
        {
            get { return name; }
        }

        #endregion
    }
}
