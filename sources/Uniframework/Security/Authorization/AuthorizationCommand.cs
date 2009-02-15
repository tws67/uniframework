using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Db4objects.Db4o.Config.Attributes;

namespace Uniframework.Security
{
    /// <summary>
    ///  授权命令
    /// </summary>
    [Serializable]
    public class AuthorizationCommand
    {
        [Indexed]
        private string commandUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCommand"/> class.
        /// </summary>
        public AuthorizationCommand() { 
        }

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
        public string CommandUri { get { return commandUri; } set { commandUri = value; } }
        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        /// <value>The image file.</value>
        public string Image { get; set; }
        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>The sequence.</value>
        public int Sequence {
            get;
            set;
        }
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
