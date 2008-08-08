using System;
using System.Collections;
using System.Data;
using System.DirectoryServices;
using System.IO;

namespace Uniframework.Services
{
    /// <summary>
    /// IISManager 的摘要说明。
    /// </summary>
    public class IISManager : IDisposable
    {
        private string server, website, anonymousUserPass, anonymousUserName;
        private VirtualDirectories virtualDirectories;
        private DirectoryEntry rootfolder;
        private readonly static string SCHEMACLASS_NAME = "IIsWebVirtualDir";
        private bool batchflag;

        public IISManager()
        {
            server = "localhost";
            website = "1";
            batchflag = false;
        }

        public IISManager(string server)
        {
            this.server = server;
            website = "1";
            batchflag = false;
        }

        #region Members

        public string AnonymousUserName
        {
            get { return anonymousUserName; }
            set { anonymousUserName = value; }
        }

        public string AnonymousUserPass
        {
            get { return anonymousUserPass; }
            set { anonymousUserPass = value; }
        }

        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        /// <summary>
        /// WebSite属性定义，为一数字，为方便，使用string；一般来说第一台主机为1,第二台主机为2，依次类推
        /// </summary>
        public string WebSite
        {
            get { return website; }
            set { website = value; }
        }

        public VirtualDirectories VirtualDirectories
        {
            get { return virtualDirectories; }
            set { virtualDirectories = value; }
        }

        #endregion

        #region Methods

        public void Connect()
        {
            ConnectToServer();
        }

        public void Connect(string server)
        {
            this.server = server;
            ConnectToServer();
        }

        public void Connect(string server, string webSite)
        {
            this.server = server;
            this.website = webSite;
            ConnectToServer();
        }

        public bool Exists(string virDir)
        {
            return virtualDirectories.Contains(virDir);
        }

