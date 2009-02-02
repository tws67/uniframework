using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 授权信息存储类
    /// </summary>
    [Serializable]
    public class AuthorizationStore
    {
        private string role;
        private Dictionary<string, AuthorizationAction> authorizations = new Dictionary<string, AuthorizationAction>();

        public AuthorizationStore(string role)
        {
            this.role = role;
        }

        /// <summary>
        /// 授权的角色
        /// </summary>
        /// <value>角色名称</value>
        public string Role 
        { 
            get { 
                return role; 
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <value>返回指定资源是否可用</value>
        public AuthorizationAction this[string index]
        {
            get {
                return authorizations.ContainsKey(index) ? authorizations[index] : AuthorizationAction.Deny;
            }
            set { authorizations[index] = value; }
        }
    }
}
