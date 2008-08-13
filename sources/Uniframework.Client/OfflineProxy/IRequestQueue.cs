using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
    public delegate void RequestQueueChangedEventHandler(object sender, RequestQueueEventArgs e);

    public class RequestQueueEventArgs : EventArgs
    {
        private Request request;
        private int queueSize;

        public RequestQueueEventArgs(Request request, int queueSize)
        {
            this.request = request;
            this.queueSize = queueSize;
        }

        public Request Request
        {
            get { return request; }
        }

        public int QueueSize
        {
            get { return queueSize; }
        }
    }

    /// <summary>
    /// 请求队列接口
    /// </summary>
	public interface IRequestQueue
	{
        event RequestQueueChangedEventHandler RequestQueueChanged;

		void Enqueue(Request request);
		int GetCount();
		Request GetNextRequest();
        Request GetRequest(Guid requestId);
		IList<Request> GetRequests(string tag);
		IList<Request> GetRequests(uint stampsEqualOrMoreThan);
		IList<Request> GetRequests();
		void Remove(Request request);
	}
}
