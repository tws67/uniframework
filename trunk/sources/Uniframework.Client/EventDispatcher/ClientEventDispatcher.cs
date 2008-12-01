using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using log4net;
using Uniframework.Services;

namespace Uniframework.Client
{
    /// <summary>
    /// 客户端事件分发器
    /// </summary>
    public class ClientEventDispatcher
    {
        private Dictionary<string, Dictionary<MethodInfo, List<object>>> subscribers;
        private static ClientEventDispatcher instance;
        private static object syncObj = new object();
        private ILog logger;
        private IEventDispatcher dispatcher;

        /// <summary>
        /// 日志记录组件
        /// </summary>
        public ILog Logger
        {
            get
            {
                return logger;
            }
            set
            {
                logger = value;
            }
        }

        /// <summary>
        /// 事件分配器单件实例
        /// </summary>
        public static ClientEventDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new ClientEventDispatcher();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <param name="e">参数</param>
        public void DispachEvent(string topic, EventArgs e)
        {
            Dictionary<MethodInfo, List<object>> eventList = subscribers[topic];
            foreach (MethodInfo method in eventList.Keys)
            {
                foreach (object receiver in eventList[method])
                {
                    try
                    {
                        InvokeMehtod(receiver, method, e);
                        logger.Debug("已经将事件参数 [" + e.GetType().Name + "] 提交 [" + receiver.GetType().Name + "] 类的 [" + method.Name + "] 方法");
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("分发事件时发生错误", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 检查某主题的事件是否已经注册
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <returns>如果包含主题的事件已经注册返回true，否则为false</returns>
        public bool HasRegisteredEventTopic(string topic)
        {
            return subscribers.ContainsKey(topic);
        }

        /// <summary>
        /// 注册事件订阅者
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <param name="target">订阅者</param>
        /// <param name="method">事件方法</param>
        public void RegisterSubscriber(string topic, object target, MethodInfo method)
        {
            if (!subscribers.ContainsKey(topic)) 
                subscribers.Add(topic, new Dictionary<MethodInfo, List<object>>());
            if (!subscribers[topic].ContainsKey(method)) 
                subscribers[topic].Add(method, new List<object>());
            subscribers[topic][method].Add(target);
        }

        /// <summary>
        /// 重新订阅所有事件
        /// </summary>
        public void RereregisterAllEvent()
        {
            foreach (string topic in subscribers.Keys)
            {
                dispatcher.RegisterOuterEventSubscriber(topic, SubscriberLocation.Client);
            }
        }

        /// <summary>
        /// 指示事件分发器下是否存在订阅者
        /// </summary>
        public bool HasSubscribers
        {
            get
            {
                return subscribers.Count > 0;
            }
        }

        /// <summary>
        /// 注销事件订阅者
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <param name="method">事件方法</param>
        /// <param name="receiver">接收者</param>
        public void UnRegisterSubscriber(string topic, MethodInfo method, object receiver)
        {
            if (subscribers.ContainsKey(topic))
            {
                Dictionary<MethodInfo, List<object>> list = subscribers[topic];
                list[method].Remove(receiver);
                if (list[method].Count == 0) list.Remove(method);
                if (list.Count == 0) subscribers.Remove(topic);
            }
        }

        /// <summary>
        /// 注册事件订阅者
        /// </summary>
        /// <param name="topic">事件主题</param>
        /// <param name="reciever">接收者</param>
        /// <param name="methodToRemind">方法</param>
        public void RegisterEventSubscriber(string topic, object reciever, MethodInfo methodToRemind)
        {
            if (!HasRegisteredEventTopic(topic))
            {
                dispatcher.RegisterOuterEventSubscriber(topic, SubscriberLocation.Client);
            }
            RegisterSubscriber(topic, reciever, methodToRemind);
        }

        /// <summary>
        /// 注册事件订阅者
        /// </summary>
        /// <param name="sender">事件触发者</param>
        /// <param name="topic">主题</param>
        /// <param name="reciever">接收者</param>
        /// <param name="methodToRemind">事件方法</param>
        public void RegisterEventSubscriber(object sender, string topic, object reciever, MethodInfo methodToRemind)
        {
            RegisterEventSubscriber(topic, reciever, methodToRemind);
        }

        /// <summary>
        /// 注销事件订阅者
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="methodInfo">方法</param>
        /// <param name="reciever">接收者</param>
        public void UnRegisterEventSubscriber(string topic, MethodInfo methodInfo, object reciever)
        {
            UnRegisterSubscriber(topic, methodInfo, reciever);

            if (!HasRegisteredEventTopic(topic))
            {
                dispatcher.UnRegisterAnOuterEventSubscriber(topic);
            }
        }

        #region Assistant functions

        private ClientEventDispatcher()
        {
            subscribers = new Dictionary<string, Dictionary<MethodInfo, List<object>>>();
            dispatcher = ServiceRepository.Instance.GetService<IEventDispatcher>();
        }

        private void InvokeMehtod(object target, MethodInfo method, EventArgs e)
        {
            DynamicInvokerHandler invoker = DynamicInvoker.GetMethodInvoker(method);
            invoker(target, new object[] { null, e });
        }

        #endregion
    }
}
