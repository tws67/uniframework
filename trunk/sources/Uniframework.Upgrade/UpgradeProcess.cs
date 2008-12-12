using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using Uniframework.Services;
using System.Threading;
using System.Net;

namespace Uniframework.Upgrade
{
    public partial class UpgradeProcess : DevExpress.XtraEditors.XtraForm
    {
        private bool isFinished = false;
        private UpgradeProject upgradeProject;
        private string upgradeUrl;
        private string upgradeTempPath;
        private ManualResetEvent eventUpgrade = null;
        private ManualResetEvent evtPerDonwload = null;
        private WebClient client = null;
        private List<UpgradeItem> downloadItems = new List<UpgradeItem>();

        public UpgradeProcess(UpgradeProject upgradeProject, string upgradeUrl)
        {
            this.upgradeProject = upgradeProject;
            this.upgradeUrl = upgradeUrl;
            this.upgradeTempPath = FileUtility.ConvertToFullPath(@"..\Upgrade\") + upgradeProject.Product + @"\" + upgradeProject.Version + @"\";
            if (!Directory.Exists(upgradeTempPath))
                Directory.CreateDirectory(upgradeTempPath);
            downloadItems.Clear();
            foreach (UpgradeGroup group in upgradeProject.Groups)
                downloadItems.AddRange(group.Items);

            InitializeComponent();
        }

        private void UpgradeProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isFinished && XtraMessageBox.Show("您要取消当前的系统升级操作吗？", "提示", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                if (client != null)
                    client.CancelAsync();

                eventUpgrade.Set();
                evtPerDonwload.Set();
            }
        }

        private long totalSize = 0;
        private long downloadSize = 0;

        /// <summary>
        /// 执行下载操作
        /// </summary>
        private void StartDownload()
        {
            evtPerDonwload = new ManualResetEvent(false);
            totalSize = upgradeProject.TotalSize;

            while (!eventUpgrade.WaitOne(0, false))
            {
                if (downloadItems.Count == 0)
                    break;

                UpgradeItem item = downloadItems[0];
                ShowCurrentItem(item.Name);

                // 准备下载
                client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                evtPerDonwload.Reset();
                Uri uri = new Uri(upgradeUrl + "/" + item.Name);
                client.DownloadFileAsync(uri, Path.Combine(upgradeTempPath, item.Name), item);
                evtPerDonwload.WaitOne();

                client.Dispose(); // 完成下载
                client = null;
                downloadItems.Remove(item);
            }

            if (downloadItems.Count == 0)
                Exit(true);
            else
                Exit(false);
            eventUpgrade.Set();
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpgradeItem item = e.UserState as UpgradeItem;
            downloadSize += item.Size;
            SetProcessBar(0, (int)(downloadSize * 100 / totalSize));
            evtPerDonwload.Set();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            SetProcessBar(e.ProgressPercentage, (int)((downloadSize + e.BytesReceived) * 100 / totalSize));
        }

        delegate void ShowCurrentItemCallBack(string filename);
        private void ShowCurrentItem(string filename)
        {
            if (this.labCurrent.InvokeRequired)
            {
                ShowCurrentItemCallBack cb = new ShowCurrentItemCallBack(ShowCurrentItem);
                this.Invoke(cb, new object[] { filename });
            }
            else
            {
                labCurrent.Text = "正在下载：" + filename;
            }
        }

        delegate void ExitCallBack(bool success);
        private void Exit(bool success)
        {
            if (this.InvokeRequired)
            {
                ExitCallBack cb = new ExitCallBack(Exit);
                this.Invoke(cb, new object[] { success });
            }
            else
            {
                this.isFinished = success;
                this.DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            }
        }

        delegate void SetProcessBarCallBack(int current, int total);
        private void SetProcessBar(int current, int total)
        {
            if (this.pbCurrent.InvokeRequired)
            {
                SetProcessBarCallBack cb = new SetProcessBarCallBack(SetProcessBar);
                this.Invoke(cb, new object[] { current, total });
            }
            else
            {
                pbCurrent.Text = current.ToString();
                pbTotal.Text = total.ToString();
            }
        }

        private void UpgradeProcess_Load(object sender, EventArgs e)
        {
            eventUpgrade = new ManualResetEvent(true);
            eventUpgrade.Reset();
            Thread thread = new Thread(new ThreadStart(StartDownload));
            thread.Name = "UpgradeProcess";
            thread.Start();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            eventUpgrade.Set();
            evtPerDonwload.Set();
        }
    }
}