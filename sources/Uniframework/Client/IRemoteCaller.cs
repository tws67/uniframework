using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;

namespace Uniframework.Client
{
    /// <summary>
    /// 调用结束的处理方法委托
    /// </summary>
    /// <param name="result"></param>
    public delegate void InvokedFinishedHandler(InvokedResult result);

    /// <summary>
    /// 调用失败的处理方法委托
    /// </summary>
    /// <param name="errorInfo"></param>
    public delegate void InvokedErrorHandler(InvokeErrorInfo errorInfo);

    #region IRemoteCaller
    /// <summary>
    /// 远程调用者的接口
    /// </summary>
    public interface IRemoteCaller
    {
        /// <summary>
        /// 调用结束事件
        /// </summary>
        event InvokedFinishedHandler InvokedMethodFinished;

        /// <summary>
        /// 调用失败事件
        /// </summary>
        event InvokedErrorHandler InvokeMethodFailed;
    }

    #endregion

    #region InvokedResult
    /// <summary>
    /// 方法调用结果
    /// </summary>
    public class InvokedResult
    {
        private string methodName;
        private object result;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="result">结果</param>
        public InvokedResult(string methodName, object result)
        {
            this.methodName = methodName;
            this.result = result;
        }

        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName
        {
            get { return methodName; }
        }

        /// <summary>
        /// 结果
        /// </summary>
        public object Result
        {
            get { return result; }
        }
    }

    #endregion

    #region InvokeErrorInfo
    /// <summary>
    /// 方法调用的异常信息
    /// </summary>
    public class InvokeErrorInfo
    {
        private string methodName;
        private Exception failureReason = null;
        private Hashtable requestData;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="exception">异常信息</param>
        /// <param name="requestData">所请求的数据</param>
        public InvokeErrorInfo(string methodName, Exception exception, Hashtable requestData)
        {
            this.methodName = methodName;
            this.requestData = requestData;
        }

        /// <summary>
        /// 获取方法名
        /// </summary>
        public string MethodName
        {
            get { return methodName; }
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        public Exception FailureReason
        {
            get { return failureReason; }
        }

        /// <summary>
        /// 获取所请求的数据
        /// </summary>
        public Hashtable RequestData
        {
            get { return requestData; }
        }
    }

    #endregion
}
