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
    /// 服务器端事件分发器，负责注册/注销所有的事件及向客户端分发事件
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
                throw new ArgumentException("接口 [" + events[publisher.Topic].EventInfo.DeclaringType.ToString() + "] 和接口 [" + publisher.EventInfo.DeclaringType.ToString() + "] 重复定义了topic为 [" + publisher.Topic + "] 的事件");
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

            if (!events.ContainsKey(topic)) throw new ArgumentException("系统中没有topic为 [" + topic + "] 的事件");
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
                throw new ArgumentException("请求事件的Session [" + sessionId + "] 没有被注册");
            EventResultData[] results = eventCollectors[sessionId].GetCollectedEventArgs();
            //if (results.Length == 0) {
            //    logger.Info("会话 [" + sessionId + "] 的客户端事件请求超时");
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
        /// 注册事件发布者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="description">事件描述</param>
        /// <param name="eventScope">事件范围</param>
        /// <param name="eventInfo">EventInfo实例</param>
        public void RegisterEventPublisher(string topic, string description, EventPublisherScope eventScope, EventInfo eventInfo)
        {
            AddEventPublisher(new EventPublisherInfo(topic, description, eventScope, eventInfo));
        }

        /// <summary>
        /// 注册事件订阅者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="methodInfo">MethodInfo实例</param>
        /// <param name="serviceType">服务类型</param>
        public void RegisterEventSubscriber(string topic, MethodInfo methodInfo, Type serviceType)
        {
            AddEventSubscriber(SecurityUtility.HashObject(methodInfo), new EventSubscriberInfo(topic, methodInfo, serviceType, SubscriberLocation.Local));
        }

        /// <summary>
        /// 注册外部的事件订阅者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="location">订阅者位置</param>
        public void RegisterOuterEventSubscriber(string topic, SubscriberLocation location)
        {
            RegisterOuterEventSubscriber(SessionId, topic, location);
        }

        /// <summary>
        /// 注销外部事件订阅者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        public void UnRegisterAnOuterEventSubscriber(string topic)
        {
            UnRegisterAnOuterEventSubscriber(SessionId, topic);
        }

        /// <summary>
        /// 注销所有外部事件订阅者
        /// </summary>
        public void UnRegisterAllOuterEventSubscriber()
        {
            UnRegisterAllOuterEventSubscriber(SessionId);
        }

        /// <summary>
        /// 注销指定会话的所有外部事件订阅者
        /// </summary>
        /// <param name="sessionId">会话Id</param>
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
        /// 获取所有事件发布者的描述
        /// </summary>
        /// <returns>事件发布者数组</returns>
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
        /// 获取外部事件结果
        /// </summary>
        /// <returns>事件结果信息数组</returns>
        public EventResultData[] GetOuterEventResults()
        {
            return GetOuterEventResults(SessionId);
        }

        /// <summary>
        /// 连接到内部事件
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
                            throw new Exception("在连接对象[" + info.MethodInfo.DeclaringType.Name +
                                "]上的订阅者[" + info.Topic + "]到接口[" +
                                publisher.EventInfo.DeclaringType.ToString() +
                                "]上定义的事件[" + publisher.EventInfo.Name + "]时发生错误", ex);
                        }
                    }
                }
            }
            ReregisterOuterEvent();
        }

        #endregion
    }
}
