using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 抽象的事件探测器，负责对某一事件的探测。由它的成员subject实现了事件分配的能力，允许对特定事件的组件注册到Subject.Subscripters
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
        /// 侦听事件之间的间隔默认为500毫秒，以防止工作线程占用过多的系统资源
        /// </summary>
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        /// <summary>
        /// 事件分配器（实现了Observer模式）
        /// </summary>
        public EventSubject Subject
        {
            get { return subject as EventSubject; }
        }

        #endregion

        /// <summary>
        /// 开始事件侦听服务
        /// </summary>
        public void Start()
        {
            abort = false;
            workThread = new Thread(new ThreadStart(Listen)); // 开始监听
            workThread.Priority = ThreadPriority.AboveNormal;
            workThread.IsBackground = true;
            workThread.Start();
            Subject.Enable();
        }

        /// <summary>
        /// 停止事件侦听服务
        /// </summary>
        public void Stop()
        {
            abort = true;
            Thread.Sleep(5000);
            Subject.Disable(); // 停止向事件订阅者分发事件
        }

        /// <summary>
        /// 事件侦听服务的实现代码在子类中进行处理。在侦听某事件时要不断的检查abort标志是否为true，
        /// 如果为true的话则要立即停止事件的侦听并释放工作线程
        /// </summary>
        protected abstract void Listen();
    }
}
