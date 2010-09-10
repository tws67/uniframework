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
        //private Dictionary<string, AuthorizationNode> authorizations;
        private List<AuthorizationNode> authorizations;
        //private Dictionary<string, AuthorizationAction> actions;
        private List<AuthorizationActionInfo> actions;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationStore"/> class.
        /// </summary>
        public AuthorizationStore()
        {
            authorizations = new List<AuthorizationNode>();
            actions = new List<AuthorizationActionInfo>();
        }

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
        /// 返回当前角色的权限节点列表
        /// </summary>
        /// <value>权限节点列表</value>
        public IEnumerable<AuthorizationNode> Nodes
        {
            get {
                return authorizations;
            }
        }

        /// <summary>
        /// 为特定的权限路径进行授权
        /// </summary>
        /// <param name="authorizationUri">权限路径</param>
        /// <param name="action">授权动作<see cref="AuthorizationAction"/></param>
        public void Authorization(string authorizationUri, AuthorizationAction action)
        {
            if(!actions.Contains(new AuthorizationActionInfo(authorizationUri, action)))
                actions.Add(new AuthorizationActionInfo(authorizationUri, action));
        }

        /// <summary>
        /// 删除授权节点信息
        /// </summary>
        /// <param name="authorizationUri">授权路径</param>
        public void Remove(string authorizationUri)
        {
            Guard.ArgumentNotNull(authorizationUri, "Authorization Uri");

            //if (authorizations.ContainsKey(authorizationUri)) {
            //    AuthorizationNode node = authorizations[authorizationUri];
            //    // 选择节点下的所有操作项的授权信息
            //    foreach (AuthorizationCommand cmd in node.Commands) {
            //        string key = SecurityUtility.HashObject(node.AuthorizationUri + cmd.CommandUri);
            //        if (actions.ContainsKey(key))
            //            actions.Remove(key);
            //    }
            //    authorizations.Remove(authorizationUri);
            //}

            foreach (AuthorizationNode node in authorizations) {
                if (node.AuthorizationUri == authorizationUri) {
                    foreach (AuthorizationCommand cmd in node.Commands) {
                        string key = SecurityUtility.HashObject(node.AuthorizationUri + cmd.CommandUri);
                        AuthorizationActionInfo actionInfo = new AuthorizationActionInfo(key, cmd.Action);
                        if (actions.Contains(actionInfo))
                            actions.Remove(actionInfo);
                    }
                }
            }
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
        /// 存储授权节点
        /// </summary>
        /// <param name="node">授权节点</param>
        public void Store(AuthorizationNode node)
        {
            Guard.ArgumentNotNull(node, "node");

            Dictionary<string, AuthorizationAction> acs = new Dictionary<string, AuthorizationAction>();
            //// 原来的授权信息
            //foreach (AuthorizationCommand cmd in node.Commands) {
            //    string key = SecurityUtility.HashObject(node.AuthorizationUri + cmd.CommandUri);
            //    if (actions.ContainsKey(key))
            //        acs[key] = actions[key];
            //}

            //Remove(node.AuthorizationUri); // 删除原来的授权信息
            //authorizations[node.AuthorizationUri] = node;
            //foreach (AuthorizationCommand cmd in node.Commands) {
            //    string key = SecurityUtility.HashObject(node.AuthorizationUri + cmd.CommandUri);
            //    if (acs.ContainsKey(key))
            //        actions[key] = acs[key];
            //    else
            //        actions[key] = AuthorizationAction.Deny;
            //}

            // 首先删除原来的授权信息
            foreach (AuthorizationCommand cmd in node.Commands) {
                string key = SecurityUtility.HashObject(node.AuthorizationUri + cmd.CommandUri);
                AuthorizationActionInfo actionInfo = new AuthorizationActionInfo(key, cmd.Action);
                if (actions.Contains(actionInfo))
                    actions.Remove(actionInfo);
            }
            Remove(node.AuthorizationUri);
            authorizations.Add(node);
            foreach (AuthorizationCommand cmd in node.Commands) {
                string key = SecurityUtility.HashObject(node.AuthorizationUri + cmd.CommandUri);
                AuthorizationActionInfo actionInfo = new AuthorizationActionInfo(key, cmd.Action);
                actions.Add(actionInfo); 
            }
        }

        /// <summary>
        /// 检查当前角色是否可以执行特定路径下的命令
        /// </summary>
        /// <param name="authorizationUri">授权路径</param>
        /// <returns>
        /// 	如果当前角色拥有相关权限返回<c>true</c>；否则返回<c>false</c>.
        /// </returns>
        /// <remarks>在处理角色的权限时如果没有将相关的授权路径放入Store中则直接返回<c>true</c></remarks>
        public bool CanExecute(string authorizationUri)
        {
            bool result = true;
            //if (actions.ContainsKey(authorizationUri))
            //    result = actions[authorizationUri] == AuthorizationAction.Allow;

            return result;
        }
    }

    [Serializable]
    public class AuthorizationActionInfo
    {
        public AuthorizationActionInfo(string authorizationUri, AuthorizationAction action)
        {
            AuthorizationUri = authorizationUri;
            Action = action;
        }

        public string AuthorizationUri { get; set;    }

        public AuthorizationAction Action { get; set; }
    }
}
