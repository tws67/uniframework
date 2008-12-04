using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

using Uniframework.Client.ConnectionManagement;
using Uniframework.Db4o;
using Uniframework.Services;

namespace Uniframework.Client.OfflineProxy
{
    /// <summary>
    /// 本地请求管理器用于对所有的离线请求进行处理
    /// </summary>
	public sealed class RequestManager
	{
		private static volatile RequestManager instance;
		private static object syncRoot = new object();
		private static object syncLockObject = new object();
		private IRequestQueue requestQueue;
		private IRequestQueue deadLetterQueue;
		private IRequestDispatcher requestDispatcher;
        private ConnectionManager connectionManager;
		private DispatchRequestThread thread;
		private bool running = false;
		private Queue<Command> dispatchCommands;

		/// <summary>
        /// 当请求被分发后触发此事件
		/// </summary>
		public event EventHandler<RequestDispatchedEventArgs> RequestDispatched;

        /// <summary>
        /// 构造函数
        /// </summary>
		private RequestManager()
		{
            this.requestQueue = new MemoryRequestQueue();
            this.deadLetterQueue = new MemoryRequestQueue();
            this.connectionManager = new ConnectionManager(new WinNetDetectionStrategy(), 1);
            this.dispatchCommands = new Queue<Command>();
            this.requestDispatcher = new OfflineRequestDispatcher();
            this.connectionManager.Start();
		}

