using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// ����Զ�̵������ݰ�
    /// </summary>
    [Serializable]
    public class NetworkInvokePackage
    {
        private NetworkInvokeType invokeType = NetworkInvokeType.Unknown;
        private string sessionId;
        private ClientType clientType = ClientType.SmartClient;
        private HybridDictionary context;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="invokeType">��������</param>
        /// <param name="sessionId">�����߻Ự��ʶ</param>
        public NetworkInvokePackage(NetworkInvokeType invokeType, string sessionId)
        {
            this.invokeType = invokeType;
            this.sessionId = sessionId;
            context = new HybridDictionary();
        }

        /// <summary>
        /// ���캯�������أ�
        /// </summary>
        /// <param name="invokeType">��������</param>
        /// <param name="sessionId">�����߻Ự��ʶ</param>
        /// <param name="clientType">�ͻ�������</param>
        public NetworkInvokePackage(NetworkInvokeType invokeType, string sessionId, ClientType clientType)
            : this(invokeType, sessionId)
        {
            this.clientType = clientType;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public NetworkInvokeType InvokeType
        {
            get { return invokeType; }
        }

        /// <summary>
        /// �����߻Ự��ʶ
        /// </summary>
        public string SessionId
        {
            get { return sessionId; }
        }

        /// <summary>
        /// �ͻ�������
        /// </summary>
        public ClientType ClientType
        {
            get { return clientType; }
        }

        /// <summary>
        /// ���ð����������ڴ����صĵ��õ���ϸ��Ϣ���������˽��յ����ð��Դ˽��н���
        /// </summary>
        public HybridDictionary Context
        {
            get { return context; }
        }
    }

    /// <summary>
    /// ����Զ�̵�������
    /// </summary>
    [Serializable]
    public enum NetworkInvokeType
    { 
        /// <summary>
        /// δ֪����
        /// </summary>
        Unknown,
        /// <summary>
        /// Ping
        /// </summary>
        Ping,
        /// <summary>
        /// Զ�̵���
        /// </summary>
        Invoke,
        /// <summary>
        /// ע��Ự
        /// </summary>
        Register,
        /// <summary>
        /// ��ȡ����
        /// </summary>
        RemoteService
    }

    /// <summary>
    /// �ͻ�������
    /// </summary>
    [Serializable]
    public enum ClientType
    { 
        /// <summary>
        /// ���ܿͻ���
        /// </summary>
        SmartClient,
        /// <summary>
        /// �ƶ��ͻ���
        /// </summary>
        Mobile
    }
}
