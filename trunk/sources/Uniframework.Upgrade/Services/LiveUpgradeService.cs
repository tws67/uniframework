using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;

using Uniframework.Services;
using DevExpress.XtraEditors;
using Uniframework.SmartClient;

namespace Uniframework.Upgrade
{
    /// <summary>
    /// 本地更新服务
    /// </summary>
    public class LiveUpgradeService : ILiveUpgradeService, IDisposable
    {
        private readonly static string UPGRADELAUNCH_FILE = "UpgradeLaunch.exe";
        private readonly static string LIVEUPGRADE_SECTION = "LiveUpgrade";

        private WorkItem workItem;
        private IUpgradeService upgradeService;
        private Thread thread;
        private bool abort = false;
        private int UPGRADE_INTERVAL = 1800;

        public LiveUpgradeService()
        {
            LiveUpgradeConfigurationSection cs = ConfigurationManager.GetSection(LIVEUPGRADE_SECTION) as LiveUpgradeConfigurationSection;
            if (cs != null)
            {
                thread = new Thread(new ThreadStart(StartUpgradeDetect));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        #region Dependency services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set { workItem = value; }
        }

        [ServiceDependency]
        public IUpgradeService UpgradeService
        {
            get { return upgradeService; }
            set { upgradeService = value; }
        }

        [ServiceDependency]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        #endregion

        #region ILiveUpgradeService Members

        public UpgradeProject GetValidUpgradeProject()
        {
            UpgradeElements upgradeElements = null;
            LiveUpgradeConfigurationSection cs = ConfigurationManager.GetSection(LIVEUPGRADE_SECTION) as LiveUpgradeConfigurationSection;
            if (cs != null)
            {
                upgradeElements = cs.UpgradeProducts;
            }

            // 检查需要更新的升级项目
            // 先检查相对于本地版本号大1的版本如果没有的话则直接获取其服务器端最新的更新版本
            foreach (UpgradeElement element in upgradeElements)
            {
                Version ver = new Version(element.LocalVersion);
                string upgradeVersion = String.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision + 1);

                UpgradeProject proj = UpgradeService.GetUpgradeProject(element.Product, upgradeVersion);
                if (proj == null)
                {
                    proj = UpgradeService.GetUpgradeProject(element.Product);
                    if (proj == null)
                        continue;
                }

                Version localVer = new Version(element.LocalVersion);
                Version newVer = new Version(proj.Version);
                if (localVer < newVer || proj.UpgradePatchTime > element.UpgradeDate)
                    return proj;
            }
            return null;
        }

        /// <summary>
        /// 服务器端系统升级更新订阅程序
        /// </summary>
        /// <param name="sender">事件触发者</param>
        /// <param name="e">事件参数</param>
        [EventSubscriber("TOPIC://Upgrade/UpgradeProjectCreated")]
        public void OnUpgradeProjectCreated(object sender, EventArgs<UpgradeProject> e)
        {
            if (UpgradeSetting.ReciveUpgradeMessage)
                UpgradeNotify(e.Data);
        }

        #endregion

        #region Assistant functions

        private void StartUpgradeDetect()
        {
            while (!abort)
            {
                Thread.Sleep(UPGRADE_INTERVAL);
                UpgradeProject proj = GetValidUpgradeProject();
                if (proj != null)
                    UpgradeNotify(proj);
            }
        }

        /// <summary>
        /// 系统升级提示
        /// </summary>
        /// <param name="project"></param>
        public void UpgradeNotify(UpgradeProject project)
        {
            string upgradeUrl = UpgradeService.GetUpgradeUrl(project.Product, project.Version);
            if (XtraMessageBox.Show(String.Format("服务器发布了 \'{0}\" 的新版本 {1}，您要立即升级到新版本吗？", project.Product, project.Version),
                "升级提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UpgradeProcess up = new UpgradeProcess(project, upgradeUrl);
                if (up.ShowDialog() == DialogResult.OK)
                {
                    SerializeUpgrade(project);
                    ReStartApplication(project);
                }
            }
        }

        /// <summary>
        /// 重启程序
        /// </summary>
        /// <param name="project"></param>
        private void ReStartApplication(UpgradeProject project)
        {
            string upgradeLaunch = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), UPGRADELAUNCH_FILE);
            if (File.Exists(upgradeLaunch))
            {
                string upgradePath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Upgrade\" + project.Product + @"\" + project.Version;
                Process.Start(upgradeLaunch, upgradePath);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 保存更新配置文件
        /// </summary>
        /// <param name="project">更新项目</param>
        private void SerializeUpgrade(UpgradeProject project)
        {
            string upgradeConfigfile = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Upgrade\" + project.Product + @"\" + project.Version + @"\Upgrade.dat";
            Serializer serializer = new Serializer();
            try
            {
                byte[] buffer = serializer.Serialize<UpgradeProject>(project);
                FileStream fs = new FileStream(upgradeConfigfile, FileMode.Create);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            catch
            {

            }
        }

        private UpgradeSetting UpgradeSetting
        {
            get {
                return PropertyService.Get<UpgradeSetting>(UIExtensionSiteNames.Shell_Property_Upgrade, new UpgradeSetting());
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            abort = true;
        }

        #endregion
    }
}
