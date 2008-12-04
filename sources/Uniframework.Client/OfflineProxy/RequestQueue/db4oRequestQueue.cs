using System;
using System.Collections.Generic;
using System.Text;

using Uniframework.Db4o;
using Uniframework.Services;

namespace Uniframework.Client.OfflineProxy
{
    /// <summary>
    /// db4o请求队列
    /// </summary>
    public class db4oRequestQueue : IRequestQueue, IDisposable
    {
        private IDb4oDatabase dbService;
        
        /// <summary>
        /// db4o请求队列构造函数
        /// </summary>
        /// <param name="databaseService">db4o数据库服务</param>
        public db4oRequestQueue(IDb4oDatabaseService databaseService)
            : this(databaseService, "Request")
        { }

        /// <summary>
        /// 构造函数（重载）
        /// </summary>
        /// <param name="databaseService">db4o数据库服务</param>
        /// <param name="databaseName">请求队列存放的数据库名称</param>
        public db4oRequestQueue(IDb4oDatabaseService databaseService, string databaseName)
        {
            dbService = databaseService.Open(databaseName);
        }

        #region IRequestQueue Members

        public event RequestQueueChangedEventHandler RequestQueueChanged;

        public void Enqueue(Request request)
        {
            IList<Request> results = dbService.Load<Request>(delegate(Request queryRequest) {
                return queryRequest.RequestId == request.RequestId;
            });
            if (results.Count == 0)
            {
                dbService.Store(request);
                if (RequestQueueChanged != null)
                    RequestQueueChanged(this, new RequestQueueEventArgs(request, GetCount()));
            }
        }

        public int GetCount()
        {
            return dbService.Load<Request>().Count;
        }

        public Request GetNextRequest()
        {
            throw new NotImplementedException();
        }

        public IList<Request> GetRequests(string tag)
        {
            return dbService.Load<Request>(delegate(Request request) {
                return request.Behavior.Tag == tag;
            });
        }

        public IList<Request> GetRequests(uint stampsEqualOrMoreThan)
        {
            return dbService.Load<Request>(delegate(Request request) {
                return request.Behavior.Stamps >= stampsEqualOrMoreThan;
            });
        }

        public IList<Request> GetRequests()
        {
            return dbService.Load<Request>();
        }

        public Request GetRequest(Guid requestId)
        {
            IList<Request> results = dbService.Load<Request>(delegate(Request request) {
                return request.RequestId == requestId;
            });
            if(results.Count > 0)
                return results[0];
            return null;
        }

        public void Remove(Request request)
        {
            IList<Request> results = dbService.Load<Request>(delegate(Request queryRequest) {
                return request.RequestId == queryRequest.RequestId;
            });
            if (results.Count > 0)
            {
                dbService.Delete(results[0]);
                if (RequestQueueChanged != null)
                    RequestQueueChanged(this, new RequestQueueEventArgs(results[0], GetCount()));
            }
        }

        #endregion

        #region IDisposable Members

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                foreach (Request request in dbService.Load<Request>())
                    dbService.Delete(request);
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
