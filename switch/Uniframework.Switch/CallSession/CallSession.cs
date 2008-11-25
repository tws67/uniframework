using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.Utility;

namespace Uniframework.Switch
{
    /// <summary>
    /// ���ӻỰ���ڱ�ʶÿһ������
    /// </summary>
    public class CallSession : ICallSession
    {
        #region ICallSession Members
        /// <summary>
        /// �Ự��ʶ
        /// </summary>
        public String CallID
        {
            get { return callID; }
        }
        /// <summary>
        /// �Ự����
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        /// <summary>
        /// �Ự��־
        /// </summary>
        public CallSessionFlag Sessionflags
        {
            get { return sessionflags; }
        }
        /// <summary>
        /// �Ự��������ͨ��
        /// </summary>
        public IChannel Channel
        {
            get { return channel; }
        }

        //public IEndpoint Endpoint
        //{
        //    get { return endpoint; }
        //}
        /// <summary>
        /// �Ự��������Profile��Ϣ������ʵ��Ӧ�ò������߼�������
        /// </summary>
        public CallerProfile Profile
        {
            get { return profile; }
        }
        /// <summary>
        /// �¼�������
        /// </summary>
        public StateHandler StateHandler
        {
            get { return stateHandler; }
        }

        #endregion

        public void SessionStart()
        {
            ChannelState state = ChannelState.NEW, laststate = ChannelState.HANGUP, midstate = ChannelState.DONE, endstate;
            IChannel chnl = Channel;
            Guard.ArgumentNotNull(chnl, "Channel");

            while ((state = chnl.ChannelState) != ChannelState.DONE)
            {
                midstate = state;
                switch (state)
                { 
                    case ChannelState.NEW :
                        chnl.Logger.Debug("Session enter NEW state," + this.ToString());

                        break;

                    case ChannelState.INIT :
                        chnl.Logger.Debug("Session enter INIT state," + this.ToString());

                        break;

                    case ChannelState.RING :
                        chnl.Logger.Debug("Session enter RING state," + this.ToString());

                        break;

                    case ChannelState.EXECUTE :
                        chnl.Logger.Debug("Session enter EXECUTE state," + this.ToString());

                        break;

                    case ChannelState.HANGUP :
                        chnl.Logger.Debug("Session enter HANGUP state," + this.ToString());

                        break;

                    case ChannelState.LOOPBACK :
                        chnl.Logger.Debug("Session enter LOOPBACK state," + this.ToString());

                        break;

                    case ChannelState.TRANSFER :
                        chnl.Logger.Debug("Session enter TRANSFER state," + this.ToString());

                        break;

                    case ChannelState.DONE :
                        chnl.Logger.Debug("Session enter DONE state," + this.ToString());

                        break;
                }
            }
        }

        public override string ToString()
        {
            return "CallID : " + CallID + ", Session's ChannelID is " + Channel.ChannelID.ToString();
        }
        #region Member fields

        private String callID = Guid.NewGuid().ToString();
        private String name = String.Empty;
        private CallSessionFlag sessionflags = CallSessionFlag.NONE;
        private IChannel channel = null;
        //private IEndpoint endpoint = null;
        private CallerProfile profile = new CallerProfile();
        private StateHandler stateHandler = new StateHandler();

        private Boolean threadRunning = false;

        #endregion
    }
}
