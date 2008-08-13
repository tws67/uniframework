using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
    /// <summary>
    /// 离线请求分配器
    /// </summary>
    public class OfflineRequestDispatcher : IRequestDispatcher
    {
        private readonly static string ReturnCallbackFailed = "回调返回值方法失败，";

        #region IRequestDispatcher Members

        public DispatchResult Dispatch(Request request, string networkName)
        {
            int retries = 0;
            DispatchResult? dispatchResult = null;

            while (dispatchResult == null)
            {
                Exception exception = null;

                try
                {
                    retries++;
                    object result = InvokeRequest(request);
                    try
                    {
                        // Invoke return callback
                        request.Result = result;
                        if (request.ServiceAgent != null)
                        {
                            ThreadPoolInvoker threadInvoker = new ThreadPoolInvoker(request);
                            threadInvoker.ReturnResult();
                            //request.ServiceAgent.OnRemoteCallReturned(this, new RemoteCallReturnEventArgs(request));
                        }

                        InvokeReturnCallback(request, result);
                    }
                    catch (Exception ex)
                    {
                        if (ex is TargetInvocationException)
                            throw new ReturnCallbackException(ReturnCallbackFailed + ex.Message, ex.InnerException);
                        else
                            throw new ReturnCallbackException(ReturnCallbackFailed + ex.Message, ex);
                    }
                    dispatchResult = DispatchResult.Succeeded;
                }

                catch (WebException ex)
                {
                    switch (ex.Status)
                    {
                        //At server side should be the same as a failure
                        case WebExceptionStatus.NameResolutionFailure:
                        case WebExceptionStatus.ProxyNameResolutionFailure:
                        case WebExceptionStatus.ProtocolError:
                        case WebExceptionStatus.ServerProtocolViolation:
                        case WebExceptionStatus.TrustFailure:
                            exception = ex;
                            break;

                        //Otherwise should be handled out of the dispatcher.
                        default:
                            throw;
                    }
                }

                catch (Exception ex)
                {
                    exception = ex;
                    if (ex is TargetInvocationException)
                        exception = ex.InnerException;
                }

                if (exception != null)
                {
                    try
                    {
                        //Invoke the exception callback
                        if (request.ServiceAgent != null)
                        {
                            //ThreadPoolInvoker threadInvoker = new ThreadPoolInvoker(request, exception);
                            //threadInvoker.ReturnFailure();
                            request.ServiceAgent.OnRemoteCallFailure(this, new RemoteCallExceptionEventArgs(request, exception));
                        }

                        switch (InvokeExceptionRequest(request, exception))
                        {
                            case OnExceptionAction.Dismiss:
                                dispatchResult = DispatchResult.Failed;
                                break;
                            case OnExceptionAction.Retry:
                                if (retries >= request.Behavior.MaxRetries)
                                    dispatchResult = DispatchResult.Failed;
                                break;
                        }
                    }
                    catch
                    {
                        dispatchResult = DispatchResult.Failed;
                    }
                }
            }

            return (DispatchResult)dispatchResult;
        }

        #endregion

        #region Assistant functions

        /// <summary>
        /// 调用远程访求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns>返回方法调用结果</returns>
        private object InvokeRequest(Request request)
        {
            return CommunicateProxy.InvokeCommand(request.Method, request.CallParameters);
        }

        /// <summary>
        /// 执行返回值回调访求
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="result">调用远程方法返回的结果</param>
        private void InvokeReturnCallback(Request request, object result)
        {
            if (request.Behavior.ReturnCallback != null)
            {
                if (request.Method.ReturnType != typeof(void))
                    request.Behavior.ReturnCallback.Invoke(request, request.CallParameters, result);
                else
                    request.Behavior.ReturnCallback.Invoke(request, request.CallParameters);
            }
        }

        protected virtual OnExceptionAction InvokeExceptionRequest(Request request, Exception realException)
        {
            if (request.Behavior.ExceptionCallback != null)
            {
                return (OnExceptionAction)request.Behavior.ExceptionCallback.Invoke(request, realException);
            }
            return OnExceptionAction.Dismiss;
        }

        #endregion
    }

    public class ReturnCallbackException : Exception
    {
        public ReturnCallbackException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
