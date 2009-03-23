using System;
using System.DirectoryServices;
using System.Collections;

namespace Uniframework.Services
{
    /// <summary>
    /// IISWebServer的状态
    /// </summary>
    public enum IISServerState
    {
        Starting = 1,
        Started = 2,
        Stopping = 3,
        Stopped = 4,
        Pausing = 5,
        Paused = 6,
        Continuing = 7
    }

    /// <summary>
    /// IIS管理器
    /// </summary>
    public class IISManagement
    {
        /// <summary>
        /// 
        /// </summary>
        public IISWebServerCollection WebServers = new IISWebServerCollection();
        
        internal static string Machinename = "localhost";

        /// <summary>
        /// Initializes a new instance of the <see cref="IISManagement"/> class.
        /// </summary>
        public IISManagement()
        {
            Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IISManagement"/> class.
        /// </summary>
        /// <param name="MachineName">机器名,默认值为localhost</param>
        public IISManagement(string MachineName)
        {
            if (!String.IsNullOrEmpty(Machinename))
                Machinename = MachineName;
            Start();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + Machinename + "/W3SVC");
            DirectoryEntry Server;
            DirectoryEntry Root = null;
            DirectoryEntry VirDir;
            IEnumerator ie = Service.Children.GetEnumerator();
            IEnumerator ieRoot;
            IISWebServer item;
            IISWebVirtualDir item_virdir;
            bool finded = false;

            // 枚举所有Web网站
            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    item = new IISWebServer();
                    item.index = Convert.ToInt32(Server.Name);

                    item.ServerComment = (string)Server.Properties["ServerComment"][0];
                    item.AccessRead = (bool)Server.Properties["AccessRead"][0];
                    item.AccessScript = (bool)Server.Properties["AccessScript"][0];
                    item.DefaultDoc = (string)Server.Properties["DefaultDoc"][0];
                    item.EnableDefaultDoc = (bool)Server.Properties["EnableDefaultDoc"][0];
                    item.EnableDirBrowsing = (bool)Server.Properties["EnableDirBrowsing"][0];
                    
                    // 确认其是否是站点
                    ieRoot = Server.Children.GetEnumerator();
                    while (ieRoot.MoveNext()) {
                        Root = (DirectoryEntry)ieRoot.Current;
                        if (Root.SchemaClassName == "IIsWebVirtualDir") {
                            finded = true;
                            break;
                        }
                    }

                    if (finded)
                        item.Path = Root.Properties["path"][0].ToString(); // 获取站点路径

                    string[] bindings = Server.Properties["ServerBindings"][0].ToString().Split(':');
                    item.Port = Convert.ToInt32(bindings[1]); // 获取站点的端口
                    this.WebServers.AddWebServer(item);

                    // 枚举网点下的虚拟目录
                    ieRoot = Root.Children.GetEnumerator();
                    while (ieRoot.MoveNext()) {
                        VirDir = (DirectoryEntry)ieRoot.Current;
                        if (VirDir.SchemaClassName != "IIsWebVirtualDir" && VirDir.SchemaClassName != "IIsWebDirectory")
                            continue;

                        item_virdir = new IISWebVirtualDir(item.ServerComment);
                        item_virdir.Name = VirDir.Name;
                        item_virdir.AccessRead = (bool)VirDir.Properties["AccessRead"][0];
                        item_virdir.AccessScript = (bool)VirDir.Properties["AccessScript"][0];
                        item_virdir.DefaultDoc = (string)VirDir.Properties["DefaultDoc"][0];
                        item_virdir.EnableDefaultDoc = (bool)VirDir.Properties["EnableDefaultDoc"][0];

                        switch (VirDir.SchemaClassName) { 
                            case "IIsWebVirtualDir" :
                                item_virdir.Path = (string)VirDir.Properties["Path"][0];
                                break;

                            case "IIsWebDirectory" :
                                item_virdir.Path = Root.Properties["Path"][0] + "\\" + VirDir.Name;
                                break;
                        }
                        item.WebVirtualDirs.Add_(item_virdir);
                    }
                }
            }
        }

        /// <summary>
        /// 创建站点
        /// </summary>
        /// <param name="iisServer">The IIS server.</param>
        public static void CreateIISWebServer(IISWebServer iisServer)
        {
            if (iisServer.ServerComment.ToString() == "")
                throw (new Exception("IISWebServer的ServerComment不能为空!"));
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server;
            int i = 0;
            IEnumerator ie = Service.Children.GetEnumerator();

            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    if (Convert.ToInt32(Server.Name) > i)
                        i = Convert.ToInt32(Server.Name);
                    //     if( Server.Properties["Serverbindings"][0].ToString() == ":" + iisServer.Port + ":" ) 
                    //     {
                    //      Server.Invoke("stop",new object[0]);
                    //     }
                }
            }

            i++;

            try {
                iisServer.index = i;
                Server = Service.Children.Add(i.ToString(), "IIsWebServer");
                Server.Properties["ServerComment"][0] = iisServer.ServerComment;
                Server.Properties["Serverbindings"].Add(":" + iisServer.Port + ":");
                Server.Properties["AccessScript"][0] = iisServer.AccessScript;
                Server.Properties["AccessRead"][0] = iisServer.AccessRead;
                Server.Properties["EnableDirBrowsing"][0] = iisServer.EnableDirBrowsing;
                Server.Properties["DefaultDoc"][0] = iisServer.DefaultDoc;
                Server.Properties["EnableDefaultDoc"][0] = iisServer.EnableDefaultDoc;

                DirectoryEntry root = Server.Children.Add("Root", "IIsWebVirtualDir");
                root.Properties["path"][0] = iisServer.Path;

                Service.CommitChanges();
                Server.CommitChanges();
                root.CommitChanges();
                root.Invoke("AppCreate2", new object[1] { 2 });
                //Server.Invoke("start",new object[0]);
            }
            catch (Exception ex) {
                throw (ex);
            }
        }

        /// <summary>
        /// 删除IISWebServer
        /// </summary>
        /// <param name="ServerComment">The server comment.</param>
        public static void RemoveIISWebServer(string ServerComment)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server;
            IEnumerator ie = Service.Children.GetEnumerator();

            ServerComment = ServerComment.ToLower();
            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    if (Server.Properties["ServerComment"][0].ToString().ToLower() == ServerComment) {
                        Service.Children.Remove(Server);
                        Service.CommitChanges();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 删除IISWebServer
        /// </summary>
        /// <param name="index">The index.</param>
        public static void RemoveIISWebServer(int index)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");

            try {
                DirectoryEntry Server = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC/" + index);
                if (Server != null) {
                    Service.Children.Remove(Server);
                    Service.CommitChanges();
                }
                else {
                    throw (new Exception("找不到此IISWebServer"));
                }
            }
            catch {
                throw (new Exception("找不到此IISWebServer"));
            }
        }

        /// <summary>
        /// 检查是否存在IISWebServer
        /// </summary>
        /// <param name="ServerComment">网站说明</param>
        /// <returns></returns>
        public static bool ExistsIISWebServer(string ServerComment)
        {
            ServerComment = ServerComment.Trim();
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server = null;
            IEnumerator ie = Service.Children.GetEnumerator();

            string comment;
            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    comment = Server.Properties["ServerComment"][0].ToString().ToLower().Trim();
                    if (comment == ServerComment.ToLower()) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 返回指定的IISWebServer
        /// </summary>
        /// <param name="ServerComment">The server comment.</param>
        /// <returns></returns>
        internal static DirectoryEntry GetIISWebserver(string ServerComment)
        {
            ServerComment = ServerComment.Trim();
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server = null;
            IEnumerator ie = Service.Children.GetEnumerator();

            string comment;
            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    comment = Server.Properties["ServerComment"][0].ToString().ToLower().Trim();
                    if (comment == ServerComment.ToLower()) {
                        return Server;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// 返回指定的IISWebServer
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal static DirectoryEntry GetIISWebserver(int index)
        {
            DirectoryEntry Server = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC/" + index);
            try {
                IEnumerator ie = Server.Children.GetEnumerator();
                return Server;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <returns></returns>
        private static DirectoryEntry GetRoot(DirectoryEntry Server)
        {
            foreach (DirectoryEntry child in Server.Children)
            {
                string name = child.Name.ToLower();
                if (name == "iiswebvirtualdir" || name == "root")
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 修改与给定的IISWebServer具有相同网站说明的站点配置
        /// </summary>
        /// <param name="iisServer">给定的IISWebServer</param>
        public static void EditIISWebServer(IISWebServer iisServer)
        {
            if (iisServer.index == -1)
                throw (new Exception("找不到给定的站点!"));

            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server;

            IEnumerator ie = Service.Children.GetEnumerator();

            while (ie.MoveNext()) {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer") {
                    if (Server.Properties["Serverbindings"][0].ToString() == ":" + iisServer.Port + ":") {
                        Server.Invoke("stop", new object[0]);
                    }
                }
            }

            Server = GetIISWebserver(iisServer.index);
            if (Server == null) {
                throw (new Exception("找不到给定的站点!"));
            }

            try {
                Server.Properties["ServerComment"][0] = iisServer.ServerComment;
                Server.Properties["Serverbindings"][0] = ":" + iisServer.Port + ":";
                Server.Properties["AccessScript"][0] = iisServer.AccessScript;
                Server.Properties["AccessRead"][0] = iisServer.AccessRead;
                Server.Properties["EnableDirBrowsing"][0] = iisServer.EnableDirBrowsing;
                Server.Properties["DefaultDoc"][0] = iisServer.DefaultDoc;
                Server.Properties["EnableDefaultDoc"][0] = iisServer.EnableDefaultDoc;

                DirectoryEntry root = GetRoot(Server);

                Server.CommitChanges();
                if (root != null) {
                    root.Properties["path"][0] = iisServer.Path;
                    root.CommitChanges();
                }

                Server.Invoke("start", new object[0]);
            }
            catch (Exception ex) {
                throw (ex);
            }
        }

        /// <summary>
        /// 返回所有站点的网站说明
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetIISServerComment()
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server;

            ArrayList list = new ArrayList();
            IEnumerator ie = Service.Children.GetEnumerator();

            while (ie.MoveNext())
            {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer")
                {
                    list.Add(Server.Properties["ServerComment"][0]);
                }
            }

            return list;
        }

        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        /// <param name="iisVir">The IIS vir.</param>
        /// <param name="deleteIfExist">if set to <c>true</c> [delete if exist].</param>
        public static void CreateIISWebVirtualDir(IISWebVirtualDir iisVir, bool deleteIfExist)
        {
            if (iisVir.Parent == null)
                throw (new Exception("IISWebVirtualDir没有所属的IISWebServer!"));

            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server = GetIISWebserver(iisVir.Parent.index);

            if (Server == null) {
                throw (new Exception("找不到给定的站点!"));
            }

            Server = GetRoot(Server);
            if (deleteIfExist) {
                foreach (DirectoryEntry VirDir in Server.Children) {
                    if (VirDir.Name.ToLower().Trim() == iisVir.Name.ToLower()) {
                        Server.Children.Remove(VirDir);
                        Server.CommitChanges();
                        break;
                    }
                }
            }

            try {
                DirectoryEntry vir;
                vir = Server.Children.Add(iisVir.Name, "IIsWebVirtualDir");
                vir.Properties["Path"][0] = iisVir.Path;
                vir.Properties["DefaultDoc"][0] = iisVir.DefaultDoc;
                vir.Properties["EnableDefaultDoc"][0] = iisVir.EnableDefaultDoc;
                vir.Properties["AccessScript"][0] = iisVir.AccessScript;
                vir.Properties["AccessRead"][0] = iisVir.AccessRead;
                vir.Invoke("AppCreate2", new object[1] { 2 });

                Server.CommitChanges();
                vir.CommitChanges();
            }
            catch (Exception ex) {
                throw (ex);
            }

        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="WebServerComment">站点说明</param>
        /// <param name="VirtualDir">虚拟目录名称</param>
        public static void RemoveIISWebVirtualDir(string WebServerComment, string VirtualDir)
        {
            VirtualDir = VirtualDir.ToLower();
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server = GetIISWebserver(WebServerComment);

            if (Server == null)
            {
                throw (new Exception("找不到给定的站点!"));
            }

            Server = GetRoot(Server);
            foreach (DirectoryEntry VirDir in Server.Children)
            {
                if (VirDir.Name.ToLower().Trim() == VirtualDir)
                {
                    Server.Children.Remove(VirDir);
                    Server.CommitChanges();
                    return;
                }
            }

            throw (new Exception("找不到给定的虚拟目录!"));
        }

        /// <summary>
        /// 删除虚拟目录
        /// </summary>
        /// <param name="iisVir"></param>
        public static void RemoveIISWebVirtualDir(IISWebVirtualDir iisVir)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://" + IISManagement.Machinename + "/W3SVC");
            DirectoryEntry Server = GetIISWebserver(iisVir.Parent.index);

            if (Server == null) {
                throw (new Exception("找不到给定的站点!"));
            }

            Server = GetRoot(Server);
            foreach (DirectoryEntry VirDir in Server.Children) {
                if (VirDir.Name.ToLower().Trim() == iisVir.Name.ToLower()) {
                    Server.Children.Remove(VirDir);
                    Server.CommitChanges();
                    return;
                }
            }

            throw (new Exception("找不到给定的虚拟目录!"));
        }
    }
}