        /// <summary>
        /// 离线请求管理器单件实例
        /// </summary>
        public static RequestManager Instance
        {
            get {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new RequestManager();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 采用db4o数据库作为缓存队列存储方案
        /// </summary>
        /// <param name="databaseService">db4o数据库服务</param>
        public void Initilize(IDb4oDatabaseService databaseService)
        {
            this.requestQueue = new db4oRequestQueue(databaseService, "OfflineProxy.Requests.yap");
            this.deadLetterQueue = new db4oRequestQueue(databaseService, "OfflineProxy.DeadRequests.yap");
        }

		/// <summary>
		/// Starts the automatic dispatch regarding connectivity events and enquing.
		/// </summary>
		public void StartAutomaticDispatch()
		{
			if (running) return;
			running = true;
            requestQueue.RequestQueueChanged += new RequestQueueChangedEventHandler(OnAutomaticDispatch);
			connectionManager.ConnectionStateChanged += new ConnectionStateChangedEventHandler(OnConnectionDispatch);
			if (connectionManager.ConnectionState == ConnectionState.Online)
                DispatchAllPendingRequests();
		}

		/// <summary>
		/// Stops the automatic dispatch.
		/// </summary>
		public void StopAutomaticDispatch()
		{
			if (!running) return;
			connectionManager.ConnectionStateChanged -= new ConnectionStateChangedEventHandler(OnConnectionDispatch);
			requestQueue.RequestQueueChanged -= new RequestQueueChangedEventHandler(OnAutomaticDispatch);
			running = false;
        }

        #region Dispatch functions

		public void DispatchPendingRequestsByTag(string tag)
		{
			DispatchRequests(new Command(this, "GetPendingRequestsByTag", tag));
		}

		public void DispatchAllPendingRequests()
		{
			DispatchRequests(new Command(this, "GetPendingRequests"));
		}

		public void DispatchRequest(Request request)
		{
			Guard.ArgumentNotNull(request, "request");
			DispatchRequests(new Command(this, "GetRequest", request.RequestId));
        }

        #endregion

        #region DispatchRequestThread

        private class DispatchRequestThread
        {
            private Thread thread;
            private RequestManager manager;
            private Queue<Command> commands;
            private bool stop;
            private object syncObj;

            public DispatchRequestThread(RequestManager manager, Queue<Command> commands, object syncObj)
            {
                this.manager = manager;
                this.commands = commands;
                this.syncObj = syncObj;
            }

            public void Start()
            {
                thread = new Thread(new ThreadStart(DispatchRequests));
                thread.Start();
            }


            private void DispatchRequests()
            {
                try
                {
                    while (stop == false)
                    {
                        IEnumerable<Request> requests;
                        lock (syncObj)
                        {
                            if (commands.Count == 0)
                                break;
                            requests = GetRequestsFromNextCommand();

                            foreach (Request request in requests)
                            {
                                //If there is not a connection stops the thread and doesn't remove the command from the queue.
                                if (manager.connectionManager.ConnectionState == ConnectionState.Offline)
                                {
                                    stop = true;
                                    break;
                                }

                                try
                                {
                                    DispatchRequestInternal(request, ""); // 此处省略了网络名称
                                }
                                catch
                                {
                                    stop = true;
                                    break;
                                }
                            }

                            if (!stop)
                                commands.Dequeue();
                        }
                    }
                }
                finally
                {
                    manager.thread = null;
                }
            }

            /// <summary>
            /// 处理请求队列中的调用
            /// </summary>
            /// <param name="request">请求</param>
            /// <param name="networkName">网络名称</param>
            private void DispatchRequestInternal(Request request, string networkName)
            {
                DispatchResult result = DispatchResult.Expired;

                if (request.Behavior.Expiration == null || request.Behavior.Expiration >= DateTime.Now)
                {
                    try
                    {
                        result = manager.requestDispatcher.Dispatch(request, networkName);
                    }
                    catch (WebException)
                    {
                        //Any WebException should fail the dispatch request
                        result = DispatchResult.Failed;
                    }

                    if (result == DispatchResult.Failed)
                    {
                        if (manager.connectionManager.ConnectionState == ConnectionState.Offline)
                            return;
                        manager.deadLetterQueue.Enqueue(request);
                    }
                }
                manager.requestQueue.Remove(request);

                if (manager.RequestDispatched != null)
                    manager.RequestDispatched(this, new RequestDispatchedEventArgs(request, result));
            }

            private IEnumerable<Request> GetRequestsFromNextCommand()
            {
                if (commands.Peek().CommandName == "GetRequest")
                {
                    Request requestByGuid = (Request)commands.Peek().Execute();
                    if (requestByGuid == null)
                        return new Request[] { };
                    else
                        return new Request[] { requestByGuid };
                }
                else
                    return (IEnumerable<Request>)commands.Peek().Execute();
            }

            public bool Join(int timeout)
            {
                return thread.Join(timeout);
            }

            public void Stop()
            {
                stop = true;
            }
        }

        #endregion

        #region RequestManager Members

        /// <summary>
        /// get the connection manager
        /// </summary>
        public ConnectionManager ConnectionManager
        {
            get { return connectionManager; }
        }

        /// <summary>
		/// Gets the pending requests queue.
		/// </summary>
		public IRequestQueue RequestQueue
		{
			get { return requestQueue; }
		}

		/// <summary>
		/// Gets the failed requests queue.
		/// </summary>
		public IRequestQueue DeadLetterQueue
		{
			get { return deadLetterQueue; }
		}

		/// <summary>
		/// Wait for the dispatch inner thread finish.
		/// </summary>
		/// <param name="timeout">Milliseconds to wait.</param>
		/// <returns>true if the thread has finished before timeout, otherwise false.</returns>
		public bool Join(int timeout)
		{
			lock (this)
			{
				if (thread == null)
					return true;

				return thread.Join(timeout);
			}
		}

		/// <summary>
		/// Gets the automatic dispatch running state of the RequestManager.
		/// </summary>
		public bool Running
		{
			get { return running; }
		}

		/// <summary>
		/// Stops the dispatching thread.
		/// </summary>
		public void Stop()
		{
			lock (this)
			{
				if (thread != null)
					thread.Stop();
			}
        }

        #endregion

        #region Command Methods

        public Request GetRequest(Guid requestId)
		{
			return requestQueue.GetRequest(requestId);
		}

		public IEnumerable<Request> GetPendingRequestsByTag(string tag)
		{
            return requestQueue.GetRequests(tag);
		}

		public IEnumerable<Request> GetPendingRequests()
		{
            return requestQueue.GetRequests();
		}

        //public IEnumerable<Request> GetRequestsForCurrentConnectionPrice()
        //{
        //    uint? price = null;
        //    try
        //    { 
        //        price = connectionMonitor.CurrentConnectionPrice; 
        //    }
        //    catch 
        //    { 
        //        price = null;
        //        thread.Stop();
        //    } //Exception getting the currentConnectionPrice seems connectivity issue

        //    if (price != null)
        //        foreach (Request request in requestQueue.GetRequests((uint)price))
        //        {
        //            yield return request;
        //        }
        //}

		#endregion

        #region Assistant functions 
        
        private void OnAutomaticDispatch(object sender, RequestQueueEventArgs e)
        {
            if (connectionManager.ConnectionState == ConnectionState.Online)
            {
                DispatchAllPendingRequests();
            }
        }

        private void OnConnectionDispatch(object sender, ConnectionStateChangedEventArgs e)
        {
            if (thread != null)
            {
                //Stop current dispatching
                thread.Stop();
                thread.Join(25000);
            }

            if (e.CurrentState == ConnectionState.Online)
            {
                DispatchAllPendingRequests();
            }
        }

        private void DispatchRequests(Command command)
        {
            lock (syncLockObject)
            {
                dispatchCommands.Enqueue(command);
                if (thread == null)
                {
                    thread = new DispatchRequestThread(this, dispatchCommands, syncLockObject);
                    thread.Start();
                }
            }
        }

        #endregion
	}
}
