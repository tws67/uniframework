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
        public const string SESSION_ID = "Framework.SessionService.SessionID";
        /// <summary>
        /// 登入事件标识
        /// </summary>
        public const string LOGGING_TIME = "Framework.SessionService.LoggingTime";
        /// <summary>
        /// 当前用户标识
        /// </summary>
        public const string CURRENT_USER = "Framework.SessionService.CurrentUser";
        /// <summary>
        /// 客户端IP标识
        /// </summary>
        public const string CLIENT_ADDRESS = "Framework.SessionService.ClientAddress";
        /// <summary>
        /// 会话密钥标识
        /// </summary>
        public const string ENCRYPT_KEY = "Framework.SessionService.EncryptKey";
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
        /// <param name="sessionID">会话标识</param>
        /// <param name="userName">用户名称</param>
        /// <param name="ipAddress">调用方IP地址</param>
        /// <param name="encryptKey">密钥</param>
        void Register(string sessionID, string userName, string ipAddress, string encryptKey);

        /// <summary>
        /// 注销会话
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        void UnloadSession(string sessionID);

        /// <summary>
        /// 根据会话标识获取会话实例
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        /// <returns>会话实例</returns>
        ISessionState GetSession(string sessionID);

        /// <summary>
        /// 激活会话
        /// </summary>
        /// <param name="sessionID">会话标识</param>
        void Activate(string sessionID);

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
