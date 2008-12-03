using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework
{
    public enum SendOption
    {
        All,
        Personal
    }

    /// <summary>
    /// 聊天信息类
    /// </summary>
    [Serializable]
    public class ChatData
    {
        private SendOption sendto = SendOption.All;
        private string receivedSessionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatData"/> class.
        /// </summary>
        public ChatData() { }

        #region Members

        public string IPAddress { get; set; }
        public string Message { get; set; }
        public string ReceivedSessionID {
            get
            {
                if (sendto != SendOption.Personal) {
                    throw new UniframeworkException("获取ReceivedSessionId属生必须将SentOption设为Personal");
                }
                return receivedSessionId;
            }
            set { receivedSessionId = value; } 
        }
        public SendOption SendOption { get { return sendto; } set { sendto = value; } }
        public string UserAlias { get; set; }
        public string UserName { get; set; }
        public string UserSessionID { get; set; }

        #endregion
    }
}
