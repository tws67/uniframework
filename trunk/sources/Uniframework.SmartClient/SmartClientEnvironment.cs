using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Uniframework 客户端执行环境
    /// </summary>
    [Service]
    public class SmartClientEnvironment
    {
        private UserInfo currentUser;

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public string ApplicationPath {
            get {
                return FileUtility.GetParent(FileUtility.ApplicationRootPath);
            }
        }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        /// <value>The current user.</value>
        public UserInfo CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value;}
        }
    }
}
