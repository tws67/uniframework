using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Uniframework.Services;

namespace Uniframework.Upgrade
{
    /// <summary>
    /// 系统自动更新侦测类
    /// </summary>
    public class UpgradeDetecter : StartableBase
    {
        private ILiveUpgradeService liveupgradeService;
        private int checkInterval = 1800; // 默认30分钟检查一次
        private Thread thread;
        private object syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeDetecter"/> class.
        /// </summary>
        /// <param name="liveupgradeService">The liveupgrade service.</param>
        public UpgradeDetecter(ILiveUpgradeService liveupgradeService)
        {
            this.liveupgradeService = liveupgradeService;
            thread = new Thread(new ThreadStart(StartDetectUpgrade));
            thread.IsBackground = true;
            thread.Start();
        }

        protected override void OnStart()
        {
            isRun = true;
        }

        protected override void OnStop()
        {
            isRun = false;
        }

        /// <summary>
        /// 系统更新侦测程序
        /// </summary>
        private void StartDetectUpgrade()
        {
            while (IsRun) {
                Thread.Sleep(checkInterval * 1000);
                UpgradeProject proj = liveupgradeService.GetValidUpgradeProject();
                if (proj != null)
                    liveupgradeService.UpgradeNotify(proj);
            }
        }

        /// <summary>
        /// 侦测的间隔时间
        /// </summary>
        /// <value>The check interval.</value>
        public int CheckInterval
        {
            get { return checkInterval; }
            set {
                lock (syncObj) {
                    checkInterval = value;
                }
            }
        }
    }
}
