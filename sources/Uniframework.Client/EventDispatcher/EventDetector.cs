using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Uniframework.Services;

namespace Uniframework.Client
{
    public class EventDetector : IDisposable
    {
        Thread thread;
        string sessionId;
        ClientEventDispatcher clientDispatcher;
        log4net.ILog logger;
        bool isRunning = true;

        public EventDetector(log4net.ILog logger, string sessionId, ClientEventDispatcher clientDispatcher)
        {
            this.sessionId = sessionId;
            this.clientDispatcher = clientDispatcher;
            this.logger = logger;
        }

        public void Start()
        {
            thread = new Thread(new ThreadStart(Run));
            thread.IsBackground = true;
            thread.Start();
        }

        private void Run()
        {
            while (isRunning)
            {
                if (CommunicateProxy.ServerReady)
                {
                    if (clientDispatcher.HasSubscribers)
                    {
                        try
                        {
                            IEventDispatcher dispatcher = ServiceRepository.Instance.GetService<IEventDispatcher>();
                            EventResultData[] results = dispatcher.GetOuterEventResults();
                            logger.Debug("从服务器获取到 " + results.Length + "个事件");
                            if (results.Length > 0)
                            {
                                foreach (EventResultData result in results)
                                {
                                    try
                                    {
                                        clientDispatcher.DispachEvent(result.Topic, result.Args);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Warn("分发事件时发生错误", ex);
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                Thread.Sleep(100); // 防止服务器不可用造成耗费大量资源的死循环
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            isRunning = false;
        }

        #endregion
    }
}
