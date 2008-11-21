using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

using Castle.DynamicProxy;
using Uniframework.Client.ConnectionManagement;
using Uniframework.Client.OfflineProxy;
using Uniframework.Services;

namespace Uniframework.Client
{
    public class ServiceRepository
    {
        private ProxyGenerator generator;
        private RequestManager requestManager;
        private int delayInSecondsUntilRefresh = 60;
        private DefaultServiceAgent serviceAgent;
        private Dictionary<Type, RemoteCaller> remoteCallers = new Dictionary<Type, RemoteCaller>();

        private static ServiceRepository instance;
        private static object syncObj = new object();

        public static ServiceRepository Instance
        {
            get {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new ServiceRepository();
                        }
                    }
                }
                return instance; 
            }
        }

        public void Initialize(IObjectDatabaseService databaseService)
        {
            requestManager.Initilize(databaseService);
        }

        private ServiceRepository()
        {
            generator = new ProxyGenerator();
            requestManager = RequestManager.Instance; 

            serviceAgent = new DefaultServiceAgent();
            serviceAgent.RemoteCallReturnEvent += new RemoteCallRetrunEventHandler(RemoteCallReturn);
            serviceAgent.RemoteCallExceptionEvent += new RemoteCallExceptionEventHandler(RemoteCallFailed);
        }

        #region Assistant function

        private void RemoteCallReturn(object sender, RemoteCallReturnEventArgs e)
        {
            InvokedResult result = new InvokedResult(e.Request.Method.Name, e.Request.Result);
            remoteCallers[e.Request.Method.DeclaringType].RemoteCallFinished(result);
        }

        private void RemoteCallFailed(object sender, RemoteCallExceptionEventArgs e)
        {
            Hashtable ht = new Hashtable();
            ParameterInfo[] parameters = e.Request.Method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ht.Add(parameters[i].Name, e.Request.CallParameters[i]);
            }
            InvokeErrorInfo errInfo = new InvokeErrorInfo(e.Request.Method.Name, e.Exception, ht);
            remoteCallers[e.Request.Method.DeclaringType].RemoteCallError(errInfo);
        }

        #endregion

        public int DelayInSecondsUntilRefresh
        {
            get
            {
                return delayInSecondsUntilRefresh;
            }
            set
            {
                delayInSecondsUntilRefresh = value;
            }
        }

        /// <summary>
        /// 获取服务代理类
        /// </summary>
        /// <param name="serviceType">服务接口类型</param>
        /// <returns>服务代理类</returns>
        public object GetService(Type serviceType)
        {
            ServiceProxyInterceptor interceptor = new ServiceProxyInterceptor();
            interceptor.ServiceAgent = serviceAgent;
            RemoteCaller caller;
            if (remoteCallers.ContainsKey(serviceType))
            {
                caller = remoteCallers[serviceType];
            }
            else
            {
                caller = new RemoteCaller();
                remoteCallers.Add(serviceType, caller);
            }

            ProxyGenerationOptions option = new ProxyGenerationOptions();
            option.AddMixinInstance(caller);

            object proxy = generator.CreateInterfaceProxyWithoutTarget(serviceType, null, option, interceptor);
            return proxy;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : class
        {
            return (T)GetService(typeof(T));
        }

        /// <summary>
        /// 启动连接管理器线程
        /// </summary>
        public void Start()
        {
            requestManager.StartAutomaticDispatch();
        }

        /// <summary>
        /// 网络连接状态改变事件
        /// </summary>
        public event ConnectionStateChangedEventHandler ConnectionStateChanged
        {
            add
            {
                requestManager.ConnectionManager.ConnectionStateChanged += value;
            }
            remove
            {
                requestManager.ConnectionManager.ConnectionStateChanged -= value;
            }
        }

        /// <summary>
        /// 离线请求队列变化事件
        /// </summary>
        public event RequestQueueChangedEventHandler RequestQueueChanged
        {
            add
            {
                requestManager.RequestQueue.RequestQueueChanged += value;
            }
            remove
            {
                requestManager.RequestQueue.RequestQueueChanged -= value;
            }
        }

        /// <summary>
        /// Stops the connection manager thread and dispose off the block builder
        /// instance.
        /// </summary>
        public void Dispose()
        {
            requestManager.Stop();
            //offlineBlockBuilderInstance.Dispose();
        }

        /// <summary>
        /// 强制转换为离线状态
        /// </summary>
        public void GoOffline()
        {
            requestManager.ConnectionManager.GoOffline();
        }

        /// <summary>
        /// 强制转换为在线状态
        /// </summary>
        public void GoOnline()
        {
            requestManager.ConnectionManager.GoOnline();
        }
    }
}
