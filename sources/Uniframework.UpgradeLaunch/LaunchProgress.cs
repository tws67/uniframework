using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using DevExpress.XtraEditors;
using log4net;
using log4net.Config;
using Uniframework.Services;
using System.Configuration;
using System.IO;
using System.Diagnostics;


namespace Uniframework.UpgradeLaunch
{
    public partial class LaunchProgress : DevExpress.XtraEditors.XtraForm
    {
        private readonly static string UPGRADE_LOGCONFIGFILE = "UpgradeLaunch.config";
        private readonly static string UPGRADE_SECTION = "LiveUpgrade";
        private readonly static string APP_CONFIGFILE = "Uniframework.StartUp.exe";

        private readonly static string UPGRADE_CONFIGFILE = "Upgrade.dat";
        private readonly static string UPGRADE_LOGFILE = @"..\Logs\Upgrade.log";
        private readonly static string ROOT_PATH = FileUtility.GetParent(FileUtility.ApplicationRootPath);

        private string upgradePath;
        private UpgradeProject project;
        private ILog logger = null;

        public LaunchProgress()
        {
            InitializeComponent();
        }

        public LaunchProgress(string upgradePath)
            : this()
        {
            this.upgradePath = upgradePath;
            string configfile = Path.Combine(upgradePath, UPGRADE_CONFIGFILE);
            if (!File.Exists(configfile))
                throw new Exception(String.Format("不存在升级配置文件 {0}。", configfile));

            // 加载日志组件
            XmlConfigurator.Configure(new FileInfo(UPGRADE_LOGCONFIGFILE));
            logger = LogManager.GetLogger(typeof(LaunchProgress));

            // 反序列化升级配置文件
            FileStream fs = new FileStream(configfile, FileMode.Open);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            Serializer serializer = new Serializer();
            project = serializer.Deserialize<UpgradeProject>(buffer);
        }

        private void StartUpgrade()
        {
            logger.Info(String.Format("开始更新系统,本次更新的产品名称为:{0},版本号:{1},服务器端发布时间:{2}", project.Product, project.Version, project.UpgradePatchTime));
            logger.Info(String.Format("本次更新描述:{0}", project.Description));

            foreach (UpgradeGroup group in project.Groups)
            {
                UpgradeFiles(group);
            }
            logger.Info("完成本次系统更新工作。");

            try
            {
                UpdateConfiguration();
                File.Copy(UPGRADE_LOGFILE, Path.Combine(upgradePath, UPGRADE_LOGFILE), true);
                File.Delete(UPGRADE_LOGFILE);
            }
            catch (Exception ex)
            {
                logger.Info("在保存本次更新时出现错误," + ex.Message);
            }

            // 重新启动程序
            try
            {
                Process.Start(project.StartUpApp);
            }
            catch
            {
                Environment.Exit(0);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        private void UpgradeFiles(UpgradeGroup group)
        {
            logger.Info(String.Format("准备更新组:{0} ", group.Name));

            // 创建更新的目标文件夹
            string targetPath = group.Target.IndexOf("${RootPath}") != -1 ? Path.Combine(ROOT_PATH, group.Target.Substring(11, group.Target.Length - 11)) : group.Target;
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            foreach (UpgradeItem item in group.Items)
            {
                string srcfile = Path.Combine(upgradePath, item.Name);
                string tarfile = Path.Combine(targetPath, item.Name);
                if (!File.Exists(srcfile))
                {
                    logger.Info(String.Format("下载 {0} 不成功更新此文件失败", item.Name));
                    continue;
                }

                try
                {
                    logger.Info(String.Format("删除待更新的旧版本文件 {0}", tarfile));
                    File.Delete(tarfile);
                    logger.Info(String.Format("更新文件 {0}", item.Name));
                    File.Move(srcfile, tarfile);
                    logger.Info(String.Format("更新文件 {0} 成功", item.Name));
                }
                catch (Exception ex)
                {
                    logger.Info(String.Format("更新文件 {0} 失败,错误原因为:", item.Name, ex.Message));
                }
            }
            logger.Info(String.Format("完成更新组:{0} ", group.Name));
        }

        private void LaunchProgress_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(StartUpgrade));
            thread.Start();
        }

        private void UpdateConfiguration()
        {
            string exePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), APP_CONFIGFILE);
            Configuration conf = ConfigurationManager.OpenExeConfiguration(exePath);

            LiveUpgradeConfigurationSection newCS = new LiveUpgradeConfigurationSection();
            LiveUpgradeConfigurationSection cs = conf.GetSection(UPGRADE_SECTION) as LiveUpgradeConfigurationSection;

            if (cs != null)
            {
                foreach (UpgradeElement element in cs.UpgradeProducts)
                {
                    if (element.Product != project.Product)
                    {
                        newCS.UpgradeProducts.AddElement(element);
                    }
                }
            }

            UpgradeElement ele = new UpgradeElement();
            ele.Product = project.Product;
            ele.UpgradeDate = DateTime.Now.ToLocalTime();
            ele.LocalVersion = project.Version;
            newCS.UpgradeProducts.AddElement(ele);

            conf.Sections.Remove(UPGRADE_SECTION);
            conf.Sections.Add(UPGRADE_SECTION, newCS);
            conf.Save();
        }
    }
}