        public bool CreateVirtualDirectory(VirtualDirectory vd)
        {
            string path = "IIS://" + server + "/W3SVC/" + website + "/ROOT/" + vd.Name;
            if (!virtualDirectories.Contains(vd.Name) || batchflag)
            {
                try
                {
                    if (!Directory.Exists(vd.PhysicalPath))
                        Directory.CreateDirectory(vd.PhysicalPath);

                    DirectoryEntry myDE = rootfolder.Children.Add(vd.Name, SCHEMACLASS_NAME);
                    myDE.Invoke("AppCreate", true);
                    myDE.CommitChanges();
                    rootfolder.CommitChanges();
                    UpdateVirtualDirectoryInfo(myDE, vd);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public VirtualDirectory GetVirtualDirectory(string virDir)
        {
            VirtualDirectory tmp = null;
            if (virtualDirectories.Contains(virDir))
            {
                tmp = virtualDirectories.Find(virDir);
                ((VirtualDirectory)virtualDirectories[virDir]).Flag = 2;
            }
            return tmp;
        }

        public void UpdateVirtualDirecotry(VirtualDirectory vd)
        {
            //判断需要更改的虚拟目录是否存在
            if (virtualDirectories.Contains(vd.Name))
            {
                DirectoryEntry ode = rootfolder.Children.Find(vd.Name, SCHEMACLASS_NAME);
                UpdateVirtualDirectoryInfo(ode, vd);
            }
        }

        public void DeleteVirtualDirectory(string vdName)
        {
            if (virtualDirectories.Contains(vdName))
            {
                object[] paras = new object[2];
                paras[0] = SCHEMACLASS_NAME; // 表示操作的是虚拟目录
                paras[1] = vdName;
                rootfolder.Invoke("Delete", paras);
                rootfolder.CommitChanges();
            }
        }

        public void UpdateBatch()
        {
            BatchUpdate(virtualDirectories);
        }

        public void UpdateBatch(VirtualDirectories vds)
        {
            BatchUpdate(vds);
        }

        public void GetAnonymousUser()
        {
            anonymousUserPass = "IUSR_DEVE-SERVER";
            anonymousUserName = "IUSR_DEVE-SERVER";
            VirtualDirectory vDir;
            try
            {
                Hashtable myList = (Hashtable)virtualDirectories;
                IDictionaryEnumerator myEnumerator = myList.GetEnumerator();
                while (myEnumerator.MoveNext())
                {
                    vDir = (VirtualDirectory)myEnumerator.Value;
                    if (vDir.AnonymousUserName != "" && vDir.AnonymousUserPass != "")
                    {
                        anonymousUserName = vDir.AnonymousUserName;
                        anonymousUserPass = vDir.AnonymousUserPass;
                        break;
                    }
                }
            }
            catch
            {
                anonymousUserPass = "IUSR_DEVE-SERVER";
                anonymousUserName = "IUSR_DEVE-SERVER";
            }
        }

        #endregion

        #region Assistant functions

        private void Close()
        {
            virtualDirectories.Clear();
            virtualDirectories = null;
            rootfolder.Dispose();
        }

        private void ConnectToServer()
        {
            string path = "IIS://" + server + "/W3SVC/" + website + "/ROOT";
            try
            {
                this.rootfolder = new DirectoryEntry(path);
                virtualDirectories = GetVirtualDirectories(this.rootfolder.Children);
            }
            catch (Exception ex)
            {
                throw new Exception("Can''t connect to the server [" + server + "] ...", ex);
            }
        }

        private void BatchUpdate(VirtualDirectories vds)
        {
            batchflag = true;
            foreach (object item in vds.Values)
            {
                VirtualDirectory vd = (VirtualDirectory)item;
                switch (vd.Flag)
                {
                    case 0:
                        break;
                    case 1:
                        CreateVirtualDirectory(vd);
                        break;
                    case 2:
                        UpdateVirtualDirecotry(vd);
                        break;
                }
            }
            batchflag = false;
        }

        private void UpdateVirtualDirectoryInfo(DirectoryEntry de, VirtualDirectory vd)
        {
            de.Properties["AnonymousUserName"][0] = vd.AnonymousUserName;
            de.Properties["AnonymousUserPass"][0] = vd.AnonymousUserPass;
            de.Properties["AccessRead"][0] = vd.AccessRead;
            de.Properties["AccessExecute"][0] = vd.AccessExecute;
            de.Properties["AccessWrite"][0] = vd.AccessWrite;
            de.Properties["AuthBasic"][0] = vd.AuthBasic;
            de.Properties["AuthNTLM"][0] = vd.AuthNTLM;
            de.Properties["ContentIndexed"][0] = vd.ContentIndexed;
            de.Properties["EnableDefaultDoc"][0] = vd.EnableDefaultDoc;
            de.Properties["EnableDirBrowsing"][0] = vd.EnableDirBrowsing;
            de.Properties["AccessSSL"][0] = vd.AccessSSL;
            de.Properties["AccessScript"][0] = vd.AccessScript;
            de.Properties["DefaultDoc"][0] = vd.DefaultDoc;
            de.Properties["Path"][0] = vd.PhysicalPath;
            de.CommitChanges();
        }

        private VirtualDirectories GetVirtualDirectories(DirectoryEntries des)
        {
            VirtualDirectories tmpdirs = new VirtualDirectories();
            foreach (DirectoryEntry de in des)
            {
                if (de.SchemaClassName == SCHEMACLASS_NAME)
                {
                    VirtualDirectory vd = new VirtualDirectory();
                    vd.Name = de.Name;
                    vd.AccessRead = (bool)de.Properties["AccessRead"][0];
                    vd.AccessExecute = (bool)de.Properties["AccessExecute"][0];
                    vd.AccessWrite = (bool)de.Properties["AccessWrite"][0];
                    vd.AnonymousUserName = (string)de.Properties["AnonymousUserName"][0];
                    vd.AnonymousUserPass = (string)de.Properties["AnonymousUserName"][0];
                    vd.AuthBasic = (bool)de.Properties["AuthBasic"][0];
                    vd.AuthNTLM = (bool)de.Properties["AuthNTLM"][0];
                    vd.ContentIndexed = (bool)de.Properties["ContentIndexed"][0];
                    vd.EnableDefaultDoc = (bool)de.Properties["EnableDefaultDoc"][0];
                    vd.EnableDirBrowsing = (bool)de.Properties["EnableDirBrowsing"][0];
                    vd.AccessSSL = (bool)de.Properties["AccessSSL"][0];
                    vd.AccessScript = (bool)de.Properties["AccessScript"][0];
                    vd.PhysicalPath = (string)de.Properties["Path"][0];
                    vd.Flag = 0;
                    vd.DefaultDoc = (string)de.Properties["DefaultDoc"][0];
                    tmpdirs.Add(vd.Name, vd);
                }
            }
            return tmpdirs;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }

    /// <summary>
    /// VirtualDirectory类
    /// </summary>
    public class VirtualDirectory
    {
        private bool read, execute, script, ssl, write, authbasic, authNTLM, indexed, enableDirbrow, enableDefaultdoc;
        private string ausername, auserpass, name, physicalPath;
        private int flag;
        private string defaultdoc;

        /// <summary>
        /// 构造函数
        /// </summary>
        public VirtualDirectory()
        {
            read = true;
            execute = false;
            script = true;
            ssl = false;
            write = false;
            authbasic = false;
            authNTLM = true;
            indexed = true;
            enableDirbrow = false;
            enableDefaultdoc = true;
            flag = 1;
            defaultdoc = "default.htm, default.aspx, default.asp, index.htm";
            physicalPath = @"C:\\";
            ausername = "IUSR_DEVE-SERVER";
            auserpass = "IUSR_DEVE-SERVER";
            name = "";
        }

        public VirtualDirectory(string virDir)
            : this()
        {
            name = virDir;
        }

        public VirtualDirectory(string virDir, string physicalPath)
            : this(virDir)
        {
            this.physicalPath = physicalPath;
        }

        public VirtualDirectory(string virDir, string physicalPath, string anonymousUser, string anonymousPass)
            : this(virDir, physicalPath)
        {
            ausername = anonymousUser;
            auserpass = anonymousPass;
        }

        #region Members

        public int Flag
        {
            get { return flag; }
            set { flag = value; }
        }

        public bool AccessRead
        {
            get { return read; }
            set { read = value; }
        }

        public bool AccessWrite
        {
            get { return write; }
            set { write = value; }
        }

        public bool AccessExecute
        {
            get { return execute; }
            set { execute = value; }
        }

        public bool AccessSSL
        {
            get { return ssl; }
            set { ssl = value; }
        }

        public bool AccessScript
        {
            get { return script; }
            set { script = value; }
        }

        public bool AuthBasic
        {
            get { return authbasic; }
            set { authbasic = value; }
        }

        public bool AuthNTLM
        {
            get { return authNTLM; }
            set { authNTLM = value; }
        }

        public bool ContentIndexed
        {
            get { return indexed; }
            set { indexed = value; }
        }

        public bool EnableDirBrowsing
        {
            get { return enableDirbrow; }
            set { enableDirbrow = value; }
        }

        public bool EnableDefaultDoc
        {
            get { return enableDefaultdoc; }
            set { enableDefaultdoc = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PhysicalPath
        {
            get { return physicalPath; }
            set { physicalPath = value; }
        }

        public string DefaultDoc
        {
            get { return defaultdoc; }
            set { defaultdoc = value; }
        }

        public string AnonymousUserName
        {
            get { return ausername; }
            set { ausername = value; }
        }

        public string AnonymousUserPass
        {
            get { return auserpass; }
            set { auserpass = value; }
        }

        #endregion
    }

    /// <summary>
    /// 集合VirtualDirectories
    /// </summary>

    public class VirtualDirectories : System.Collections.Hashtable
    {
        public VirtualDirectories()
        {
        }
        //添加新的方法
        public VirtualDirectory Find(string name)
        {
            return (VirtualDirectory)this[name];
        }
    }
}