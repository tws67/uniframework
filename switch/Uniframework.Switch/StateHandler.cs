using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.EventBroker;

namespace Uniframework.Switch
{
    /// <summary>
    /// ���лỰ����ί��
    /// </summary>
    /// <param name="session">��ǰ�Ự</param>
    /// <returns>���ش���������ϸ�ķ��ؽ����ο�<seealso cref="SwitchStatus"/></returns>
    public delegate SwitchStatus SessionHandler(ICallSession session);

    /// <summary>
    /// �Ự״̬�¼������װ��
    /// </summary>
    public sealed class StateHandler
    {
        /// <summary>
        /// �Ự��ʼ���¼�
        /// </summary>
        public SessionHandler Initialize;
        /// <summary>
        /// �Ự�����¼�
        /// </summary>
        public SessionHandler Ring;
        /// <summary>
        /// �ű����û��Զ���ӿ�ִ���¼�
        /// </summary>
        public SessionHandler Execute;
        /// <summary>
        /// �Ự�Ҷ��¼�
        /// </summary>
        public SessionHandler Hangup;
        /// <summary>
        /// �ỰLoopback�¼�
        /// </summary>
        public SessionHandler Loopback;
        /// <summary>
        /// �Ựת���¼�
        /// </summary>
        public SessionHandler Transfer;
        /// <summary>
        /// �Ự�����¼�
        /// </summary>
        public SessionHandler Hold;
        /// <summary>
        /// �Ự�����¼�
        /// </summary>
        public SessionHandler Hibernate;
        /// <summary>
        /// ͨ��״̬�仯�¼�
        /// </summary>
        [EventPublication(SwitchEventNames.ChannelStatusChangedEvent, PublicationScope.Global)]
        public event EventHandler<EventArgs<IChannel>> ChannelStatusChanged;

        #region �¼�������

        internal void OnChannelStatusChanged(object sender, EventArgs<IChannel> e)
        {
            if (ChannelStatusChanged != null)
                ChannelStatusChanged(sender, e);
        }

        #endregion
    }
}
