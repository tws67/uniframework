using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Uniframework.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统更新服务
    /// </summary>
    public class UpgradeService : IUpgradeService
    {
        private ILogger logger;
        private object SyncObj = new object();
        private IDb4oDatabase db;

        private readonly static string UPGRADE_CONFIGURATION_FILE = "Configuration.yap";
        private readonly static string UPGRADE_PATH = "~/Upgrade/";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="databaseService">数据库服务</param>
        /// <param name="loggerFactory">日志服务工厂</param>
        public UpgradeService(IDb4oDatabaseService databaseService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<UpgradeService>();
            Db4oFactory.Configure().ObjectClass(typeof(UpgradeProject)).CascadeOnDelete(true); // 设置级联删除
            db = databaseService.Open(UPGRADE_CONFIGURATION_FILE);
        }

        #region IUpgradeService Members

        /// <summary>
        /// 获取指定软件项目的最新版本
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <returns>返回更新项目配置情况</returns>
        public UpgradeProject GetUpgradeProject(string product)
        {
            lock (SyncObj)
            {
                IList<UpgradeProject> projects = db.Load<UpgradeProject>(delegate(UpgradeProject project) {
                    return product == project.Product;
                });
                if (projects.Count > 0)
                {
                    UpgradeProject project = projects[0];
                    foreach (UpgradeProject proj in projects)
                    {
                        Version ver1 = new Version(project.Version);
                        Version ver2 = new Version(proj.Version);
                        if (ver1 < ver2 || project.UpgradePatchTime < proj.UpgradePatchTime)
                            project = proj;
                    }
                    return project;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取指定软件项目指定版本的更新配置信息
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <param name="version">软件版本</param>
        /// <returns>返回更新项目配置情况</returns>
        public UpgradeProject GetUpgradeProject(string product, string version)
        {
            lock (SyncObj)
            {
                IList<UpgradeProject> upgradeProjects = db.Load<UpgradeProject>(delegate(UpgradeProject project) {
                    return project.Product == product && project.Version == version;
                });
                if (upgradeProjects.Count > 0)
                    return upgradeProjects[0];
                return null;
            }
        }

        /// <summary>
        /// 获取指定版本的升级项目的升级URL
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <param name="version">版本号</param>
        /// <returns>返回指定版本升级项目的升级URL，如果服务器端不存在其路径返回空</returns>
        public string GetUpgradeUrl(string product, string version)
        {
            UpgradeProject project = GetUpgradeProject(product, version);
            if (project != null)
            {
                IISManager iisManager = new IISManager(project.UpgradeServer);
                iisManager.Connect();
                string virtualPath = UPGRADE_PATH + product + "/" + version;
                return GetUpgradeAbsoluteUrl(virtualPath);
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// 获取指定软件项目的更新历史信息
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <returns>返回所有更新配置情况列表</returns>
        public IList<UpgradeProject> GetUpgradeHistory(string product)
        {
            lock (SyncObj)
            {
                IList<UpgradeProject> upgradeProjects = db.Load<UpgradeProject>(delegate(UpgradeProject project) {
                    return project.Product == product;
                });
                IList<UpgradeProject> results = new List<UpgradeProject>();
                foreach (UpgradeProject project in upgradeProjects)
                    results.Add(project);
                return results;
            }
        }

        /// <summary>
        /// 创建一个软件更新项目
        /// </summary>
        /// <param name="project">软件更新项目配置信息</param>
        public void CreateUpgradeProject(UpgradeProject project)
        {
            db.Store(project);
            if (UpgradeProjectCreated != null)
                UpgradeProjectCreated(this, new EventArgs<UpgradeProject>(project));
        }

        /// <summary>
        /// 删除一个软件更新项目
        /// </summary>
        /// <param name="product">项目名称</param>
        /// <param name="version">软件版本</param>
        public void DeleteUpgradeProject(string product, string version)
        {
            UpgradeProject project = GetUpgradeProject(product, version);
            if (project != null)
                db.Delete(project);
        }

        /// <summary>
        /// 删除指定名称软件的所有更新项目
        /// </summary>
        /// <param name="product">项目名称</param>
        public void DeleteUpgradeProjects(string product)
        {
            lock (SyncObj)
            {
                IList<UpgradeProject> upgradeProjects = db.Load<UpgradeProject>(delegate(UpgradeProject project) {
                    return project.Product == product;
                });
                foreach (UpgradeProject project in upgradeProjects)
                    db.Delete(project);
            }
        }

        /// <summary>
        /// 创建更新服务所用的虚拟目录
        /// </summary>
        /// <param name="server">服务器名称</param>
        /// <param name="proj">更新项目</param>
        /// <returns>如果创建成功则返回true，否则为false</returns>
        public string CreateVirtualDirectory(string server, UpgradeProject proj)
        {
            Guard.ArgumentNotNull(proj, "Upgrade project");
            string virtualPath = String.Empty;
            IISManager iisManager = new IISManager(server);
            try {
                iisManager.Connect();
                string physicalPath = HttpContext.Current.Server.MapPath(UPGRADE_PATH + proj.Product + "/" + proj.Version);
                virtualPath = UPGRADE_PATH + proj.Product + "/" + proj.Version;
                logger.Debug("创建升级虚拟目录: " + virtualPath + ", 物理路径为: " + physicalPath);
                VirtualDirectory vd = new VirtualDirectory(virtualPath, physicalPath);
                vd.AccessExecute = true;
                vd.AccessWrite = true;
                iisManager.CreateVirtualDirectory(vd);
            }
            catch(Exception ex) {
                logger.Debug("创建升级虚拟目录: " + virtualPath + " 失败, " + ex.Message);
                return String.Empty;
            }
            return GetUpgradeAbsoluteUrl(virtualPath);
        }

        /// <summary>
        /// 删除指定的虚拟目录
        /// </summary>
        /// <param name="server">服务器名称</param>
        /// <param name="vdName">虚拟目录</param>
        public void DeleteVirtualDirectory(string server, string vdName)
        {
            IISManager iisManager = new IISManager(server);
            try
            {
                iisManager.Connect();
                iisManager.DeleteVirtualDirectory(vdName);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 软件更新项目创建事件用于通知目前在线的客户端可以及时的进行系统更新
        /// </summary>
        public event EventHandler<EventArgs<UpgradeProject>> UpgradeProjectCreated;

        #endregion

        #region Assistant functions

        /// <summary>
        /// Gets the upgrade absolute URL.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        private string GetUpgradeAbsoluteUrl(string virtualPath)
        {
            return "http://" + HttpContext.Current.Request.Url.Authority + 
                VirtualPathUtility.ToAbsolute(virtualPath);
        }

        #endregion
    }
}
