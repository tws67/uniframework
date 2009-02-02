using System;
using System.Collections.Generic;
using System.Text;
using Uniframework.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// 基于角色的授权信息
    /// </summary>
    [Serializable]
    public class AuthorizationResource
    {
        private string role;
        private Dictionary<string, AuthorizationAction> authorizations = new Dictionary<string, AuthorizationAction>();

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Role
        {
            get { return role; }
            set { role = value; }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public AuthorizationAction this[string index]
        {
            get {
                return authorizations.ContainsKey(index) ? authorizations[index] : AuthorizationAction.Deny;
            }
            set {
                authorizations[index] = value;
            }
        }
    }
}
