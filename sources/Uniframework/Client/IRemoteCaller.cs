using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;

namespace Uniframework.Client
{
    /// <summary>
    /// ���ý����Ĵ�����ί��
    /// </summary>
    /// <param name="result"></param>
    public delegate void InvokedFinishedHandler(InvokedResult result);

    /// <summary>
    /// ����ʧ�ܵĴ�����ί��
    /// </summary>
    /// <param name="errorInfo"></param>
    public delegate void InvokedErrorHandler(InvokeErrorInfo errorInfo);

    #region IRemoteCaller
    /// <summary>
    /// Զ�̵����ߵĽӿ�
    /// </summary>
    public interface IRemoteCaller
    {
        /// <summary>
        /// ���ý����¼�
        /// </summary>
        event InvokedFinishedHandler InvokedMethodFinished;

        /// <summary>
        /// ����ʧ���¼�
        /// </summary>
        event InvokedErrorHandler InvokeMethodFailed;
    }

    #endregion

    #region InvokedResult
    /// <summary>
    /// �������ý��
    /// </summary>
    public class InvokedResult
    {
        private string methodName;
        private object result;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="methodName">������</param>
        /// <param name="result">���</param>
        public InvokedResult(string methodName, object result)
        {
            this.methodName = methodName;
            this.result = result;
        }

        /// <summary>
        /// ������
        /// </summary>
        public string MethodName
        {
            get { return methodName; }
        }

        /// <summary>
        /// ���
        /// </summary>
        public object Result
        {
            get { return result; }
        }
    }

    #endregion

    #region InvokeErrorInfo
    /// <summary>
    /// �������õ��쳣��Ϣ
    /// </summary>
    public class InvokeErrorInfo
    {
        private string methodName;
        private Exception failureReason = null;
        private Hashtable requestData;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="methodName">������</param>
        /// <param name="exception">�쳣��Ϣ</param>
        /// <param name="requestData">�����������</param>
        public InvokeErrorInfo(string methodName, Exception exception, Hashtable requestData)
        {
            this.methodName = methodName;
            this.requestData = requestData;
        }

        /// <summary>
        /// ��ȡ������
        /// </summary>
        public string MethodName
        {
            get { return methodName; }
        }

        /// <summary>
        /// ��ȡ�쳣��Ϣ
        /// </summary>
        public Exception FailureReason
        {
            get { return failureReason; }
        }

        /// <summary>
        /// ��ȡ�����������
        /// </summary>
        public Hashtable RequestData
        {
            get { return requestData; }
        }
    }

    #endregion
}
