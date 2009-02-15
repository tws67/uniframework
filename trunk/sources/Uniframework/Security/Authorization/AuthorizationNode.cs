using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 授权节点类
    /// </summary>
    [Serializable]
    public class AuthorizationNode
    {
        private string authorizationUri;
        private string id;
        private string name;
        private List<AuthorizationCommand> commands = new List<AuthorizationCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationNode"/> class.
        /// </summary>
        public AuthorizationNode() { }

        #region IAuthorizationNode Members

        /// <summary>
        /// 授权节点路径，此路径与AuthorizationAttribute属性中的路径一致，由从根节点到当前节点的所有标识组成节点的路径
        /// </summary>
        /// <value>授权节点路径</value>
        public string AuthorizationUri
        {
            get
            {
                return authorizationUri;
            }
            set
            {
                authorizationUri = value;
            }
        }

        /// <summary>
        /// 授权节点标识
        /// </summary>
        /// <value>节点标识</value>
        public string Id
        {
            get
            {
                // 为授权节点标识添加分隔符
                return id.StartsWith(GlobalConstants.Uri_Separator) ? id : GlobalConstants.Uri_Separator + id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// 授权节点名称.
        /// </summary>
        /// <value>节点名称</value>
        public string Name
        {
            get
            {
                return name; // String.IsNullOrEmpty(name) ? id : name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public IList<AuthorizationCommand> Commands
        {
            get { return commands; }
        }

        #endregion

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void AddCommand(AuthorizationCommand command)
        {
            Guard.ArgumentNotNull(command, "Authorization command");

            if (!commands.Contains(command))
                commands.Add(command);
        }

        /// <summary>
        /// Removes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void RemoveCommand(AuthorizationCommand command)
        {
            Guard.ArgumentNotNull(command, "Authorization command");

            if (commands.Contains(command))
                commands.Remove(command);
        }

        /// <summary>
        /// Clears the command.
        /// </summary>
        public void ClearCommand()
        {
            commands.Clear();
        }
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return name;
        }
    }
}
