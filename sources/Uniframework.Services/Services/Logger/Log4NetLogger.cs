using System;
using System.Collections.Generic;
using System.Text;

using log4net;

namespace Uniframework.Services
{
    /// <summary>
    /// log4net��־��¼��
    /// </summary>
    public class Log4NetLogger : ILogger
    {
        private ILog log;
        private string group;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="group">The group.</param>
        public Log4NetLogger(Type type, string group)
        {
            this.group = group;
            log = LogManager.GetLogger(type);
        }

        #region Assistant function

        string GetMessage(string msg)
        {
            return group + " " + msg;
        }

        #endregion

        #region ILogger Members

        /// <summary>
        /// ��¼���Լ������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        public void Debug(string message)
        {
            log.Debug(GetMessage(message));
        }

        /// <summary>
        /// ��¼���Լ������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Debug(string message, Exception ex)
        {
            log.Debug(GetMessage(message), ex);
        }

        /// <summary>
        /// ��¼��Ϣ�������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        public void Info(string message)
        {
            log.Info(GetMessage(message));
        }

        /// <summary>
        /// ��¼��Ϣ�������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Info(string message, Exception ex)
        {
            log.Info(GetMessage(message), ex);
        }

        /// <summary>
        /// ��¼���漶�����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        public void Warn(string message)
        {
            log.Warn(GetMessage(message));
        }

        /// <summary>
        /// ��¼���漶�����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Warn(string message, Exception ex)
        {
            log.Warn(GetMessage(message), ex);
        }

        /// <summary>
        /// ��¼���󼶱����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        public void Error(string message)
        {
            log.Error(GetMessage(message));
        }

        /// <summary>
        /// ��¼���󼶱����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Error(string message, Exception ex)
        {
            log.Error(GetMessage(message), ex);
        }

        /// <summary>
        /// ��¼�������󼶱����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        public void Fatal(string message)
        {
            log.Fatal(GetMessage(message));
        }

        /// <summary>
        /// ��¼��������L�������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        public void Fatal(string message, Exception ex)
        {
            log.Fatal(GetMessage(message), ex);
        }

        #endregion
    }
}
