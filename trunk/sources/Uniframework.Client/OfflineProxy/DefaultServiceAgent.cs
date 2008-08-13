using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
    public delegate void RemoteCallRetrunEventHandler(object sender, RemoteCallReturnEventArgs e);
    public delegate void RemoteCallExceptionEventHandler(object sender, RemoteCallExceptionEventArgs e);

    public class DefaultServiceAgent
    {
        public event RemoteCallRetrunEventHandler RemoteCallReturnEvent;
        public event RemoteCallExceptionEventHandler RemoteCallExceptionEvent;

        internal void OnRemoteCallReturned(object sender, RemoteCallReturnEventArgs e)
        {
            if (RemoteCallReturnEvent != null)
                RemoteCallReturnEvent(sender, e);
        }

        internal void OnRemoteCallFailure(object sender, RemoteCallExceptionEventArgs e)
        {
            if (RemoteCallExceptionEvent != null)
                RemoteCallExceptionEvent(sender, e);
        }
    }

    #region Assistant classes

    /// <summary>
    /// 远程调用返回事件参数
    /// </summary>
    public class RemoteCallReturnEventArgs : EventArgs
    {
        private Request request;

        public RemoteCallReturnEventArgs(Request request)
        {
            this.request = request;
        }

        public Request Request
        {
            get { return request; }
        }
    }

    /// <summary>
    /// 远程调用失败事件参数
    /// </summary>
    public class RemoteCallExceptionEventArgs : EventArgs
    {
        private Request request;
        private Exception exception;

        public RemoteCallExceptionEventArgs(Request request, Exception exception)
        {
            this.request = request;
            this.exception = exception;
        }

        public Request Request
        {
            get { return request; }
        }

        public Exception Exception
        {
            get { return exception; }
        }
    }

    #endregion
}
