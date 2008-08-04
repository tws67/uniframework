using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 事件发布者属性，利用它来标注某个事件需要进行发布
    /// <example>
    /// <code>
    /// <![CDATA[
    /// [EventPulisher("SampleEvent")]
    /// event EventHandler<CommonEventArgs<string>> EventPublisher;
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Event, AllowMultiple=true)]
    public class EventPublisherAttribute : Attribute
    {
        string topic = string.Empty;
        string description = string.Empty;
        EventPublisherScope eventScope = EventPublisherScope.Global;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherAttribute"/> class.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="description">The description.</param>
        /// <param name="eventScope">The event scope.</param>
        public EventPublisherAttribute(string topic, string description, EventPublisherScope eventScope)
        {
            this.topic = topic;
            this.description = description;
            this.eventScope = eventScope;
        }

        /// <summary>
        /// 无参构造函数，增加此构造函数主要是为了能够让XmlSerializer进行序列化
        /// </summary>
        public EventPublisherAttribute()
            : this(string.Empty, string.Empty, EventPublisherScope.Global)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherAttribute"/> class.
        /// </summary>
        /// <param name="topic">The topic.</param>
        public EventPublisherAttribute(string topic)
            : this(topic, string.Empty, EventPublisherScope.Global)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherAttribute"/> class.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="description">The description.</param>
        public EventPublisherAttribute(string topic, string description)
            : this(topic, description, EventPublisherScope.Global)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherAttribute"/> class.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="eventScope">The event scope.</param>
        public EventPublisherAttribute(string topic, EventPublisherScope eventScope)
            : this(topic, string.Empty, eventScope)
        { }

        /// <summary>
        /// 事件名称，通过Topic能够唯一标识某一事件
        /// </summary>
        public string Topic 
        {
            get 
            {
                return topic;
            }
        }

        /// <summary>
        /// 事件发布者对事件的描述
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// 事件发布的范围
        /// </summary>
        public EventPublisherScope EventScope
        {
            get
            {
                return eventScope;
            }
        }
    }

    #region 事件发布范围枚举
    /// <summary>
    /// 事件发布范围的枚举值
    /// </summary>
    public enum EventPublisherScope
    { 
        /// <summary>
        /// 全局性的事件
        /// </summary>
        Global,
        /// <summary>
        /// 服务器端事件
        /// </summary>
        Server,
        /// <summary>
        /// 客户端事件
        /// </summary>
        Client
    }
    #endregion
}
