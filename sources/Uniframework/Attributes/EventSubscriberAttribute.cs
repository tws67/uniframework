using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �¼���������
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class EventSubscriberAttribute : Attribute
    {
        string topic;

        /// <summary>
        /// �޲ι��캯�������Ӵ˹��캯����Ҫ��Ϊ���ܹ���XmlSerializer�������л�
        /// </summary>
        public EventSubscriberAttribute()
        {
            topic = string.Empty;
        }

        /// <summary>
        /// �¼������߹������������Ҫ����ĳ�¼��������¼��������������Topicһ�¡�
        /// </summary>
        /// <param name="topic"></param>
        public EventSubscriberAttribute(string topic)
        {
            this.topic = topic;
        }

        /// <summary>
        /// �¼���������
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
