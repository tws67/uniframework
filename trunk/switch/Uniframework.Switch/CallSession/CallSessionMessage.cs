using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// 会话消息
    /// </summary>
    public class CallSessionMessage
    {
        /// <summary>
        /// 消息源
        /// </summary>
        public String From { get { return from; } }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MessageID { get { return messageid; } }
        /// <summary>
        /// 消息参数
        /// </summary>
        public Object[] Args { get { return args; } }
        /// <summary>
        /// 应答信息
        /// </summary>
        public Object[] Replies { get { return replies; } }

        /// <summary>
        /// 会话消息构造函数
        /// </summary>
        /// <param name="from">消息源</param>
        /// <param name="messageid">消息类型</param>
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
