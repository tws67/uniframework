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
        public const string SESSION_ID = "Framework.SessionService.SessionID";
        /// <summary>
        /// �����¼���ʶ
        /// </summary>
        public const string LOGGING_TIME = "Framework.SessionService.LoggingTime";
        /// <summary>
        /// ��ǰ�û���ʶ
        /// </summary>
        public const string CURRENT_USER = "Framework.SessionService.CurrentUser";
        /// <summary>
        /// �ͻ���IP��ʶ
        /// </summary>
        public const string CLIENT_ADDRESS = "Framework.SessionService.ClientAddress";
        /// <summary>
        /// �Ự��Կ��ʶ
        /// </summary>
        public const string ENCRYPT_KEY = "Framework.SessionService.EncryptKey";
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
        /// <param name="sessionID">�Ự��ʶ</param>
        /// <param name="userName">�û�����</param>
        /// <param name="ipAddress">���÷�IP��ַ</param>
        /// <param name="encryptKey">��Կ</param>
        void Register(string sessionID, string userName, string ipAddress, string encryptKey);

        /// <summary>
        /// ע���Ự
        /// </summary>
        /// <param name="sessionID">�Ự��ʶ</param>
        void UnloadSession(string sessionID);

        /// <summary>
        /// ���ݻỰ��ʶ��ȡ�Ựʵ��
        /// </summary>
        /// <param name="sessionID">�Ự��ʶ</param>
        /// <returns>�Ựʵ��</returns>
        ISessionState GetSession(string sessionID);

        /// <summary>
        /// ����Ự
        /// </summary>
        /// <param name="sessionID">�Ự��ʶ</param>
        void Activate(string sessionID);

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
