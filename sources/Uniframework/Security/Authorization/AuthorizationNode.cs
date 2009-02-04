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
        protected string authorizationUri = String.Empty;
        private string id = String.Empty;
        private string name = String.Empty;
        private Dictionary<string, Dictionary<string, AuthorizationCommand>> commands = new Dictionary<string, Dictionary<string, AuthorizationCommand>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationNode"/> class.
        /// </summary>
        public AuthorizationNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationNode"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public AuthorizationNode(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationNode"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        public AuthorizationNode(string id, string name)
            : this(id)
        {
            this.name = name;
        }

        #region IAuthorizationNode Members

        /// <summary>
        /// 授权节点路径，此路径与AuthorizationAttribute属性中的路径一致，由从根节点到当前节点的所有标识组成节点的路径
        /// </summary>
        /// <value>授权节点路径</value>
        public string AuthorizationUri
        {
            get
            {
                return GetAuthorizationUri();
            }
            set
            {
                authorizationUri = value;
            }
        }

        /// <summary>
        /// 获取节点路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAuthorizationUri()
        {
            return authorizationUri;
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
                return String.IsNullOrEmpty(name) ? id : name;
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
        public Dictionary<string, Dictionary<string, AuthorizationCommand>> Commands
        {
            get { return commands; }
        }

        #endregion

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="command">The command.</param>
        public void AddCommand(string category, AuthorizationCommand command)
        {
            Guard.ArgumentNotNull(command, "Authorization command");

            if (!commands.ContainsKey(category))
                commands[category] = new Dictionary<string, AuthorizationCommand>();
            commands[category][command.CommandUri] = command;
        }

        /// <summary>
        /// Removes the command.
        /// </summary>
        /// <param name="cagegory">The cagegory.</param>
        /// <param name="command">The command.</param>
        public void RemoveCommand(string cagegory, AuthorizationCommand command)
        {
            Guard.ArgumentNotNull(command, "Authorization command");

            if (commands.ContainsKey(cagegory) && commands[cagegory].ContainsKey(command.CommandUri))
                commands[cagegory].Remove(command.CommandUri);
        }

        /// <summary>
        /// Clears the command.
        /// </summary>
        /// <param name="category">The category.</param>
        public void ClearCommand(string category)
        {
            Guard.ArgumentNotNull(category, "category");

            if (commands.ContainsKey(category))
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

    /// <summary>
    /// 授权命令
    /// </summary>
    [Serializable]
    public class AuthorizationCommand
    {
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        public string Category { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the command URI.
        /// </summary>
        /// <value>The command URI.</value>
        public string CommandUri { get; set; }
        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        /// <value>The image file.</value>
        public string ImageFile { get; set; }
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public AuthorizationAction Action { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
