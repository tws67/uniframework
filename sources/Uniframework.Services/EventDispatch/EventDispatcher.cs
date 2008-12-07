using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

using Castle.MicroKernel;
using Uniframework.Db4o;
using Uniframework.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// ���������¼��ַ���������ע��/ע�����е��¼�����ͻ��˷ַ��¼�
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        private static readonly string dbName = "EventDispatcher.yap";

        private Dictionary<string, EventPublisherInfo> events;
        private Dictionary<string, EventSubscriberInfo> subscribers;
        private Dictionary<string, EventCollector> eventCollectors;
        private Dictionary<string, List<string>> outerSubscriberMapping;

        private IKernel kernel;
        private ILogger logger;
        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatcher"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="kernel">The kernel.</param>
        /// <param name="databaseService">The database service.</param>
        public EventDispatcher(ILoggerFactory log, IKernel kernel, IDb4oDatabaseService databaseService)
        {
            this.kernel = kernel;

            events = new Dictionary<string, EventPublisherInfo>();
            subscribers = new Dictionary<string, EventSubscriberInfo>();
            eventCollectors = new Dictionary<string, EventCollector>();
            logger = log.CreateLogger<EventDispatcher>("Framework");
            db = databaseService.Open(dbName);
            outerSubscriberMapping = new Dictionary<string, List<string>>();
        }

        #region Assistant function

        private void AddEventPublisher(EventPublisherInfo publisher)
        {
            if (events.ContainsKey(publisher.Topic)) 
                throw new ArgumentException("�ӿ� [" + events[publisher.Topic].EventInfo.DeclaringType.ToString() + "] �ͽӿ� [" + publisher.EventInfo.DeclaringType.ToString() + "] �ظ�������topicΪ [" + publisher.Topic + "] ���¼�");
            events.Add(publisher.Topic, publisher);
        }

        private void AddEventSubscriber(string subscriberKey, EventSubscriberInfo subscriber)
        {
            subscribers.Add(subscriberKey, subscriber);
        }

        private void RemoveEventSubscriber(string subscriberKey)
        {
            subscribers.Remove(subscriberKey);
        }

        private string SessionId
        {
            get {
                ISessionService service = kernel[typeof(ISessionService)] as ISessionService;
                return service.CurrentSession[SessionVariables.SESSION_ID].ToString();
            }
        }

        private void AddOuterSubscriberToPublisherInfo(string subscriberKey, EventSubscriberInfo subInfo)
        {
            foreach (EventPublisherInfo publisher in this.events.Values) {
                if (publisher.Topic == subInfo.Topic) {
                    publisher.AddSubscriber(subscriberKey, subInfo);
                }
            }
        }

        private void RemoveOuterSubscriberFromPublisherInfo(string subscriberKey)
        {
            foreach (EventPublisherInfo publisher in this.events.Values) {
                if (publisher.ContainsSubscriber(subscriberKey))
                    publisher.RemoveSubscriber(subscriberKey);
            }
        }

        private void ReregisterOuterEvent()
        {
            List<OuterEventInfo> eventList = new List<OuterEventInfo>(db.Load<OuterEventInfo>());
            eventList.ForEach(delegate(OuterEventInfo info) {
                try {
                    RegisterOuterEventSubscriber(info.SessionId, info.Topic, info.Location);
                }
                catch {
                    db.Delete(info);
                }
            });
        }

        public void RegisterOuterEventSubscriber(string sessionId, string topic, SubscriberLocation location)
        {
            if (subscribers.ContainsKey(sessionId + topic)) return;

            if (!events.ContainsKey(topic)) throw new ArgumentException("ϵͳ��û��topicΪ [" + topic + "] ���¼�");
            EventCollector collector;
            if (eventCollectors.ContainsKey(sessionId))
                collector = eventCollectors[sessionId];
            else {
                collector = new EventCollector();
                eventCollectors.Add(sessionId, collector);
            }
            collector.RegisterEvent(topic, events[topic].EventInfo, kernel[events[topic].EventInfo.DeclaringType]);
            EventSubscriberInfo subInfo = new EventSubscriberInfo(topic, location, "", "");
            AddEventSubscriber(sessionId + topic, subInfo);
            if (!outerSubscriberMapping.ContainsKey(sessionId)) outerSubscriberMapping.Add(sessionId, new List<string>());
            outerSubscriberMapping[sessionId].Add(topic);
            AddOuterSubscriberToPublisherInfo(sessionId + topic, subInfo);
            db.Store(new OuterEventInfo(sessionId, topic, location));
        }

        public void UnRegisterAnOuterEventSubscriber(string sessionId, string topic)
        {
            if (eventCollectors.ContainsKey(sessionId)) {
                eventCollectors[sessionId].UnRegisterEvent(topic);
            }
            RemoveEventSubscriber(sessionId + topic);
            if (outerSubscriberMapping.ContainsKey(sessionId)) outerSubscriberMapping[sessionId].Remove(topic);
            RemoveOuterSubscriberFromPublisherInfo(sessionId + topic);
            OuterEventInfo template = new OuterEventInfo();
            template.SessionId = sessionId;
            template.Topic = topic;

            IList<OuterEventInfo> events = db.Load<OuterEventInfo>(template);
            foreach (OuterEventInfo info in events) {
                db.Delete(info);
            }
        }

        public EventResultData[] GetOuterEventResults(string sessionId)
        {
            if (!eventCollectors.ContainsKey(sessionId))
                throw new ArgumentException("�����¼���Session [" + sessionId + "] û�б�ע��");
            EventResultData[] results = eventCollectors[sessionId].GetCollectedEventArgs();
            //if (results.Length == 0) {
            //    logger.Info("�Ự [" + sessionId + "] �Ŀͻ����¼�����ʱ");
            //}
            return results;
        }

        #endregion

        #region Assistant class

        [Serializable]
        class OuterEventInfo
        {
            public string SessionId;
            public string Topic;
            public SubscriberLocation Location;

            public OuterEventInfo()
            {
            }

            public OuterEventInfo(string sessionId, string topic, SubscriberLocation location)
            {
                this.SessionId = sessionId;
                this.Topic = topic;
                this.Location = location;
            }
        }

        #endregion

        #region IEventDispatcher Members

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="description">�¼�����</param>
        /// <param name="eventScope">�¼���Χ</param>
        /// <param name="eventInfo">EventInfoʵ��</param>
        public void RegisterEventPublisher(string topic, string description, EventPublisherScope eventScope, EventInfo eventInfo)
        {
            AddEventPublisher(new EventPublisherInfo(topic, description, eventScope, eventInfo));
        }

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="methodInfo">MethodInfoʵ��</param>
        /// <param name="serviceType">��������</param>
        public void RegisterEventSubscriber(string topic, MethodInfo methodInfo, Type serviceType)
        {
            AddEventSubscriber(SecurityUtility.HashObject(methodInfo), new EventSubscriberInfo(topic, methodInfo, serviceType, SubscriberLocation.Local));
        }

        /// <summary>
        /// ע���ⲿ���¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        /// <param name="location">������λ��</param>
        public void RegisterOuterEventSubscriber(string topic, SubscriberLocation location)
        {
            RegisterOuterEventSubscriber(SessionId, topic, location);
        }

        /// <summary>
        /// ע���ⲿ�¼�������
        /// </summary>
        /// <param name="topic">�¼�Ψһ����</param>
        public void UnRegisterAnOuterEventSubscriber(string topic)
        {
            UnRegisterAnOuterEventSubscriber(SessionId, topic);
        }

        /// <summary>
        /// ע�������ⲿ�¼�������
        /// </summary>
        public void UnRegisterAllOuterEventSubscriber()
        {
            UnRegisterAllOuterEventSubscriber(SessionId);
        }

        /// <summary>
        /// ע��ָ���Ự�������ⲿ�¼�������
        /// </summary>
        /// <param name="sessionId">�ỰId</param>
        public void UnRegisterAllOuterEventSubscriber(string sessionId)
        {
            if (eventCollectors.ContainsKey(sessionId))
            {
                eventCollectors[sessionId].Dispose();
                if (outerSubscriberMapping.ContainsKey(sessionId)) {
                    foreach (string topic in outerSubscriberMapping[sessionId]) {
                        RemoveEventSubscriber(sessionId + topic);
                        RemoveOuterSubscriberFromPublisherInfo(sessionId + topic);
                    }
                }
                eventCollectors.Remove(sessionId);
            }
            if (outerSubscriberMapping.ContainsKey(sessionId)) outerSubscriberMapping.Remove(sessionId);
            OuterEventInfo template = new OuterEventInfo();
            template.SessionId = sessionId;

            IList<OuterEventInfo> events = db.Load<OuterEventInfo>(template);
            foreach (OuterEventInfo info in events)
            {
                db.Delete(info);
            }
        }

        /// <summary>
        /// ��ȡ�����¼������ߵ�����
        /// </summary>
        /// <returns>�¼�����������</returns>
        public EventPublisherInfo[] GetAllEventPublishers()
        {
            List<EventPublisherInfo> list = new List<EventPublisherInfo>();
            foreach (EventPublisherInfo eventpub in events.Values)
            {
                list.Add(eventpub);
            }
            return list.ToArray();
        }

        /// <summary>
        /// ��ȡ�ⲿ�¼����
        /// </summary>
        /// <returns>�¼������Ϣ����</returns>
        public EventResultData[] GetOuterEventResults()
        {
            return GetOuterEventResults(SessionId);
        }

        /// <summary>
        /// ���ӵ��ڲ��¼�
        /// </summary>
        public void ConnectInnerEvent()
        {
            foreach (EventPublisherInfo publisher in this.events.Values) {
                foreach (string subscriberKey in subscribers.Keys) {
                    EventSubscriberInfo info = subscribers[subscriberKey];
                    if (info.Location == SubscriberLocation.Local && publisher.Topic == info.Topic) {
                        try {
                            Delegate handler = Delegate.CreateDelegate(publisher.EventInfo.EventHandlerType, kernel[info.ServiceType], info.MethodInfo);
                            publisher.EventInfo.AddEventHandler(kernel[publisher.EventInfo.DeclaringType], handler);
                            publisher.AddSubscriber(subscriberKey, info);
                        }
                        catch (Exception ex) {
                            throw new Exception("�����Ӷ���[" + info.MethodInfo.DeclaringType.Name +
                                "]�ϵĶ�����[" + info.Topic + "]���ӿ�[" +
                                publisher.EventInfo.DeclaringType.ToString() +
                                "]�϶�����¼�[" + publisher.EventInfo.Name + "]ʱ��������", ex);
                        }
                    }
                }
            }
            ReregisterOuterEvent();
        }

        #endregion
    }
}
