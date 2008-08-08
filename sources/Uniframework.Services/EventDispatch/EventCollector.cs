using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �¼��ռ�����ÿ���ռ�����Ӧһ���ͻ��˵ĻỰ�������ռ��ûỰ�������¼�������Ϣ
    /// </summary>
    internal class EventCollector : IDisposable
    {
        private Dictionary<string, EventListener> listeners;
        private bool listenning = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCollector"/> class.
        /// </summary>
        public EventCollector()
        {
            listeners = new Dictionary<string, EventListener>();
        }

        /// <summary>
        /// ע��һ���¼�
        /// </summary>
        /// <param name="eventTopic">�¼���ʶ��Ϣ��ϵͳȫ��Ψһ</param>
        /// <param name="eventInfo">�¼���Ϣ</param>
        /// <param name="publisher">������</param>
        public void RegisterEvent(string eventTopic, EventInfo eventInfo, object publisher)
        {
            if (listeners.ContainsKey(eventTopic)) throw new ArgumentException("�¼��ռ������Ѿ�������topicΪ[" + eventTopic + "]���¼�������");
            EventListener listener = new EventListener(eventInfo, publisher);
            listeners.Add(eventTopic, listener);
        }

        /// <summary>
        /// ע��һ���¼�
        /// </summary>
        /// <param name="eventTopic">�¼���ʶ��Ϣ</param>
        public void UnRegisterEvent(string eventTopic)
        {
            if (listeners.ContainsKey(eventTopic))
            {
                listeners[eventTopic].Dispose();
                listeners.Remove(eventTopic);
            }
        }
        
        /// <summary>
        /// ��ȡ��ǰ�¼��ռ��������е��¼�������
        /// </summary>
        /// <returns>�¼�����������</returns>
        public EventListener[] GetAllListener()
        {
            List<EventListener> list = new List<EventListener>();
            foreach (EventListener listener in listeners.Values)
            {
                list.Add(listener);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets the collected event args.
        /// </summary>
        /// <returns></returns>
        public EventResultData[] GetCollectedEventArgs()
        {
            int counter = 0;
            while (counter < 600 && listenning)
            {
                EventResultData[] data = DetectEvent();
                if (data.Length > 0) return data;
                System.Threading.Thread.Sleep(100); // 
                counter++;
            }
            return new EventResultData[0];
        }

        /// <summary>
        /// ��������¼�
        /// </summary>
        /// <returns>�¼��������</returns>
        private EventResultData[] DetectEvent()
        {
            List<EventResultData> list = new List<EventResultData>();
            foreach (string topic in listeners.Keys)
            {
                EventArgs[] args = listeners[topic].DequeueAllEventArgs();
                if (args.Length != 0)
                {
                    foreach (EventArgs arg in args)
                    {
                        EventResultData result = new EventResultData(topic, arg);
                        list.Add(result);
                    }
                }
            }
            return list.ToArray();
        }

        #region IDisposable Members

        public void Dispose()
        {
            listenning = false;
            foreach (EventListener listener in listeners.Values)
            {
                listener.Dispose();
            }
        }

        #endregion
    }
}
