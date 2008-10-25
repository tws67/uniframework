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
        /// �жϵ�ǰ�������Ƿ�׼����
        /// </summary>
        /// <returns></returns>
        public bool UpgradeProjectIsValid()
        {
            if (String.IsNullOrEmpty(txtStartUpApp.Text))
            {
                XtraMessageBox.Show("δ����������ɺ���������������֡�");
                txtStartUpApp.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtUpgradeUrl.Text))
            {
                XtraMessageBox.Show("δ���ñ���������Ŀ�ķ�������ַ��");
                txtUpgradeUrl.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtProduct.Text))
            {
                XtraMessageBox.Show("δ���ñ���������Ŀ��������ơ�");
                txtProduct.Focus();
                return false;
            }

            if (String.IsNullOrEmpty(txtVersion.Text))
            {
                XtraMessageBox.Show("δ���ñ�������������汾�š�");
                txtVersion.Focus();
                return false;
            }

            if (lvGroups.Items.Count < 1)
            {
                XtraMessageBox.Show("û��Ϊ�������������ص������顣");
                return false;
            }

            UpgradeProject proj = Presenter.UpgradeService.GetUpgradeProject(txtProduct.Text);
            if (proj != null)
            {
                Version newVer = new Version(txtVersion.Text);
                Version oldVer = new Version(proj.Version);
                if (newVer == oldVer)
                {
                    if (XtraMessageBox.Show("�������Ѿ����ڰ汾Ϊ\"" + proj.Version + "\"��������Ŀ������ȻҪ�ϴ�����������Ŀ��",
                        "��ʾ", MessageBoxButtons.YesNo) == DialogResult.No)
                        return false;
                }

                if (newVer < oldVer)
                {
                    XtraMessageBox.Show("���������İ汾�űȷ������˵İ汾�Ϳͻ��˽��ò������¡�");
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
                XtraMessageBox.Show("����Ϊ������Ŀ���ṩһ�����ƺ�Ŀ���ļ���λ�á�");
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
        /// ѡ��������Ŀ����ڳ���
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraEditors.Controls.ButtonPressedEventArgs"/> instance containing the event data.</param>
        private void txtStartUpApp_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog opendlg = new OpenFileDialog();
            opendlg.Multiselect = false;
            opendlg.Title = "ѡ��";
            opendlg.Filter = "��ִ�г���(*.exe)|*.exe";
            if (opendlg.ShowDialog() == DialogResult.OK) {
                txtStartUpApp.Text = Path.GetFileName(opendlg.FileName);
                txtProduct.Text = Path.GetFileNameWithoutExtension(opendlg.FileName);

                // ��������Ŀ�в��������������
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
                    UpgradeProject proj = Presenter.UpgradeService.GetUpgradeProject(txtProduct.Text.Trim()); // �ӷ������������ݿ��л�ȡ�µİ汾��
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
            opendlg.Title = "ѡ�������Ŀ";
            opendlg.Filter = "���г���(*.*)|*.*";
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
