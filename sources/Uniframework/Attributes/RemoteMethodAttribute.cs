using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 方法属性标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    [Serializable]
    public class RemoteMethodAttribute : Attribute
    {
        private string description;
        private bool offline;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodAttribute"/> class.
        /// </summary>
        public RemoteMethodAttribute()
        {
            description = string.Empty;
            offline = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodAttribute"/> class.
        /// </summary>
        /// <param name="offline">if set to <c>true</c> [offline].</param>
        public RemoteMethodAttribute(bool offline)
        {
            this.offline = offline;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public RemoteMethodAttribute(string description) 
            : this(false)
        {
            this.description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMethodAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="offline">if set to <c>true</c> [offline].</param>
        public RemoteMethodAttribute(string description, bool offline)
        {
            this.description = description;
            this.offline = offline;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return description;
            }
            //set { description = value; }
        }

        /// <summary>
        /// 离线处理指示器
        /// </summary>
        public bool Offline
        {
            get
            {
                return offline;
            }
        }
    }
}
