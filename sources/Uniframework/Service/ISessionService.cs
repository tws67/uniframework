using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    #region ServerVariables
    /// <summary>
    /// ��������������
    /// </summary>
    public sealed class ServerVariables
    {
        /// <summary>
        /// �ỰId��ʶ
        /// </summary>
        public const string SESSION_ID = "Uniframework.SessionService.SessionID";
        /// <summary>
        /// �����¼���ʶ
        /// </summary>
        public const string LOGGING_TIME = "Uniframework.SessionService.LoggingTime";
        /// <summary>
        /// ��ǰ�û���ʶ
        /// </summary>
        public const string CURRENT_USER = "Uniframework.SessionService.CurrentUser";
        /// <summary>
        /// �ͻ���IP��ʶ
        /// </summary>
        public const string CLIENT_ADDRESS = "Uniframework.SessionService.ClientAddress";
        /// <summary>
        /// �Ự��Կ��ʶ
        /// </summary>
        public const string ENCRYPT_KEY = "Uniframework.SessionService.EncryptKey";
    }
    #endregion
    
    /// <summary>
    /// ϵͳ�Ự��ϵͳ�ӿ�
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// ע��Ự
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="user">The user name.</param>
        /// <param name="ipAddress">���÷�IP��ַ</param>
        /// <param name="encryptKey">��Կ</param>
        void Register(string sessionId, string user, string ipAddress, string encryptKey);

        /// <summary>
        /// ע���Ự
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        void UnloadSession(string sessionId);

        /// <summary>
        /// ���ݻỰ��ʶ��ȡ�Ựʵ��
        /// </summary>
        /// <param name="sessionID">�Ự��ʶ</param>
        /// <returns>�Ựʵ��</returns>
        ISessionState GetSession(string sessionId);

        /// <summary>
        /// ����Ự
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        void Activate(string sessionId);

        /// <summary>
        /// ��ǰ�Ựʵ��
        /// </summary>
        ISessionState CurrentSession { get; }

        /// <summary>
        /// ��ȡ���еĻỰʵ��
        /// </summary>
        /// <returns></returns>
        ISessionState[] GetAllSessions();
    }
}
