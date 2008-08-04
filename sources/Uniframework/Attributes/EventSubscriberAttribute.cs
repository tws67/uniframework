using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 事件订阅属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class EventSubscriberAttribute : Attribute
    {
        string topic;

        /// <summary>
        /// 无参构造函数，增加此构造函数主要是为了能够让XmlSerializer进行序列化
        /// </summary>
        public EventSubscriberAttribute()
        {
            topic = string.Empty;
        }

        /// <summary>
        /// 事件订阅者构造器，如果需要订阅某事件必须与事件发布者所定义的Topic一致。
        /// </summary>
        /// <param name="topic"></param>
        public EventSubscriberAttribute(string topic)
        {
            this.topic = topic;
        }

        /// <summary>
        /// 事件订阅主题
        /// </summary>
        public string Topic
        {
            get 
            {
                return topic;
            }
        }
    }
}
