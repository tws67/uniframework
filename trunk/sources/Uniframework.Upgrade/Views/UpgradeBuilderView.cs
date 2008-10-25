using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Services;
using Uniframework.SmartClient;

namespace Uniframework.Upgrade.Views
{
    public partial class UpgradeBuilderView : DevExpress.XtraEditors.XtraUserControl, ISmartPartInfoProvider
    {
        private bool upgradeItemChanged = false;
        private bool upgradeUrl;

        public UpgradeBuilderView()
        {
            InitializeComponent();
        }

        private UpgradeBuilderPresenter presenter;
        [CreateNew]
        public UpgradeBuilderPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        public void BindingUpgradeGroup(UpgradeGroup group)
        {
            Guard.ArgumentNotNull(group, "Upgrade group.");

            txtGroupName.Text = group.Name;
            txtTarget.Text = group.Target;

            lvItems.Items.Clear();
            foreach (UpgradeItem upItem in group.Items) {
                ListViewItem item = new ListViewItem(upItem.Name, 1);
                item.Tag = upItem;
                lvItems.Items.Add(item);
            }
        }

        public UpgradeProject WrapUpgradeProject()
        {
            UpgradeProject proj = new UpgradeProject();
            proj.StartUpApp = txtStartUpApp.Text.Trim();
            proj.UpgradeServer = txtUpgradeUrl.Text.Trim();
            proj.Product = txtProduct.Text.Trim();
            proj.Version = txtVersion.Text.Trim();
            proj.Description = txtDescription.Text;
            foreach (ListViewItem item in lvGroups.Items)
            {
                UpgradeGroup group = item.Tag as UpgradeGroup;
                if (group != null)
                    proj.Groups.Add(group);
            }
            return proj;
        }

