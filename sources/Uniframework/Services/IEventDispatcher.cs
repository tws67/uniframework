using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Uniframework.Services
{
    #region SubscriberLocation
    /// <summary>
    /// 订阅者位置枚举
    /// </summary>
    public enum SubscriberLocation
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,

        /// <summary>
        /// 客户端
        /// </summary>
        Client,

        /// <summary>
        /// 网页
        /// </summary>
        Web
    }

    #endregion

    #region IEventDispatcher

    /// <summary>
    /// 系统事件分发器接口
    /// </summary>
    [RemoteService("系统事件分发器", ServiceType.System)]
    public interface IEventDispatcher
    {
        #region IEventDispatcher Member
        /// <summary>
        /// 注册事件发布者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="description">事件描述</param>
        /// <param name="eventScope">事件范围</param>
        /// <param name="eventInfo">EventInfo实例</param>
        void RegisterEventPublisher(string topic, string description, EventPublisherScope eventScope, EventInfo eventInfo);

        /// <summary>
        /// 注册事件订阅者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="methodInfo">MethodInfo实例</param>
        /// <param name="serviceType">服务类型</param>
        void RegisterEventSubscriber(string topic, MethodInfo methodInfo, Type serviceType);

        /// <summary>
        /// 注册外部的事件订阅者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="location">订阅者位置</param>
        [RemoteMethod("注册外部事件订阅者")]
        void RegisterOuterEventSubscriber(string topic, SubscriberLocation location);

        /// <summary>
        /// 注销外部事件订阅者
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        [RemoteMethod("注销外部事件订阅者")]
        void UnRegisterAnOuterEventSubscriber(string topic);

        /// <summary>
        /// 注销所有外部事件订阅者
        /// </summary>
        void UnRegisterAllOuterEventSubscriber();

        /// <summary>
        /// 注销指定会话的所有外部事件订阅者
        /// </summary>
        /// <param name="sessionId">会话Id</param>
        void UnRegisterAllOuterEventSubscriber(string sessionId);

        /// <summary>
        /// 获取所有事件发布者的描述
        /// </summary>
        /// <returns>事件发布者数组</returns>
        EventPublisherInfo[] GetAllEventPublishers();

        /// <summary>
        /// 获取外部事件结果
        /// </summary>
        /// <returns>事件结果信息数组</returns>
        [RemoteMethod("获取外部事件结果")]
        EventResultData[] GetOuterEventResults();

        /// <summary>
        /// 连接到内部事件
        /// </summary>
        void ConnectInnerEvent();

        #endregion
    }

    #endregion

    #region EventResultData

    /// <summary>
    /// 事件结果
    /// </summary>
    [Serializable]
    public class EventResultData
    {
        string topic;
        EventArgs args;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public EventResultData()
            : this(string.Empty, null)
        { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="args">事件参数</param>
        public EventResultData(string topic, EventArgs args)
        {
            this.topic = topic;
            this.args = args;
        }

        /// <summary>
        /// 事件唯一名称
        /// </summary>
        public string Topic { get { return topic; } }

        /// <summary>
        /// 事件参数
        /// </summary>
        public EventArgs Args { get { return args; } }
    }

    #endregion

    #region EventPublisherInfo

    /// <summary>
    /// 事件发布者信息
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
        /// 无参构造函数
        /// </summary>
        public EventPublisherInfo()
        { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="description">事件描述</param>
        /// <param name="scope">事件范围</param>
        /// <param name="eventInfo">EventInfo实例</param>
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
        /// 获取事件唯一名称
        /// </summary>
        public string Topic { get { return this.topic; } }

        /// <summary>
        /// 获取描述
        /// </summary>
        public string Description { get { return this.description; } }

        /// <summary>
        /// 事件范围
        /// </summary>
        public EventPublisherScope EventScope { get { return this.eventScope; } }

        /// <summary>
        /// EventInfo
        /// </summary>
        public EventInfo EventInfo { get { return this.eventInfo; } }

        #endregion

        /// <summary>
        /// 添加订阅者到订阅者数组
        /// </summary>
        /// <param name="subscriberKey">订阅者标识</param>
        /// <param name="subscriber">订阅者信息</param>
        public void AddSubscriber(string subscriberKey, EventSubscriberInfo subscriber)
        {
            subscribers.Add(subscriberKey, subscriber);
        }

        /// <summary>
        /// 从订阅者数组中移除指定标识的订阅者
        /// </summary>
        /// <param name="subscriberKey">订阅者标识</param>
        public void RemoveSubscriber(string subscriberKey)
        {
            if (subscribers.ContainsKey(subscriberKey))
                subscribers.Remove(subscriberKey);
        }

        /// <summary>
        /// 检查指定标识的订阅者是否在订阅者数组中
        /// </summary>
        /// <param name="subscriberKey">订阅者标识</param>
        /// <returns></returns>
        public bool ContainsSubscriber(string subscriberKey)
        {
            return subscribers.ContainsKey(subscriberKey);
        }

        /// <summary>
        /// 订阅者数组
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
    /// 事件订阅者信息
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
        /// 无参构造函数
        /// </summary>
        public EventSubscriberInfo()
        { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="methodInfo">MethodInfo实例</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="location">订阅者位置</param>
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
        /// 构造方法
        /// </summary>
        /// <param name="topic">事件唯一名称</param>
        /// <param name="location">订阅者位置</param>
        /// <param name="componentName">组件名称</param>
        /// <param name="methodName">MethodInfo实例</param>
        public EventSubscriberInfo(string topic, SubscriberLocation location, string componentName, string methodName)
        {
            this.topic = topic;
            this.location = location;
            this.componentName = componentName;
            this.methodName = methodName;
        }
        
        #region EventSubscriberInfo Members

        /// <summary>
        /// 获取事件唯一名称
        /// </summary>
        public string Topic { get { return topic; } }

        /// <summary>
        /// 获取服务类型
        /// </summary>
        public Type ServiceType { get { return serviceType; } }

        /// <summary>
        /// 获取MethodInfo实例
        /// </summary>
        public MethodInfo MethodInfo { get { return methodInfo; } }

        /// <summary>
        /// 获取订阅者位置
        /// </summary>
        public SubscriberLocation Location { get { return location; } }

        /// <summary>
        /// 获取组件名称
        /// </summary>
        public string ComponentName { get { return componentName; } }

        /// <summary>
        /// 获取方法名称
        /// </summary>
        public string MethodName { get { return methodName; } }

        #endregion 
    }

    #endregion
}
