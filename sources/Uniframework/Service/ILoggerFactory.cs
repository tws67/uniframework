using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// ��־������
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// ����һ����־��¼��
        /// </summary>
        /// <typeparam name="T">��־��¼������</typeparam>
        /// <param name="loggingPath">�����ļ�����־��¼·��</param>
        /// <returns>��־����</returns>
        ILogger CreateLogger<T>(string loggingPath);

        /// <summary>
        /// ����һ����־��¼��
        /// </summary>
        /// <typeparam name="T">��־��¼����</typeparam>
        /// <returns>��־����</returns>
        ILogger CreateLogger<T>();

        /// <summary>
        /// ����һ����־��¼��
        /// </summary>
        /// <param name="type">��־��������</param>
        /// <param name="loggingPath">�����ļ�����־��¼·��</param>
        /// <returns>��־����</returns>
        ILogger CreateLogger(Type type, string loggingPath);

        /// <summary>
        /// ����һ����־��¼��
        /// </summary>
        /// <param name="type">��־��������</param>
        /// <returns>��־����</returns>
        ILogger CreateLogger(Type type);
    }

    #region ILogger
    /// <summary>
    /// ��־��¼�߽ӿ�
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// ��¼���Լ������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        void Debug(string message);

        /// <summary>
        /// ��¼���Լ������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        void Debug(string message, Exception ex);

        /// <summary>
        /// ��¼��Ϣ�������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        void Info(string message);

        /// <summary>
        /// ��¼��Ϣ�������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        void Info(string message, Exception ex);

        /// <summary>
        /// ��¼���漶�����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        void Warn(string message);

        /// <summary>
        /// ��¼���漶�����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        void Warn(string message, Exception ex);

        /// <summary>
        /// ��¼���󼶱����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        void Error(string message);

        /// <summary>
        /// ��¼���󼶱����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        void Error(string message, Exception ex);

        /// <summary>
        /// ��¼�������󼶱����־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        void Fatal(string message);

        /// <summary>
        /// ��¼��������L�������־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="ex">�쳣</param>
        void Fatal(string message, Exception ex);
    }
    #endregion
}
