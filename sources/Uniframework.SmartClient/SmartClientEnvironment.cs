using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Uniframework 客户端执行环境
    /// </summary>
    [Service]
    public class SmartClientEnvironment
    {
        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public string ApplicationPath {
            get {
                return FileUtility.ApplicationRootPath;
            }
        }
    }
}
