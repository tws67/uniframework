using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 事件收集器，每个收集器对应一个客户端的会话，用于收集该会话的所有事件参数信息
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
        /// 注册一个事件
        /// </summary>
        /// <param name="eventTopic">事件标识信息，系统全局唯一</param>
        /// <param name="eventInfo">事件信息</param>
        /// <param name="publisher">发布者</param>
        public void RegisterEvent(string eventTopic, EventInfo eventInfo, object publisher)
        {
            if (listeners.ContainsKey(eventTopic)) throw new ArgumentException("事件收集器中已经建立了topic为[" + eventTopic + "]的事件接收器");
            EventListener listener = new EventListener(eventInfo, publisher);
            listeners.Add(eventTopic, listener);
        }

        /// <summary>
        /// 注销一个事件
        /// </summary>
        /// <param name="eventTopic">事件标识信息</param>
        public void UnRegisterEvent(string eventTopic)
        {
            if (listeners.ContainsKey(eventTopic))
            {
                listeners[eventTopic].Dispose();
                listeners.Remove(eventTopic);
            }
        }
        
        /// <summary>
        /// 获取当前事件收集器中所有的事件监听器
        /// </summary>
        /// <returns>事件监听器数组</returns>
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
        /// 检测所有事件
        /// </summary>
        /// <returns>事件结果数组</returns>
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