        /// <summary>
        /// 判断当前升级包是否准备好
        /// </summary>
        /// <returns></returns>
        public bool UpgradeProjectIsValid()
        {
            if (String.IsNullOrEmpty(txtStartUpApp.Text))
            {
                XtraMessageBox.Show("未设置升级完成后的启动主程序名字。");
                txtStartUpApp.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtUpgradeUrl.Text))
            {
                XtraMessageBox.Show("未设置保存升级项目的服务器地址。");
                txtUpgradeUrl.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtProduct.Text))
            {
                XtraMessageBox.Show("未设置本次升级项目的软件名称。");
                txtProduct.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtVersion.Text))
            {
                XtraMessageBox.Show("未设置本次升级的软件版本号。");
                txtVersion.Focus();
                return false;
            }

            if (lvGroups.Items.Count < 1)
            {
                XtraMessageBox.Show("没有为本次升级添加相关的升级组。");
                return false;
            }

            UpgradeProject proj = Presenter.UpgradeService.GetUpgradeProject(txtProduct.Text);
            if (proj != null)
            {
                Version newVer = new Version(txtVersion.Text);
                Version oldVer = new Version(proj.Version);
                if (newVer == oldVer)
                {
                    if (XtraMessageBox.Show("服务器已经存在版本为\"" + proj.Version + "\"的升级项目，您依然要上传本次升级项目吗？",
                        "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                        return false;
                }

                if (newVer < oldVer)
                {
                    XtraMessageBox.Show("本次升级的版本号比服务器端的版本低客户端将得不到更新。");
                    return false;
                }
            }
            return true;
        }

        #region Assistant functions

        private void UpdateUpgradeGroup()
        {
            if (String.IsNullOrEmpty(txtGroupName.Text) || String.IsNullOrEmpty(txtTarget.Text))
            {
                XtraMessageBox.Show("必须为更新项目组提供一个名称和目标文件夹位置。");
                txtGroupName.Focus();
                return;
            }
            int index = -1;
            foreach (ListViewItem item in lvGroups.Items)
            {
                if (item.Text == txtGroupName.Text)
                {
                    index = item.Index;
                    lvGroups.Items.Remove(item);
                    break;
                }
            }

            UpgradeGroup group = new UpgradeGroup(txtGroupName.Text.Trim(), txtTarget.Text.Trim());
            foreach (ListViewItem item in lvItems.Items)
                group.Items.Add(item.Tag as UpgradeItem);
            ListViewItem groupItem = new ListViewItem(group.Name, 0);
            groupItem.Tag = group;
            if (index != -1)
                lvGroups.Items.Insert(index, groupItem);
            else
                lvGroups.Items.Add(groupItem);
            upgradeItemChanged = false;
        }

        /// <summary>
        /// Clears the upgrade group detail.
        /// </summary>
        private void ClearUpgradeGroupDetail()
        {
            txtGroupName.Text = "";
            txtTarget.Text = "";
            lvItems.Items.Clear();
            txtGroupName.Focus();
        }

        /// <summary>
        /// 选择升级项目的入口程序
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraEditors.Controls.ButtonPressedEventArgs"/> instance containing the event data.</param>
        private void txtStartUpApp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog opendlg = new OpenFileDialog();
            opendlg.Multiselect = false;
            opendlg.Title = "选择";
            opendlg.Filter = "可执行程序(*.exe)|*.exe";
            if (opendlg.ShowDialog() == DialogResult.OK) {
                txtStartUpApp.Text = Path.GetFileName(opendlg.FileName);
                txtProduct.Text = Path.GetFileNameWithoutExtension(opendlg.FileName);

                // 向升级项目中插入启动组的内容
                UpgradeGroup group = new UpgradeGroup("StartUp", "${RootPath}");
                group.Items.Add(new UpgradeItem(opendlg.FileName));
                foreach (ListViewItem item in lvGroups.Items) {
                    UpgradeGroup gr = item.Tag as UpgradeGroup;
                    if (gr != null && gr.Name == "StartUp") {
                        lvGroups.Items.Remove(item);
                        break;
                    }
                }
                ListViewItem groupItem = new ListViewItem(group.Name);
                groupItem.Tag = group;
                lvGroups.Items.Insert(0, groupItem);
                BindingUpgradeGroup(group);

                try {
                    AssemblyName assemblyName = Assembly.ReflectionOnlyLoadFrom(opendlg.FileName).GetName();
                    txtVersion.Text = assemblyName.Version.ToString();
                }
                catch {
                    UpgradeProject proj = Presenter.UpgradeService.GetUpgradeProject(txtProduct.Text.Trim()); // 从服务器升级数据库中获取新的版本号
                    if (proj != null) {
                        Version ver = new Version(proj.Version);
                        txtVersion.Text = String.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision + 1);
                    }
                    else
                        txtVersion.Text = "1.0.0.0";
                }
            }
            txtUpgradeUrl.Text = Presenter.UpgradeSetting.UpgradeUrl;
            txtUpgradeUrl.SelectAll();
            txtUpgradeUrl.Focus();
        }

        private void EnumDirectory(string path)
        {
            List<string> files = FileUtility.SearchDirectory(path, "*.*");
            foreach (string file in files)
            {
                if (FileUtility.IsDirectory(file))
                    EnumDirectory(file);
                else
                {
                    UpgradeItem upItem = new UpgradeItem(file);
                    ListViewItem item = new ListViewItem(upItem.Name, 1);
                    item.Tag = upItem;
                    lvItems.Items.Add(item);
                }
            }
        }

        #endregion

        private void lvGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvGroups.SelectedItems.Count > 0) {
                UpgradeGroup group = lvGroups.SelectedItems[0].Tag as UpgradeGroup;
                if (group != null)
                    BindingUpgradeGroup(group);
            }
            btngDelete.Enabled = lvGroups.Items.Count > 0;
        }

        private void btniAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog opendlg = new OpenFileDialog();
            opendlg.Multiselect = true;
            opendlg.Title = "选择更新项目";
            opendlg.Filter = "所有程序(*.*)|*.*";
            if (opendlg.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in opendlg.FileNames)
                {
                    if (FileUtility.IsDirectory(file))
                    {
                        EnumDirectory(file);
                    }
                    else
                    {
                        UpgradeItem upItem = new UpgradeItem(file);
                        ListViewItem item = new ListViewItem(upItem.Name, 1);
                        item.Tag = upItem;
                        lvItems.Items.Add(item);
                    }
                }
                upgradeItemChanged = true;
            }
        }

        private void btniDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (lvItems.SelectedItems.Count == 0)
                return;

            foreach (ListViewItem item in lvItems.SelectedItems)
                lvItems.Items.Remove(item);
            upgradeItemChanged = true;
        }

        private void lvItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            btniDelete.Enabled = lvItems.Items.Count > 0;
        }

        private void btniRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UpdateUpgradeGroup();
        }

        private void btngAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UpdateUpgradeGroup();
            ClearUpgradeGroupDetail();
            txtTarget.Text = "${RootPath}";
        }

        private void btngDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (lvGroups.SelectedItems.Count <= 0)
                return;

            lvGroups.Items.RemoveAt(0);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Presenter.UploadProject();
        }

        public Microsoft.Practices.CompositeUI.SmartParts.ISmartPartInfo GetSmartPartInfo(Type smartPartInfoType)
        {
            // Implement ISmartPartInfoProvider in the containing smart part. Required in order for contained smart part infos to work.
            Microsoft.Practices.CompositeUI.SmartParts.ISmartPartInfoProvider ensureProvider = this;
            return this.infoProvider.GetSmartPartInfo(smartPartInfoType);

        }

        #region ISmartPartInfoProvider Members

        ISmartPartInfo ISmartPartInfoProvider.GetSmartPartInfo(Type smartPartInfoType)
        {
            ISmartPartInfoProvider ensureProvider = this;
            return this.infoProvider.GetSmartPartInfo(smartPartInfoType);
        }

        #endregion
    }
}
