using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Db4objects.Db4o.Config.Attributes;

namespace Uniframework.Services
{
    /// <summary>
    /// 数据表格、列表的Layout数据用于存储或恢复用户对表格、列表的布局信息
    /// </summary>
    [Serializable]
    public class Layout
    {
        [Indexed]
        private string user;
        [Indexed]
        private string module;
        [Indexed]
        private string appUri;
        private string version;
        private DateTime updatedOn = DateTime.Now;
        private byte[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListLayout"/> class.
        /// </summary>
        public Layout() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListLayout"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="module">The module.</param>
        /// <param name="appUri">The app URI.</param>
        public Layout(string user, string module, string appUri)
            : this()
        {
            this.user = user;
            this.module = module;
            this.appUri = appUri;
        }

        #region Members

        public string Module
        {
            get { return module; }
            set { module = value; }
        }

        public string AppUri
        {
            get { return appUri; }
            set { appUri = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public DateTime UpdatedOn
        {
            get { return updatedOn; }
            set { updatedOn = value; }
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion
    }
}
