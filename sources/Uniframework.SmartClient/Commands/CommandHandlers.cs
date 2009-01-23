using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.XtraForms.SmartPartInfos;
using Uniframework.XtraForms.Workspaces;
using Microsoft.Practices.CompositeUI.WinForms;
using Uniframework.SmartClient.Views;
using DevExpress.XtraEditors;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 框架通用的操作命令项
    /// </summary>
    public class CommandHandlers : Controller
    {
        /// <summary>
        /// 单击测试
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_CLICKME)]
        public void OnUniframeworkClickMe(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Command name " + ((Command)sender).Name);
            sb.AppendLine("Hi, " + Thread.CurrentPrincipal.Identity.Name + " you click me at " + DateTime.Now.ToLocalTime() + " !!!");
            XtraMessageBox.Show(sb.ToString());
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_FILE_EXIT)]
        public void OnUniframeworkExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 显示关于窗口
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_HELP_ABOUT)]
        public void OnUniframeworkAbout(object sender, EventArgs e)
        {
            frmAbout form = WorkItem.SmartParts.AddNew<frmAbout>();
            form.ShowDialog();
            WorkItem.SmartParts.Remove(form);
        }

        /// <summary>
        /// 系统设置
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_VIEW_SETTING)]
        public void OnUniframeworkSetting(object sender, EventArgs e)
        {
            SettingView view = WorkItem.SmartParts.Get<SettingView>(SmartPartNames.SmartPart_Shell_SettingView);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<SettingView>(SmartPartNames.SmartPart_Shell_SettingView);

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo
            {
                Title = "选项",
                MaximizeBox = false,
                MinimizeBox = false,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Modal = true,
                Icon = ImageService.GetIcon("preferences", new System.Drawing.Size(16, 16))
            };
            IWorkspace wp = new XtraWindowWorkspace();
            wp.Show(view, spi);
        }

        /// <summary>
        /// 显示动态帮助视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_HELP_DYNAMICHELP)]
        public void OnShowDynamicHelp(object sender, EventArgs e)
        {
            DynamicHelpView view = WorkItem.SmartParts.Get<DynamicHelpView>(SmartPartNames.SmartPart_Shell_DynamicHelp);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<DynamicHelpView>(SmartPartNames.SmartPart_Shell_DynamicHelp);

            IWorkspace wp = WorkItem.Workspaces[UIExtensionSiteNames.Shell_Workspace_Dockable];
            if (wp != null)
            {
                DockManagerSmartPartInfo spi = new DockManagerSmartPartInfo();
                spi.Title = "动态帮助";
                spi.ImageFile = "${help}";
                spi.Tabbed = true;
                spi.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;

                wp.Show(view, spi);
            }
        }

        /// <summary>
        /// 显示快速链接视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_VIEW_TASKBAR)]
        public void OnShowTaskbar(object sender, EventArgs e)
        {
            TaskbarView view = WorkItem.SmartParts.Get<TaskbarView>(SmartPartNames.SmartPart_Shell_TaskbarView);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<TaskbarView>(SmartPartNames.SmartPart_Shell_TaskbarView);

            IWorkspace wp = WorkItem.Workspaces[UIExtensionSiteNames.Shell_Workspace_Dockable];
            if (wp != null) {
                DockManagerSmartPartInfo spi = new DockManagerSmartPartInfo();
                spi.Title = "关联任务";
                spi.ImageFile = "${magic-wand}";
                spi.Tabbed = true;
                spi.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;

                wp.Show(view, spi);
            }
        }

        /// <summary>
        /// 显示系统用户管理视图
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_COMM_MEMBERSHIPUSER)]
        public void OnShowMembershipUser(object sender, EventArgs e)
        {
            MembershipUserView view = WorkItem.SmartParts.Get<MembershipUserView>(SmartPartNames.SmartPart_Shell_MembershipUserView);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<MembershipUserView>(SmartPartNames.SmartPart_Shell_MembershipUserView);

            IWorkspace wp = WorkItem.Workspaces[UIExtensionSiteNames.Shell_Workspace_Main];
            if (wp != null) {
                WindowSmartPartInfo spi = new WindowSmartPartInfo();
                spi.Title = "用户管理";
                wp.Show(view, spi);
            }
        }

        #region Dependency services

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IUIExtensionService UIExtensionService
        {
            get;
            set;
        }

        [ServiceDependency]
        public ITaskbarService TaskbarService
        {
            get;
            set;
        }

        #endregion
    }
}
