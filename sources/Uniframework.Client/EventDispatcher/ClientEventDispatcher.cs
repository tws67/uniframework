using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using log4net;
using Uniframework.Services;

namespace Uniframework.Client
{
    /// <summary>
    /// �ͻ����¼��ַ���
    /// </summary>
    public class ClientEventDispatcher
    {
        private Dictionary<string, Dictionary<MethodInfo, List<object>>> subscribers;
        private static ClientEventDispatcher instance;
        private static object syncObj = new object();
        private ILog logger;
        private IEventDispatcher dispatcher;

        /// <summary>
        /// ��־��¼���
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
        /// �¼�����������ʵ��
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
        /// �ַ��¼�
        /// </summary>
        /// <param name="topic">�¼�����</param>
        /// <param name="e">����</param>
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
                        logger.Debug("�Ѿ����¼����� [" + e.GetType().Name + "] �ύ [" + receiver.GetType().Name + "] ��� [" + method.Name + "] ����");
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("�ַ��¼�ʱ��������", ex);
                    }
                }
            }
        }

        /// <summary>
        /// ���ĳ������¼��Ƿ��Ѿ�ע��
        /// </summary>
        /// <param name="topic">�¼�����</param>
        /// <returns>�������������¼��Ѿ�ע�᷵��true������Ϊfalse</returns>
        public bool HasRegisteredEventTopic(string topic)
        {
            return subscribers.ContainsKey(topic);
        }

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�����</param>
        /// <param name="target">������</param>
        /// <param name="method">�¼�����</param>
        public void RegisterSubscriber(string topic, object target, MethodInfo method)
        {
            if (!subscribers.ContainsKey(topic)) 
                subscribers.Add(topic, new Dictionary<MethodInfo, List<object>>());
            if (!subscribers[topic].ContainsKey(method)) 
                subscribers[topic].Add(method, new List<object>());
            subscribers[topic][method].Add(target);
        }

        /// <summary>
        /// ���¶��������¼�
        /// </summary>
        public void RereregisterAllEvent()
        {
            foreach (string topic in subscribers.Keys)
            {
                dispatcher.RegisterOuterEventSubscriber(topic, SubscriberLocation.Client);
            }
        }

        /// <summary>
        /// ָʾ�¼��ַ������Ƿ���ڶ�����
        /// </summary>
        public bool HasSubscribers
        {
            get
            {
                return subscribers.Count > 0;
            }
        }

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�����</param>
        /// <param name="method">�¼�����</param>
        /// <param name="receiver">������</param>
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
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">�¼�����</param>
        /// <param name="reciever">������</param>
        /// <param name="methodToRemind">����</param>
        public void RegisterEventSubscriber(string topic, object reciever, MethodInfo methodToRemind)
        {
            if (!HasRegisteredEventTopic(topic))
            {
                dispatcher.RegisterOuterEventSubscriber(topic, SubscriberLocation.Client);
            }
            RegisterSubscriber(topic, reciever, methodToRemind);
        }

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="topic">����</param>
        /// <param name="reciever">������</param>
        /// <param name="methodToRemind">�¼�����</param>
        public void RegisterEventSubscriber(object sender, string topic, object reciever, MethodInfo methodToRemind)
        {
            RegisterEventSubscriber(topic, reciever, methodToRemind);
        }

        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="topic">����</param>
        /// <param name="methodInfo">����</param>
        /// <param name="reciever">������</param>
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
