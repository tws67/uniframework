using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 框架授权权限资源属性
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple=true)]
    public class AuthorizationAttribute : Attribute
    {
        private string authorizationUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationAttribute"/> class.
        /// </summary>
        /// <param name="authorizationUri">The authorization URI.</param>
        public AuthorizationAttribute(string authorizationUri)
        {
            this.authorizationUri = authorizationUri;
        }

        /// <summary>
        /// Gets the authorization URI.
        /// </summary>
        /// <value>The authorization URI.</value>
        public string AuthorizationUri
        {
            get { return authorizationUri; }
        }
    }
}
