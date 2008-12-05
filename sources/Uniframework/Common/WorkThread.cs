using System;
using System.Diagnostics;
using System.Threading;

namespace Uniframework
{
    /// <summary>
    /// 工作者线程类，用于需要进行多线程处理的任务
    /// </summary>
    public class WorkerThread : IDisposable
    {
        private ManualResetEvent threadHandle;
        private Thread threadObj;
        private ThreadStart threadStart;
        private bool abort;
        private Mutex endLoopMutex;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkerThread(ThreadStart threadStart)
        {
            this.threadStart = threadStart;
            abort = false;
            threadObj = null;
            endLoopMutex = new Mutex();
            threadHandle = new ManualResetEvent(false);
            threadObj = new Thread(threadStart);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threadStart">工作线程启动委托</param>
        /// <param name="autoStart">自动启动标识</param>
        public WorkerThread(ThreadStart threadStart, bool autoStart)
            : this(threadStart)
        {
            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>
        /// 获取工作者线程的哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return threadObj.GetHashCode();
        }

        /// <summary>
        /// 判断两个线程是否相等
        /// </summary>
        /// <param name="obj">待比较对象</param>
        /// <returns>如果工作者线程内部持有的线程相等则返回true，否则为false</returns>
        public override bool Equals(object obj)
        {
            return threadObj.Equals(obj);
        }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ManagedThreadId
        {
            get { return threadObj.ManagedThreadId; }
        }

        /// <summary>
        /// 内部线程
        /// </summary>
        public Thread Thread
        {
            get { return threadObj; }
        }

        /// <summary>
        /// 线程结束标志
        /// </summary>
        protected bool Abort
        {
            set
            {
                endLoopMutex.WaitOne();
                abort = value;
                endLoopMutex.ReleaseMutex();
            }

            get
            {
                bool result = false;
                endLoopMutex.WaitOne();
                result = abort;
                endLoopMutex.ReleaseMutex();
                return result;
            }
        }

        /// <summary>
        /// 等待句柄
        /// </summary>
        public WaitHandle Handle
        {
            get { return threadHandle; }
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        public void Start()
        {
            Debug.Assert(threadObj != null);
            Debug.Assert(threadObj.IsAlive == false);
            threadObj.Start();
        }

        /// <summary>
        /// 工作者线程的Run方法，在子类中要重写此方法
        /// </summary>
        protected virtual void Run()
        {
            while (!abort)
            {
                try
                {
                    threadStart();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 关闭工作者线程
        /// </summary>
        public void Kill()
        {
            Debug.Assert(threadObj != null);
            if (IsAlive == false)
                return;
            abort = true;
            Join();
            endLoopMutex.Close();
            threadHandle.Close();
        }

        /// <summary>
        /// Join
        /// </summary>
        public void Join()
        {
            Join(Timeout.Infinite);
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Join(int millisecondsTimeout)
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(millisecondsTimeout);
            return Join(timeout);
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Join(TimeSpan timeout)
        {
            Debug.Assert(threadObj != null);
            if (IsAlive == false)
                return true;
            Debug.Assert(Thread.CurrentThread.ManagedThreadId != threadObj.ManagedThreadId);
            return threadObj.Join(timeout);
        }

        /// <summary>
        /// workerthread's name
        /// </summary>
        public string Name
        {
            get { return threadObj.Name; }
            set { threadObj.Name = value; }
        }

        /// <summary>
        /// determine the workerthread is alive?
        /// </summary>
        public bool IsAlive
        {
            get
            {
                Debug.Assert(threadObj != null);
                bool handleSignaled = threadHandle.WaitOne(0, true);
                while (handleSignaled == threadObj.IsAlive)
                {
                    Thread.Sleep(0);
                }
                return threadObj.IsAlive;
            }
        }

        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            Kill();
        }
    }
}