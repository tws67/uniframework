using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �¼�����������
    /// </summary>
    public static class EventHelper
    {
        /// <summary>
        /// �����¼�
        /// </summary>
        /// <typeparam name="T">�¼�����</typeparam>
        /// <param name="handler">�¼�ʵ��</param>
        /// <param name="sender">�¼�������</param>
        /// <param name="e">�¼�����</param>
        public static void Raise<T>(EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public delegate T CreateEventArgs<T>() where T : EventArgs;

        /// <summary>
        /// ����ָ�����¼�
        /// </summary>
        /// <typeparam name="T">�¼����ͣ����ͣ�</typeparam>
        /// <param name="handler">�¼����</param>
        /// <param name="sender">�¼�������</param>
        /// <param name="createEventArgs">�¼�����</param>
        public static void Raise<T>(EventHandler<T> handler, object sender, CreateEventArgs<T> createEventArgs) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, createEventArgs());
            }
        }

        /// <summary>
        /// Generic method used to broadcast events when needed. This method will attempt to send the event to each registered
        /// listener, even if any particular event throws an exception
        /// </summary>
        /// <param name="eventToBroadcast">Event to be broadcast</param>
        /// <param name="eventSender">Object that sent the event</param>
        /// <param name="eventData">EventArgs to be broadcast with the event</param>
        public static void BroadcastEvent(Delegate eventToBroadcast, object eventSender, EventArgs eventData)
        {
            if (eventToBroadcast != null)
            {
                object[] args = new object[] { eventSender, eventData };
                foreach (Delegate callbackDelegate in eventToBroadcast.GetInvocationList())
                {
                    try
                    {
                        callbackDelegate.DynamicInvoke(args);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
