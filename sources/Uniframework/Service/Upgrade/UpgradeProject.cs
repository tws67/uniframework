using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统自动更新配置信息
    /// </summary>
    [Serializable]
    public class UpgradeProject
    {
        private string product;
        private string version;
        private string description;
        private string upgradeServer;
        private DateTime upgradePatchTime = DateTime.Now;
        private bool upgradeForce = false;
        private string startUpApp;
        private long totalSize = 0;
        private IList<UpgradeGroup> groups = new List<UpgradeGroup>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public UpgradeProject()
        { }

        /// <summary>
        /// 更新的产品名称
        /// </summary>
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        public string UpgradeServer
        {
            get { return upgradeServer; }
            set { upgradeServer = value; }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpgradePatchTime
        {
            get { return upgradePatchTime; }
        }

        /// <summary>
        /// 强制更新标志
        /// </summary>
        public bool UpgradeForce
        {
            get { return upgradeForce; }
            set { upgradeForce = value; }
        }

        /// <summary>
        /// 系统启动程序
        /// </summary>
        public string StartUpApp
        {
            get { return startUpApp; }
            set
            {
                if (startUpApp != value)
                {
                    startUpApp = value;
                }
            }
        }

        /// <summary>
        /// 更新的组
        /// </summary>
        public IList<UpgradeGroup> Groups
        {
            get { return groups; }
        }

        /// <summary>
        /// 更新大小
        /// </summary>
        public long TotalSize
        {
            get
            {
                totalSize = 0;
                foreach (UpgradeGroup group in groups)
                    totalSize += group.SubTotalSize;
                return totalSize;
            }
        }
    }
}