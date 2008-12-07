using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �Ự״̬
    /// </summary>
    [Serializable]
    public class SessionState : ISessionState
    {
        private readonly int DEFAULT_TIMEOUT = 180; // Ĭ�ϳ�ʱֵΪ3����

        private string sessionId;
        private int activeTime = 0;
        private int timeout;
        private HybridDictionary context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionState"/> class.
        /// </summary>
        public SessionState()
        {
            timeout = DEFAULT_TIMEOUT;
            context = new HybridDictionary();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionState"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public SessionState(string sessionId, int timeout)
            : this()
        {
            this.sessionId = sessionId;
            this.timeout = timeout;
        }


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
        /// Gets or sets the session id.
        /// </summary>
        /// <value>The session id.</value>
        public string SessionId
        {
            get { return sessionId; }
            set { sessionId = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[object key]
        {
            get {
                return context[key];
            }

            set {
                context[key] = value;
                OnContextChanged();
            }
        }

        /// <summary>
        /// �Ƴ����лỰ״̬
        /// </summary>
        public void RemoveAll()
        {
            object sessionId = context[SessionVariables.SESSION_ID];
            object username = context[SessionVariables.SESSION_CURRENT_USER];
            object ipAddress = context[SessionVariables.SESSION_CLIENT_ADDRESS];
            object encryptKey = context[SessionVariables.SESSION_ENCRYPTKEY];
            object loginTime = context[SessionVariables.SESSION_LOGIN_TIME];

            context.Clear();

            context[SessionVariables.SESSION_ID] = sessionId;
            context[SessionVariables.SESSION_CURRENT_USER] = username;
            context[SessionVariables.SESSION_CLIENT_ADDRESS] = ipAddress;
            context[SessionVariables.SESSION_ENCRYPTKEY] = encryptKey;
            context[SessionVariables.SESSION_LOGIN_TIME] = loginTime;
            OnContextChanged();
        }

        /// <summary>
        /// �Ƴ�ָ����ʶ��ʵ��
        /// </summary>
        /// <param name="key">��ʶ</param>
        public void Remove(object key)
        {
            context.Remove(key);
            OnContextChanged();
        }

        /// <summary>
        /// �ж��Ƿ���ڸ�keyֵ
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(object key)
        {
            return context.Contains(key);
        }

        #endregion

        #region Assistant function

        /// <summary>
        /// �Ự״̬�����ı仯�¼�
        /// </summary>
        public event EventHandler ContextChanged;

        protected void OnContextChanged()
        {
            if (ContextChanged != null)
                ContextChanged(this, EventArgs.Empty);
        }

        #endregion

    }
}
