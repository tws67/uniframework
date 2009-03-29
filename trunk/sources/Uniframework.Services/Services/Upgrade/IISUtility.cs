using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;

namespace Uniframework.Services.Services.Upgrade
{
    public static class IISUtility
    {
        private static readonly string SCHEMACLASS_NAME = "IIsWebVirtualDir";

        /// <summary>
        /// 创建IIS虚拟目录
        /// </summary>
        /// <param name="webSite">The web site.</param>
        /// <param name="vdir">The vdir.</param>
        /// <param name="physicalPath">The physical path.</param>
        /// <param name="rootDir">if set to <c>true</c> [root dir].</param>
        /// <param name="chkRead">if set to <c>true</c> [CHK read].</param>
        /// <param name="chkWrite">if set to <c>true</c> [CHK write].</param>
        /// <param name="chkExecute">if set to <c>true</c> [CHK execute].</param>
        /// <param name="chkScript">if set to <c>true</c> [CHK script].</param>
        /// <param name="chkAuth">if set to <c>true</c> [CHK auth].</param>
        /// <param name="webSiteNum">The web site num.</param>
        /// <param name="serverName">Name of the server.</param>
        /// <returns></returns>
        public static string CreateVDir(string webSite, string vdir, string physicalPath, bool rootDir, bool chkRead, bool chkWrite, bool chkExecute, bool chkScript, bool chkAuth, int webSiteNum, string serverName)
        {
            string sRet = String.Empty;
            DirectoryEntry IISSchema;
            DirectoryEntry IISAdmin;
            DirectoryEntry VDir;
            bool IISUnderNT;

            /// 确定IIS版本
            IISSchema = new DirectoryEntry("IIS://" + serverName + "/Schema/AppIsolated");
            if (IISSchema.Properties["Syntax"].Value.ToString().ToUpper() == "BOOLEAN")
                IISUnderNT = true;
            else
                IISUnderNT = false;
            IISSchema.Dispose();

            IISAdmin = new DirectoryEntry("IIS://" + serverName + "/W3SVC/" + webSiteNum + "/Root");
            if (!rootDir) {
                // 如果虚拟目录已经存在则删除            
                foreach (DirectoryEntry de in IISAdmin.Children) {
                    if (de.Name == vdir && de.SchemaClassName == SCHEMACLASS_NAME) {
                        // Delete the specified virtual directory if it already exists                           
                        try {
                            IISAdmin.Invoke("Delete", new object[] { de.SchemaClassName, vdir });
                            IISAdmin.CommitChanges();
                        }
                        catch (Exception ex) {
                            sRet += ex.Message;
                        }
                    }
                }
            }

            /// 创建一个虚拟目录
            try {
                if (!Directory.Exists(physicalPath))
                    Directory.CreateDirectory(physicalPath);
                
                if (!rootDir)
                    VDir = IISAdmin.Children.Add(vdir, SCHEMACLASS_NAME);
                else
                    VDir = IISAdmin;

                // 设置属性              
                VDir.Properties["AccessRead"][0] = chkRead;
                VDir.Properties["AccessExecute"][0] = chkExecute;
                VDir.Properties["AccessWrite"][0] = chkWrite;
                VDir.Properties["AccessScript"][0] = chkScript;
                VDir.Properties["AuthNTLM"][0] = chkAuth;
                VDir.Properties["EnableDefaultDoc"][0] = true;
                VDir.Properties["EnableDirBrowsing"][0] = false;
                VDir.Properties["DefaultDoc"][0] = "default.aspx,default.html,index.aspx,index.html";
                VDir.Properties["Path"][0] = physicalPath;

                if (!IISUnderNT)
                    VDir.Properties["AspEnableParentPaths"][0] = true;

                VDir.CommitChanges();  // 提交更改

                if (IISUnderNT)
                    VDir.Invoke("AppCreate", false);
                else
                    VDir.Invoke("AppCreate", 1);
            }
            catch (Exception ex) {
                throw ex;
            }
            sRet += "VRoot " + vdir + " created!";
            return sRet;
        }
    }
}
