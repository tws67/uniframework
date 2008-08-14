using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// ������¼�̽�����������ĳһ�¼���̽�⡣�����ĳ�Աsubjectʵ�����¼������������������ض��¼������ע�ᵽSubject.Subscripters
    /// </summary>
    public abstract class EventDispatcher
    {
        private Thread workThread = null;
        private int interval = 500;
        private object subject = null;

        protected bool abort = true;

        public EventDispatcher(Type serviceType)
        {
            ArgumentUtility.AssertNotNull<Type>(serviceType, "serviceType");

            subject = EventSubject.GetObject(serviceType);
        }

        public EventDispatcher(Type serviceType, Type baseType)
        {
            ArgumentUtility.AssertNotNull<Type>(serviceType, "service");
            ArgumentUtility.AssertNotNull<Type>(baseType, "baseType");

            subject = EventSubject.GetObject(baseType, serviceType);
        }

        #region AbstractEventListener Members

        /// <summary>
        /// �����¼�֮��ļ��Ĭ��Ϊ500���룬�Է�ֹ�����߳�ռ�ù����ϵͳ��Դ
        /// </summary>
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        /// <summary>
        /// �¼���������ʵ����Observerģʽ��
        /// </summary>
        public EventSubject Subject
        {
            get { return subject as EventSubject; }
        }

        #endregion

        /// <summary>
        /// ��ʼ�¼���������
        /// </summary>
        public void Start()
        {
            abort = false;
            workThread = new Thread(new ThreadStart(Listen)); // ��ʼ����
            workThread.Priority = ThreadPriority.AboveNormal;
            workThread.IsBackground = true;
            workThread.Start();
            Subject.Enable();
        }

        /// <summary>
        /// ֹͣ�¼���������
        /// </summary>
        public void Stop()
        {
            abort = true;
            Thread.Sleep(5000);
            Subject.Disable(); // ֹͣ���¼������߷ַ��¼�
        }

        /// <summary>
        /// �¼����������ʵ�ִ����������н��д���������ĳ�¼�ʱҪ���ϵļ��abort��־�Ƿ�Ϊtrue��
        /// ���Ϊtrue�Ļ���Ҫ����ֹͣ�¼����������ͷŹ����߳�
        /// </summary>
        protected abstract void Listen();
    }
}
