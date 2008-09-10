using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;
using Uniframework.Services;
using log4net;
using Uniframework.SmartClient;
using DevExpress.XtraEditors;
using System.Net;

namespace Uniframework.Upgrade.Views
{
    public class UpgradeBuilderPresenter : Presenter<UpgradeBuilderView>
    {
        
        private IUpgradeService upgradeService;
        private UpgradeProject project = null;
        private String uploadVirtaulPath = String.Empty;
        private ILog logger;

        #region Dependency services

        [ServiceDependency]
        public IUpgradeService UpgradeService
        {
            get { return upgradeService; }
            set { upgradeService = value; }
        }

        [ServiceDependency]
        public ILog Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        [ServiceDependency]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        #endregion

        public UpgradeSetting UpgradeSetting
        {
            get {
                UpgradeSetting setting = PropertyService.Get<UpgradeSetting>(UIExtensionSiteNames.Shell_Property_Upgrade, new UpgradeSetting());
                return setting;
            }
        }

        /// <summary>
        /// 上传升级项目到更新服务器
        /// </summary>
        public void UploadProject()
        {
            if (!View.UpgradeProjectIsValid())
                return;

            try
            {
                using (WaitCursor cursor = new WaitCursor(true)) {
                    project = View.WrapUpgradeProject();
                    uploadVirtaulPath = UpgradeService.CreateVirtualDirectory(project.UpgradeServer, project);
                    
                    List<UpgradeItem> items = new List<UpgradeItem>();
                    foreach (UpgradeGroup group in project.Groups)
                        items.AddRange(group.Items);

                    uploadTotal = 0;
                    WebClient client = new WebClient();
                    client.UploadProgressChanged += new UploadProgressChangedEventHandler(webClient_UploadProgressChanged);
                    foreach (UpgradeItem item in items)
                    {
                        Uri uri = new Uri(uploadVirtaulPath + "/" + item.Name);
                        client.UploadFile(uri, "PUT", item.FileName);
                        uploadTotal += item.Size;
                    }
                    UpgradeService.CreateUpgradeProject(project);
                }
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show("上传系统升级项目失败, " + ex.Message);
            }
            finally
            {
                OnShellProgressBarChanged(-1);
            }
        }
 
        private long uploadTotal = 0; // 当前一共上传的字节数

        /// <summary>
        /// Handles the UploadProgressChanged event of the webClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Net.UploadProgressChangedEventArgs"/> instance containing the event data.</param>
        private void webClient_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
           OnShellProgressBarChanged((int)((uploadTotal + e.BytesSent) / project.TotalSize * 100));
        }

        /// <summary>
        /// Occurs when [shell progress bar changed].
        /// </summary>
        [EventPublication(EventNames.Shell_ProgressBarChanged, PublicationScope.Global)]
        public event EventHandler<EventArgs<int>> ShellProgressBarChanged;

        /// <summary>
        /// Called when [shell progress bar changed].
        /// </summary>
        /// <param name="progress">The progress.</param>
        protected void OnShellProgressBarChanged(int progress)
        {
            if (ShellProgressBarChanged != null) { 
                ShellProgressBarChanged(this, new EventArgs<int>(progress));
            }
        }
    }
}
