using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// Web网站，定义一个IIS Web网站
    /// </summary>
    public class IISWebServer
    {
        /// <summary>
        /// 
        /// </summary>
        internal int index = -1;
        /// <summary>
        /// 
        /// </summary>
        public IISWebVirtualDirCollection WebVirtualDirs;
        /// <summary>
        /// 网站说明
        /// </summary>
        public string ServerComment = "Web site created by Uniframework!";
        /// <summary>
        /// 脚本支持
        /// </summary>
        public bool AccessScript = true;
        /// <summary>
        /// 读取
        /// </summary>
        public bool AccessRead = true;
        /// <summary>
        /// 物理路径
        /// </summary>
        public string Path = "C:\\Inetpub\\wwwroot\\Website1";
        /// <summary>
        /// 端口
        /// </summary>
        public int Port = 80;
        /// <summary>
        /// 目录浏览
        /// </summary>
        public bool EnableDirBrowsing = false;
        /// <summary>
        /// 默认文档
        /// </summary>
        public string DefaultDoc = "index.aspx, default.aspx";
        /// <summary>
        /// 使用默认文档
        /// </summary>
        public bool EnableDefaultDoc = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="IISWebServer"/> class.
        /// </summary>
        public IISWebServer()
        {
            WebVirtualDirs = new IISWebVirtualDirCollection(this);
        }

        /// <summary>
        /// IISWebServer的状态
        /// </summary>
        public IISServerState ServerState
        {
            get
            {
                DirectoryEntry server = IISManagement.GetIISWebserver(this.index);
                if (server == null)
                    throw (new Exception("找不到此IISWebServer"));

                switch (server.Properties["ServerState"][0].ToString())
                {
                    case "2":
                        return IISServerState.Started;
                    case "4":
                        return IISServerState.Stopped;
                    case "6":
                        return IISServerState.Paused;
                }
                return IISServerState.Stopped;
            }
        }

        /// <summary>
        /// 停止Web网站
        /// </summary>
        public void Stop()
        {
            DirectoryEntry Server;
            if (index == -1)
                throw (new Exception("在IIS找不到此IISWebServer!"));
            try {
                Server = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC/" + index);
                if (Server != null)
                    Server.Invoke("stop", new object[0]);
                else
                    throw (new Exception("在IIS找不到此IISWebServer!"));
            }
            catch {
                throw (new Exception("在IIS找不到此IISWebServer!"));
            }
        }

        /// <summary>
        /// 把基本信息的更改更新到IIS
        /// </summary>
        public void CommitChanges()
        {
            IISManagement.EditIISWebServer(this);
        }

        /// <summary>
        /// 启动IISWebServer
        /// </summary>
        public void Start()
        {
            if (index == -1)
                throw (new Exception("在IIS找不到此IISWebServer!"));

            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server;
            IEnumerator ie = Service.Children.GetEnumerator();

            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    if (Server.Properties["Serverbindings"][0].ToString() == ":" + this.Port + ":")
                    {
                        Server.Invoke("stop", new object[0]);
                    }
                }
            }

            try {
                Server = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC/" + index);
                if (Server != null)
                    Server.Invoke("start", new object[0]);
                else
                    throw (new Exception("在IIS找不到此IISWebServer!"));
            }
            catch {
                throw (new Exception("在IIS找不到此IISWebServer!"));
            }
        }

        public override string ToString()
        {
            return ServerComment + " at port \"" + Port + "\" on path \"" + Path + "\" state is " + ServerState; 
        }

    }
}
