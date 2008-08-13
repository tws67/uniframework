using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
    /// <summary>
    /// 池化的反射调用器
    /// </summary>
    public class ThreadPoolInvoker
    {
        private Request request;
        private Exception exception;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public ThreadPoolInvoker(Request request)
        {
            this.request = request;
        }

        /// <summary>
        /// 构造函数（重载）
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="exception">执行请求时抛出的异常</param>
        public ThreadPoolInvoker(Request request, Exception exception)
            : this(request)
        {
            this.exception = exception;
        }

        /// <summary>
        /// 向客户端调用方返回结果
        /// </summary>
        public void ReturnResult()
        {
            WaitCallback methodInvoke = new WaitCallback(InvokeReturnOnAnotherThread);
            ThreadPool.QueueUserWorkItem(methodInvoke);
        }

        /// <summary>
        /// 向客户端调用方返回调用失败信息
        /// </summary>
        public void ReturnFailure()
        {
            WaitCallback methodInvoke = new WaitCallback(InvokeFailureOnAnothrerThread);
            ThreadPool.QueueUserWorkItem(methodInvoke);
        }

        #region Assistant functions

        private void InvokeReturnOnAnotherThread(object threadInfo)
        {
            MethodInfo method = request.ServiceAgent.GetType().GetMethod("OnRemoteCallReturned",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //FastInvokeHandler invoker = FastInvokeUtility.GetMethodInvoker(method);
            //invoker(request.ServiceAgent, new object[] { null, new RemoteCallReturnEventArgs(request) });
            method.Invoke(request.ServiceAgent, new object[] { null, new RemoteCallReturnEventArgs(request) });
        }

        private void InvokeFailureOnAnothrerThread(object threadInfo)
        {
            MethodInfo method = request.ServiceAgent.GetType().GetMethod("OnRemoteCallFailure",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //FastInvokeHandler invoker = FastInvokeUtility.GetMethodInvoker(method);
            //invoker(request.ServiceAgent, new object[] { null, new RemoteCallExceptionEventArgs(request, exception) });
            method.Invoke(request.ServiceAgent, new object[] { null, new RemoteCallExceptionEventArgs(request, exception) });
        }

        #endregion
    }
}
