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
        private Dictionary<string, AuthorizationNode> authorizations = new Dictionary<string, AuthorizationNode>();
        private Dictionary<string, AuthorizationAction> actions = new Dictionary<string, AuthorizationAction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationStore"/> class.
        /// </summary>
        public AuthorizationStore()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationStore"/> class.
        /// </summary>
        /// <param name="role">The role.</param>
        public AuthorizationStore(string role)
            : this()
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
        /// 添加授权节点信息
        /// </summary>
        /// <param name="node">授权节点</param>
        public void Add(AuthorizationNode node)
        {
            Guard.ArgumentNotNull(node, "Authorization node");
            Guard.ArgumentNotNull(node.AuthorizationUri, "Authorization uri");

            authorizations[node.AuthorizationUri] = node;
            actions[node.AuthorizationUri] = AuthorizationAction.Deny;
        }

        /// <summary>
        /// 添加授权节点信息
        /// </summary>
        /// <param name="node">授权节点</param>
        /// <param name="action">授权动作</param>
        public void Add(AuthorizationNode node, AuthorizationAction action)
        {
            Add(node);
            actions[node.AuthorizationUri] = action;
        }

        /// <summary>
        /// 删除授权节点信息
        /// </summary>
        /// <param name="authorizationUri">授权路径</param>
        public void Remove(string authorizationUri)
        {
            if (authorizations.ContainsKey(authorizationUri))
                authorizations.Remove(authorizationUri);
            if (actions.ContainsKey(authorizationUri))
                actions.Remove(authorizationUri);
        }

        /// <summary>
        /// 删除授权节点信息
        /// </summary>
        /// <param name="node">授权节点</param>
        public void Remove(AuthorizationNode node)
        {
            if (node != null && !String.IsNullOrEmpty(node.AuthorizationUri))
                Remove(node.AuthorizationUri);
        }

        /// <summary>
        /// 检查当前角色是否可以执行特定路径下的命令
        /// </summary>
        /// <param name="authorizationUri">授权路径</param>
        /// <returns>
        /// 	如果当前角色拥有相关权限返回<c>true</c>；否则返回<c>false</c>.
        /// </returns>
        public bool CanExecute(string authorizationUri)
        {
            bool result = false;
            if (actions.ContainsKey(authorizationUri))
                result = actions[authorizationUri] == AuthorizationAction.Allow;
            return result;
        }
    }
}
