using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// �Ự��Ϣ
    /// </summary>
    public class CallSessionMessage
    {
        /// <summary>
        /// ��ϢԴ
        /// </summary>
        public String From { get { return from; } }
        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public MessageType MessageID { get { return messageid; } }
        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public Object[] Args { get { return args; } }
        /// <summary>
        /// Ӧ����Ϣ
        /// </summary>
        public Object[] Replies { get { return replies; } }

        /// <summary>
        /// �Ự��Ϣ���캯��
        /// </summary>
        /// <param name="from">��ϢԴ</param>
        /// <param name="messageid">��Ϣ����</param>
        public CallSessionMessage(String from, MessageType messageid)
        {
            this.from = from;
            this.messageid = messageid;
        }

        #region Member fields

        private String from = String.Empty;
        private MessageType messageid = MessageType.REDIRECT_AUDIO;
        private Object[] args;
        private Object[] replies;
        #endregion
    }
}
