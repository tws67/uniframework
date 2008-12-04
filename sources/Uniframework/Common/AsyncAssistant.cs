using System;
using System.Diagnostics;
using System.Threading;

namespace Uniframework
{
    /// <summary>
    /// 异步操作帮助类
    /// </summary>
    public class AsyncAssistant
    {
        private bool busy;
        private bool cancelled;
        private int waitEventIndex;

        private AutoResetEvent completionEvent = new AutoResetEvent(false);
        private AutoResetEvent errorEvent = new AutoResetEvent(false);
        private AutoResetEvent cancelEvent = new AutoResetEvent(false);
        private AutoResetEvent[] resultEvent;
        private Exception lastException;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncHelper"/> class.
        /// </summary>
        public AsyncAssistant()
        {
            resultEvent = new AutoResetEvent[3] { completionEvent, errorEvent, cancelEvent };
        }

        #region Async
        
        /// <summary>
        /// 是否正在操作
        /// </summary>
        public bool Busy
        {
            get { return busy; }
        }

        /// <summary>
        /// 异步操作发生的异常
        /// </summary>
        public Exception Exception
        {
            get { return lastException; }
        }
        
        /// <summary>
        /// 异步操作是否被取消
        /// </summary>
        public bool Cancelled
        {
            get { return cancelled; }
        }
      
        ///// <summary>
        ///// 开始新的异步操作
        ///// </summary>
        //public  void BeginAsyncOperation()
        //{
        //    if (Busy) {
        //        throw new UniframeworkException("It's busy, cann't start a new async operation");
        //    }
        //    cancelled = false;
        //    lastException = null;
        //    busy = true;

        //    foreach (AutoResetEvent e in resultEvent) {
        //        e.Reset();
        //    }
        //}

        /// <summary>
        /// 结束异步操作
        /// </summary>
        public void EndAsyncOperation()
        {
            busy = false;
            cancelled = false;
            lastException = null;
            completionEvent.Set();
        }

        /// <summary>
        /// 发生错误结束异步操作
        /// </summary>
        /// <param name="e">描述错误的异常</param>
        public void EndAsyncOperationWithError(Exception e)
        {
            busy = false;
            cancelled = false;
            lastException = e;
            errorEvent.Set();
        }

        /// <summary>
        /// 取消异步操作
        /// </summary>
        public void CancelAsyncOperation()
        {
            cancelEvent.Set();
        }

        public int WaitEventIndex
        {
            get { return waitEventIndex; }
        }

        /// <summary>
        /// 等待异步操作返回
        /// </summary>
        /// <param name="timeOut">等待的超时值.</param>
        public void WaitAsyncResult(int timeOut)
        {
            int index = WaitHandle.WaitAny(resultEvent, timeOut, true);
            waitEventIndex = index;
            switch (index) {
                case 0: // 异步操作已经完成
                    cancelled = false;
                    lastException = null;
                    break;

                case 1: // 异步操作发生错误
                    if(lastException!=null)
                        throw lastException;
                    break;

                case 2: // 异步操作已经取消
                    cancelled = true;
                    busy = false;
                    throw  new CancelAsyncOperationException();

                case WaitHandle.WaitTimeout: // 异步操作超时
                    cancelled = false;
                    busy = false;
                    lastException = null;
                    throw new TimeoutException();

                default:
                    Debug.Assert(false);
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// 异步被取消异常
    /// </summary>
    public class CancelAsyncOperationException : UniframeworkException
    {
        public CancelAsyncOperationException()
            :base("User cancel async operation")
        {
        }
    }
}
