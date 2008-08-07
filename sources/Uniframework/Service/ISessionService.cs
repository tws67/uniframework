using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    #region ServerVariables
    /// <summary>
    /// 服务器变量定义
    /// </summary>
    public sealed class ServerVariables
    {
        /// <summary>
        /// 会话Id标识
        /// </summary>
        public const string SESSION_ID = "Uniframework.SessionService.SessionID";
        /// <summary>
        /// 登入事件标识
        /// </summary>
        public const string LOGGING_TIME = "Uniframework.SessionService.LoggingTime";
        /// <summary>
        /// 当前用户标识
        /// </summary>
        public const string CURRENT_USER = "Uniframework.SessionService.CurrentUser";
        /// <summary>
        /// 客户端IP标识
        /// </summary>
        public const string CLIENT_ADDRESS = "Uniframework.SessionService.ClientAddress";
        /// <summary>
        /// 会话密钥标识
        /// </summary>
        public const string ENCRYPT_KEY = "Uniframework.SessionService.EncryptKey";
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
        /// <param name="sessionID">会话标识</param>
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
