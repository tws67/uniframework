using System;
using System.Threading;

namespace Uniframework
{
    /// <summary>
    /// ͬ��������
    /// </summary>
    public class AsyncHelper
    {
        bool busy;
        bool cancelled;

        AutoResetEvent completionEvent = new AutoResetEvent(false);
        AutoResetEvent errorEvent = new AutoResetEvent(false);
        AutoResetEvent cancelEvent = new AutoResetEvent(false);
        AutoResetEvent[] resultEvent;
        Exception lastException;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncHelper"/> class.
        /// </summary>
        public AsyncHelper()
        {
            resultEvent = new AutoResetEvent[3] { completionEvent, errorEvent, cancelEvent };
        }

        #region Async
        
        /// <summary>
        /// �Ƿ����ڲ���
        /// </summary>
        public bool Busy
        {
            get { return busy; }
        }

        /// <summary>
        /// �첽�����������쳣
        /// </summary>
        public Exception Exception
        {
            get { return lastException; }
        }
        
        /// <summary>
        /// �첽�����Ƿ�ȡ��
        /// </summary>
        public bool Cancelled
        {
            get { return cancelled; }
        }
      
        /// <summary>
        /// ��ʼ�µ��첽����
        /// </summary>
        public  void BeginAsyncOperation()
        {
            if (Busy)
            {
                throw new RuntimeException("It's busy, can start a new async operation");
            }
            cancelled = false;
            lastException = null;
            busy = true;

            foreach (AutoResetEvent e in resultEvent)
            {
                e.Reset();
            }
        }

        /// <summary>
        /// �����첽����
        /// </summary>
        public void EndAsyncOperation()
        {
            cancelled = false;
            lastException = null;
            busy = false;
            completionEvent.Set();
        }

        /// <summary>
        /// ������������첽����
        /// </summary>
        /// <param name="e">����������쳣</param>
        public void EndAsyncOperationWithError(Exception e)
        {
            busy = false;
            cancelled = false;
            lastException = e;
            errorEvent.Set();
        }

        /// <summary>
        /// ȡ���첽����
        /// </summary>
        public void CancelAsyncOperation()
        {
            cancelEvent.Set();
        }

        int waitEventIndex;

        public int WaitEventIndex
        {
            get { return waitEventIndex; }
        }

        public void WaitAsyncResult(int timeOut)
        {
            int index = WaitHandle.WaitAny(resultEvent, timeOut, true);
            waitEventIndex = index;
            switch (index)
            {
                case 0: //�첽�����Ѿ����
                    cancelled = false;
                    lastException = null;
                    break;
                case 1: //�첽������������
                    
                    if(lastException!=null)
                        throw lastException;
                    break;
                case 2: //�첽�����Ѿ�ȡ��
                    cancelled = true;
                    busy = false;
                    throw  new CancelAsyncOperationException();
                case WaitHandle.WaitTimeout: //�첽������ʱ
                    cancelled = false;
                    busy = false;
                    lastException = null;
                    throw new TimeoutException();
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// ͬ��ȡ���쳣��Ϣ
    /// </summary>
    public class CancelAsyncOperationException : RuntimeException
    {
        public CancelAsyncOperationException()
            :base("User cancel async operation")
        {
        }
    }
}
