using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uniframework.Services
{
    #region SubscriberLocation
    /// <summary>
    /// ������λ��ö��
    /// </summary>
    public enum SubscriberLocation
    {
        /// <summary>
        /// ����
        /// </summary>
        Local,

        /// <summary>
        /// �ͻ���
        /// </summary>
        Client,

        /// <summary>
        /// ��ҳ
        /// </summary>
        Web
    }

    #endregion

    #region IEventDispatcher

    /// <summary>
    /// ϵͳ�¼��ַ����ӿ�
    /// </summary>
    [RemoteService("ϵͳ�¼��ַ���", ServiceType.System)]
    public interface IEventDispatcher
    {
        #region IEventDispatcher Member
        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="description">�¼�����</param>
        /// <param name="eventScope">�¼���Χ</param>
        /// <param name="eventInfo">EventInfoʵ��</param>
        void RegisterEventPublisher(string topic, string description, EventPublisherScope eventScope, EventInfo eventInfo);

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="methodInfo">MethodInfoʵ��</param>
        /// <param name="serviceType">��������</param>
        void RegisterEventSubscriber(string topic, MethodInfo methodInfo, Type serviceType);

        /// <summary>
        /// ע���ⲿ���¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="location">������λ��</param>
        [RemoteMethod("ע���ⲿ�¼�������")]
        void RegisterOuterEventSubscriber(string topic, SubscriberLocation location);

        /// <summary>
        /// ע���ⲿ�¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        [RemoteMethod("ע���ⲿ�¼�������")]
        void UnRegisterAnOuterEventSubscriber(string topic);

        /// <summary>
        /// ע�������ⲿ�¼�������
        /// </summary>
        void UnRegisterAllOuterEventSubscriber();

        /// <summary>
        /// ע��ָ���Ự�������ⲿ�¼�������
        /// </summary>
        /// <param name="sessionId">�ỰId</param>
        void UnRegisterAllOuterEventSubscriber(string sessionId);

        /// <summary>
        /// ��ȡ�����¼������ߵ�����
        /// </summary>
        /// <returns>�¼�����������</returns>
        EventPublisherInfo[] GetAllEventPublishers();

        /// <summary>
        /// ��ȡ�ⲿ�¼����
        /// </summary>
        /// <returns>�¼������Ϣ����</returns>
        [RemoteMethod("��ȡ�ⲿ�¼����")]
        EventResultData[] GetOuterEventResults();

        /// <summary>
        /// ���ӵ��ڲ��¼�
        /// </summary>
        void ConnectInnerEvent();

        #endregion
    }

    #endregion

    #region EventResultData

    /// <summary>
    /// �¼����
    /// </summary>
    [Serializable]
    public class EventResultData
    {
        string topic;
        EventArgs args;

        /// <summary>
        /// �޲ι��캯��
        /// </summary>
        public EventResultData()
            : this(string.Empty, null)
        { }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="args">�¼�����</param>
        public EventResultData(string topic, EventArgs args)
        {
            this.topic = topic;
            this.args = args;
        }

        /// <summary>
        /// �¼�Ψһ����
        /// </summary>
        public string Topic { get { return topic; } }

        /// <summary>
        /// �¼�����
        /// </summary>
        public EventArgs Args { get { return args; } }
    }

    #endregion

    #region EventPublisherInfo

    /// <summary>
    /// �¼���������Ϣ
    /// </summary>
    [Serializable]
    public class EventPublisherInfo
    {
        string topic;
        string description = "";
        EventInfo eventInfo;
        EventPublisherScope eventScope = EventPublisherScope.Global;
        Dictionary<string, EventSubscriberInfo> subscribers;

        /// <summary>
        /// �޲ι��캯��
        /// </summary>
        public EventPublisherInfo()
        { }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="description">�¼�����</param>
        /// <param name="scope">�¼���Χ</param>
        /// <param name="eventInfo">EventInfoʵ��</param>
        public EventPublisherInfo(string topic, string description, EventPublisherScope scope, EventInfo eventInfo)
        {
            this.topic = topic;
            this.description = description;
            this.eventScope = scope;
            this.eventInfo = eventInfo;
            subscribers = new Dictionary<string, EventSubscriberInfo>();
        }

        #region EventPublisherInfo Members

        /// <summary>
        /// ��ȡ�¼�Ψһ����
        /// </summary>
        public string Topic { get { return this.topic; } }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public string Description { get { return this.description; } }

        /// <summary>
        /// �¼���Χ
        /// </summary>
        public EventPublisherScope EventScope { get { return this.eventScope; } }

        /// <summary>
        /// EventInfo
        /// </summary>
        public EventInfo EventInfo { get { return this.eventInfo; } }

        #endregion

        /// <summary>
        /// ��Ӷ����ߵ�����������
        /// </summary>
        /// <param name="subscriberKey">�����߱�ʶ</param>
        /// <param name="subscriber">��������Ϣ</param>
        public void AddSubscriber(string subscriberKey, EventSubscriberInfo subscriber)
        {
            subscribers.Add(subscriberKey, subscriber);
        }

        /// <summary>
        /// �Ӷ������������Ƴ�ָ����ʶ�Ķ�����
        /// </summary>
        /// <param name="subscriberKey">�����߱�ʶ</param>
        public void RemoveSubscriber(string subscriberKey)
        {
            if (subscribers.ContainsKey(subscriberKey))
                subscribers.Remove(subscriberKey);
        }

        /// <summary>
        /// ���ָ����ʶ�Ķ������Ƿ��ڶ�����������
        /// </summary>
        /// <param name="subscriberKey">�����߱�ʶ</param>
        /// <returns></returns>
        public bool ContainsSubscriber(string subscriberKey)
        {
            return subscribers.ContainsKey(subscriberKey);
        }

        /// <summary>
        /// ����������
        /// </summary>
        public EventSubscriberInfo[] Subscribers
        {
            get
            {
                List<EventSubscriberInfo> subs = new List<EventSubscriberInfo>();
                foreach (EventSubscriberInfo si in subscribers.Values)
                {
                    subs.Add(si);
                }
                return subs.ToArray();
            }
        }
    }

    #endregion

    #region EventSubscriberInfo

    /// <summary>
    /// �¼���������Ϣ
    /// </summary>
    [Serializable]
    public class EventSubscriberInfo
    {
        private string topic;
        private Type serviceType;
        private MethodInfo methodInfo;
        private SubscriberLocation location;
        private string componentName;
        private string methodName;

        /// <summary>
        /// �޲ι��캯��
        /// </summary>
        public EventSubscriberInfo()
        { }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="methodInfo">MethodInfoʵ��</param>
        /// <param name="serviceType">��������</param>
        /// <param name="location">������λ��</param>
        public EventSubscriberInfo(string topic, MethodInfo methodInfo, Type serviceType, SubscriberLocation location)
        {
            this.topic = topic;
            this.serviceType = serviceType;
            this.methodInfo = methodInfo;
            this.location = location;
            methodName = methodInfo.Name;
            componentName = methodInfo.DeclaringType.Name;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="location">������λ��</param>
        /// <param name="componentName">�������</param>
        /// <param name="methodName">MethodInfoʵ��</param>
        public EventSubscriberInfo(string topic, SubscriberLocation location, string componentName, string methodName)
        {
            this.topic = topic;
            this.location = location;
            this.componentName = componentName;
            this.methodName = methodName;
        }
        
        #region EventSubscriberInfo Members

        /// <summary>
        /// ��ȡ�¼�Ψһ����
        /// </summary>
        public string Topic { get { return topic; } }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public Type ServiceType { get { return serviceType; } }

        /// <summary>
        /// ��ȡMethodInfoʵ��
        /// </summary>
        public MethodInfo MethodInfo { get { return methodInfo; } }

        /// <summary>
        /// ��ȡ������λ��
        /// </summary>
        public SubscriberLocation Location { get { return location; } }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        public string ComponentName { get { return componentName; } }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        public string MethodName { get { return methodName; } }

        #endregion 
    }

    #endregion
}
