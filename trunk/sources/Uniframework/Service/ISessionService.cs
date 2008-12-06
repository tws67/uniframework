using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    #region Session Variables
    
    /// <summary>
    /// 会话变量
    /// </summary>
    public sealed class SessionVariables
    {
        public const string SESSION_ID             = "uf_session_id"; // 会话标识
        public const string SESSION_LOGIN_TIME     = "uf_session_login_time"; // 登录时间
        public const string SESSION_CURRENT_USER   = "uf_session_current_user"; // 建立会话的用户
        public const string SESSION_CLIENT_ADDRESS = "uf_session_client_address"; // 客户端ip地址
        public const string SESSION_ENCRYPTKEY     = "uf_session_encryptkey"; // 会话所使用的密钥
    }

    #endregion
    
    /// <summary>
    /// 系统会话子系统接口
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// 注册会话
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <param name="user">The user name.</param>
        /// <param name="ipAddress">调用方IP地址</param>
        /// <param name="encryptKey">密钥</param>
        void Register(string sessionId, string user, string ipAddress, string encryptKey);

        /// <summary>
        /// 注销会话
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        void UnloadSession(string sessionId);

        /// <summary>
        /// 根据会话标识获取会话实例
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        /// <returns>会话实例</returns>
        ISessionState GetSession(string sessionId);

        /// <summary>
        /// 激活会话
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        void Activate(string sessionId);

        /// <summary>
        /// 当前会话实例
        /// </summary>
        ISessionState CurrentSession { get; }

        /// <summary>
        /// 获取所有的会话实例
        /// </summary>
        /// <returns></returns>
        ISessionState[] GetAllSessions();
    }
}
