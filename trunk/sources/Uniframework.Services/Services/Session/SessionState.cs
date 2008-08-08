using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 会话状态
    /// </summary>
    [Serializable]
    public class SessionState : ISessionState
    {
        private int activeTime;
        private int timeout;
        private Hashtable ht;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionState"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="value">The value.</param>
        public SessionState(int timeout, Hashtable value)
        {
            this.timeout = timeout;
            this.ht = value;
        }

        #region Assistant function
        
        /// <summary>
        /// 值变化事件
        /// </summary>
        [NonSerialized]
        private EventHandler valueChangedHandler;
        
        public event EventHandler ValueChanged
        {
            add
            {
                valueChangedHandler += value;
            }
            remove
            {
                valueChangedHandler -= value;
            }
        }

        private void Notify()
        {
            if (this.valueChangedHandler != null)
                valueChangedHandler(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public void Activate()
        {
            activeTime = 0;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is timeouted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is timeouted; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimeouted
        {
            get
            {
                activeTime++;
                return activeTime > timeout;
            }
        }

        #region ISessionState Members

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[object key]
        {
            get
            {
                return ht[key];
            }
            set
            {
                ht[key] = value;
                Notify();
            }
        }

        /// <summary>
        /// 移除所有会话状态
        /// </summary>
        public void RemoveAll()
        {
            object sessionID = ht[ServerVariables.SESSION_ID];
            object username = ht[ServerVariables.CURRENT_USER];
            object ipAddress = ht[ServerVariables.CLIENT_ADDRESS];
            object encryptKey = ht[ServerVariables.ENCRYPT_KEY];
            object loggingTime = ht[ServerVariables.LOGGING_TIME];
            ht.Clear();
            ht[ServerVariables.SESSION_ID] = sessionID;
            ht[ServerVariables.CURRENT_USER] = username;
            ht[ServerVariables.CLIENT_ADDRESS] = ipAddress;
            ht[ServerVariables.ENCRYPT_KEY] = encryptKey;
            ht[ServerVariables.LOGGING_TIME] = loggingTime;
            Notify();
        }

        /// <summary>
        /// 移除指定标识的实例
        /// </summary>
        /// <param name="key">标识</param>
        public void Remove(object key)
        {
            ht.Remove(key);
            Notify();
        }

        /// <summary>
        /// 判断是否存在该key值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(object key)
        {
            return ht.Contains(key);
        }

        #endregion
    }
}
