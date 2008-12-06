using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    #region Session Variables
    
    /// <summary>
    /// �Ự����
    /// </summary>
    public sealed class SessionVariables
    {
        public const string SESSION_ID             = "uf_session_id"; // �Ự��ʶ
        public const string SESSION_LOGIN_TIME     = "uf_session_login_time"; // ��¼ʱ��
        public const string SESSION_CURRENT_USER   = "uf_session_current_user"; // �����Ự���û�
        public const string SESSION_CLIENT_ADDRESS = "uf_session_client_address"; // �ͻ���ip��ַ
        public const string SESSION_ENCRYPTKEY     = "uf_session_encryptkey"; // �Ự��ʹ�õ���Կ
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
        /// <param name="sessionId">�Ự��ʶ</param>
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
