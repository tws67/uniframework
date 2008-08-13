using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
    public class MemoryRequestQueue : IRequestQueue, IDisposable
    {
        private Dictionary<Guid, Request> requestQueue = new Dictionary<Guid, Request>();

        #region IRequestQueue Members

        public event RequestQueueChangedEventHandler RequestQueueChanged;

        public void Enqueue(Request request)
        {
            if (!requestQueue.ContainsKey(request.RequestId))
            {
                requestQueue.Add(request.RequestId, request);
                if (RequestQueueChanged != null)
                    RequestQueueChanged(this, new RequestQueueEventArgs(request, requestQueue.Count));
            }
        }

        public int GetCount()
        {
            return requestQueue.Count;
        }

        public Request GetNextRequest()
        {
            throw new NotImplementedException();
        }

        public IList<Request> GetRequests(string tag)
        {
            List<Request> requests = new List<Request>();
            foreach (Request request in requestQueue.Values)
            {
                if (request.Behavior.Tag == tag)
                    requests.Add(request);
            }
            return requests;
        }

        public IList<Request> GetRequests(uint stampsEqualOrMoreThan)
        {
            List<Request> requests = new List<Request>();
            foreach (Request request in requestQueue.Values)
            {
                if (request.Behavior.Stamps >= stampsEqualOrMoreThan)
                    requests.Add(request);
            }
            return requests;
        }

        public IList<Request> GetRequests()
        {
            List<Request> requests = new List<Request>();
            requests.AddRange(requestQueue.Values);
            return requests;
        }

        public Request GetRequest(Guid requestId)
        {
            if (requestQueue.ContainsKey(requestId))
                return requestQueue[requestId];
            return null;
        }

        public void Remove(Request request)
        {
            if (requestQueue.ContainsKey(request.RequestId))
            {
                requestQueue.Remove(request.RequestId);
                if (RequestQueueChanged != null)
                    RequestQueueChanged(this, new RequestQueueEventArgs(request, requestQueue.Count));
            }
        }

        #endregion

        #region IDisposable Members

        private bool disposed = false;
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                requestQueue.Clear();
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
