using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �¼����������ԣ�����������עĳ���¼���Ҫ���з���
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
        /// �޲ι��캯�������Ӵ˹��캯����Ҫ��Ϊ���ܹ���XmlSerializer�������л�
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
        /// �¼����ƣ�ͨ��Topic�ܹ�Ψһ��ʶĳһ�¼�
        /// </summary>
        public string Topic 
        {
            get 
            {
                return topic;
            }
        }

        /// <summary>
        /// �¼������߶��¼�������
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// �¼������ķ�Χ
        /// </summary>
        public EventPublisherScope EventScope
        {
            get
            {
                return eventScope;
            }
        }
    }

    #region �¼�������Χö��
    /// <summary>
    /// �¼�������Χ��ö��ֵ
    /// </summary>
    public enum EventPublisherScope
    { 
        /// <summary>
        /// ȫ���Ե��¼�
        /// </summary>
        Global,
        /// <summary>
        /// ���������¼�
        /// </summary>
        Server,
        /// <summary>
        /// �ͻ����¼�
        /// </summary>
        Client
    }
    #endregion
}
