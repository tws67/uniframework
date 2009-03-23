using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 虚拟目录
    /// </summary>
    public class IISWebVirtualDir
    {
        public IISWebServer Parent = null;
        /// <summary>
        /// 虚拟目录名称
        /// </summary>
        public string Name = "Virtual directory created by Uniframework!";
        /// <summary>
        /// 读取
        /// </summary>
        public bool AccessRead = true;
        /// <summary>
        /// 脚本支持
        /// </summary>
        public bool AccessScript = true;
        /// <summary>
        /// 物理路径
        /// </summary>
        public string Path = "c:\\";
        /// <summary>
        /// 默认文档
        /// </summary>
        public string DefaultDoc = "index.aspx, default.aspx";
        /// <summary>
        /// 使用默认文档
        /// </summary>
        public bool EnableDefaultDoc = true;
        /// <summary>
        /// 所属的网站的网站说明
        /// </summary>
        public string WebServer = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="IISWebVirtualDir"/> class.
        /// </summary>
        public IISWebVirtualDir()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IISWebVirtualDir"/> class.
        /// </summary>
        /// <param name="WebServerName">Name of the web server.</param>
        public IISWebVirtualDir(string WebServerName)
            : this()
        {
            if (WebServerName.ToString() == "")
                throw (new Exception("WebServerName不能为空!"));
            this.WebServer = WebServerName;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
