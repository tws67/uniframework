//=============================================================================
// Copyright ?2004 Microsoft Corporation
// All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
//=============================================================================
using System;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Uniframework.Client.ConnectionManagement
{
	/// <summary>
	/// This class is the facade over the ConnectionManager subsystem. It is responsible
	/// for ensuring that the subsystem is configured correctly, based on the configuration
	/// file, managing the thread involved with polling the connection state, and handling
	/// the event notifications as the connection state changes. It is assumed that all methods of 
	/// this class will be called from a single thread, so no attempt was made at making its public
	/// methods thread-safe.
	/// </summary>
	public class ConnectionManager
	{
		private const int ConversionToMilliSeconds = 1000;

		private ConnectionDetector detector;
		private ThreadRunner thread = null;

		private bool forcedOffline = false;
		private int pollInterval;

		/// <summary>
		/// Event raised when state of connection changes.
		/// </summary>
		public event ConnectionStateChangedEventHandler ConnectionStateChanged;

		/// <summary>
		/// Constructor for ConnectionManager.
		/// </summary>
		/// <param name="detectionStrategy">The ConnectionDetectionStrategy chosen from the config file</param>
		/// <param name="pollingIntervalInSeconds">Interval in seconds at which the ConnectionDetectionStrategy will be polled</param>
		public ConnectionManager(IConnectionDetectionStrategy detectionStrategy, int pollingIntervalInSeconds)
		{
			this.detector = new ConnectionDetector(detectionStrategy, this);
			this.pollInterval = pollingIntervalInSeconds * ConversionToMilliSeconds;
			thread = new ThreadRunner(new ThreadStart(WorkerMethod));
		}
	
		/// <summary>
		/// Starts the polling of the ConnectionDetector
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Start()
		{
            if (forcedOffline == false)
            {
                thread.Start();
            }
		}

		/// <summary>
		/// Stops polling of the ConnectionDetector. This method may block momentarily as internal threads die and are joined.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Stop()
		{
			thread.Stop();
		}

		/// <summary>
		/// Forces the system offline
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void GoOffline()
		{
			forcedOffline = true;
			Stop();
			detector.ForceOffline();
		}

		/// <summary>
		/// Forces the system online. 
		/// </summary>
		/// <exception cref="ConnectionUnavailableException">Thrown if ConnectionDetector.CanGoOnline() would return false</exception>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void GoOnline()
		{
			ThrowExceptionIfWeCannotGoOnline();

			forcedOffline = false;
			detector.ForceOnline();
			Start();
		}

		/// <summary>
		/// Returns true if we are currently polling the ConnectionDetector
		/// </summary>
		/// <returns>Returns true if we are currently polling the ConnectionDetector</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool IsPolling()
		{
			return thread.IsRunning;
		}

		/// <summary>
		/// Returns the connected state of the physical connection
		/// </summary>
		/// <value>Stores the enumerated value of the Connection State</value>
		public ConnectionState ConnectionState
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get {return this.detector.ConnectedState;}
		}

		internal void RaiseConnectionStateChangedEvent(ConnectionStateChangedEventArgs e)
		{
			EventUtility.BroadcastEvent(ConnectionStateChanged, this, e);
		}

		/// <summary>
		/// Encapulates check for whether we can go online
		/// </summary>
		private void ThrowExceptionIfWeCannotGoOnline()
		{
			if(detector.CanGoOnline() == false)
			{
				throw new ConnectionUnavailableException("无效的连接类型，请检查网络是否可用或与管理员联系。");
			}
		}

		/// <summary>
		/// Method that is invoked on worker thread to actually poll the ConnectionDetector
		/// </summary>
		private void WorkerMethod()
		{
            detector.DetectConnectionState();
            Thread.Sleep(pollInterval);
		}
    }

    #region Assistant classes

    /// <summary>
    /// WorkerThread is used by ThreadRunner to manage the housekeeping information belonging to
    /// a particular thread. There is one WorkerThread instance created for each thread to be spawned, 
    /// and each one contains its own instance of housekeeping information.
    /// </summary>
    internal class WorkerThread
    {
        private ThreadStart delegateToRun;
        private Thread workingThread;
        private bool isRunning = false;
        private static readonly int FiveSecondJoinDelay = 5000;

        /// <summary>
        /// Getter property to return running state of thread. Returns true if thread is running
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }

        /// <summary>
        /// Getter property to return the delete to run on this thread.
        /// </summary>
        protected ThreadStart DelegateToRun { get { return delegateToRun; } }

        /// <summary>
        /// Constructor for instance.
        /// </summary>
        /// <param name="delegateToRun">Delegate referring to client behavior to invoke on this thread</param>
        public WorkerThread(ThreadStart delegateToRun)
        {
            this.delegateToRun = delegateToRun;
        }

        /// <summary>
        /// Starts execution of this thread.
        /// </summary>
        public void Start()
        {
            isRunning = true;

            workingThread = new Thread(new ThreadStart(WorkerMethod));
            workingThread.Start();
        }

        /// <summary>
        /// Stops execution of this thread. This method may block momentarily while waiting for the thread to die and be joined.
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            workingThread.Join(FiveSecondJoinDelay);
            workingThread = null;
        }

        /// <summary>
        /// This method is the worker method that will be spawned on the thread managed by this class. It is
        /// the responsibility of the caller to provide the delegateToRun, which will be run inside a loop until
        /// IsRunning is set to false through calling Stop(). Callers can also provide their own implementation for
        /// WorkerMethod, changing the looping logic to be whatever they need for their particular situation.
        /// </summary>
        virtual protected void WorkerMethod()
        {
            while (isRunning)
            {
                try
                {
                    DelegateToRun();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }

    /// <summary>
    /// This class handles the mechanics of starting, stopping, and managing non-thread pool threads. All 
    /// methods of this class are thread safe.
    /// </summary>
    internal class ThreadRunner
    {
        private WorkerThread workerThread;
        private ThreadStart delegateToRun;
        private object lockObject = new object();

        /// <summary>
        /// Constructor for ThreadRunner. 
        /// </summary>
        /// <param name="delegateToRun">ThreadStart delegate for method to be run in other thread</param>
        public ThreadRunner(ThreadStart delegateToRun)
        {
            if (delegateToRun == null) throw new ArgumentNullException("delegateToRun", "Delegate to thread to be started cannot be null.");
            this.delegateToRun = delegateToRun;
        }

        /// <summary>
        /// Causes a thread to be started with the delegateToRun as the method invoked on that thread.
        /// This method will not start a second thread until the first thread has been stopped through calling
        /// the Stop() method
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (workerThread == null)
            {
                workerThread = CreateWorkerThread(DelegateToRun);
                workerThread.Start();
            }
        }

        /// <summary>
        /// Stops a thread dead in its tracks. Thread will complete when it polls value of isRunning again.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (workerThread != null)
            {
                workerThread.Stop();
                workerThread = null;
            }
        }

        /// <summary>
        /// Property to return current state of embedded thread.
        /// </summary>
        public bool IsRunning
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (workerThread == null) return false;
                return workerThread.IsRunning;
            }
        }

        /// <summary>
        /// Factory method to create the worker thread. Subclasses may override this to change the type of worker thread
        /// to create
        /// </summary>
        /// <param name="delegateToRun">Delegate referring to the client behavior to invoke on worker thread</param>
        /// <returns></returns>
        protected virtual WorkerThread CreateWorkerThread(ThreadStart delegateToRun)
        {
            return new WorkerThread(delegateToRun);
        }

        /// <summary>
        /// Getter property to retrieve the delegate to run
        /// </summary>
        protected ThreadStart DelegateToRun { get { return delegateToRun; } }
    }

    #endregion
}
