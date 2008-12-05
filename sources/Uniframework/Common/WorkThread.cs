using System;
using System.Diagnostics;
using System.Threading;

namespace Uniframework
{
    /// <summary>
    /// �������߳��࣬������Ҫ���ж��̴߳��������
    /// </summary>
    public class WorkerThread : IDisposable
    {
        private ManualResetEvent threadHandle;
        private Thread threadObj;
        private ThreadStart threadStart;
        private bool abort;
        private Mutex endLoopMutex;

        /// <summary>
        /// ���캯��
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
        /// ���캯��
        /// </summary>
        /// <param name="threadStart">�����߳�����ί��</param>
        /// <param name="autoStart">�Զ�������ʶ</param>
        public WorkerThread(ThreadStart threadStart, bool autoStart)
            : this(threadStart)
        {
            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>
        /// ��ȡ�������̵߳Ĺ�ϣֵ
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return threadObj.GetHashCode();
        }

        /// <summary>
        /// �ж������߳��Ƿ����
        /// </summary>
        /// <param name="obj">���Ƚ϶���</param>
        /// <returns>����������߳��ڲ����е��߳�����򷵻�true������Ϊfalse</returns>
        public override bool Equals(object obj)
        {
            return threadObj.Equals(obj);
        }

        /// <summary>
        /// �߳�ID
        /// </summary>
        public int ManagedThreadId
        {
            get { return threadObj.ManagedThreadId; }
        }

        /// <summary>
        /// �ڲ��߳�
        /// </summary>
        public Thread Thread
        {
            get { return threadObj; }
        }

        /// <summary>
        /// �߳̽�����־
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
        /// �ȴ����
        /// </summary>
        public WaitHandle Handle
        {
            get { return threadHandle; }
        }

        /// <summary>
        /// �����߳�
        /// </summary>
        public void Start()
        {
            Debug.Assert(threadObj != null);
            Debug.Assert(threadObj.IsAlive == false);
            threadObj.Start();
        }

        /// <summary>
        /// �������̵߳�Run��������������Ҫ��д�˷���
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
        /// �رչ������߳�
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