using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �¼�����������
    /// </summary>
    public static class EventUtility
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
    }
}
