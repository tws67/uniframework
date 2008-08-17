　using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.CompositeUI.Services
{
    /// <summary>
    /// 系统权限管理服务
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// 检查指定路径下的命令可否执行
        /// </summary>
        /// <param name="command">命令名</param>
        /// <returns>如果可以执行返回true，否则为false</returns>
        bool CanExecute(string command);
    }

    /// <summary>
    /// 系统权限管理服务异常
    /// </summary>
    public class AuthorizationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthorizationException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public AuthorizationException(string message)
            : base(message)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public AuthorizationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
