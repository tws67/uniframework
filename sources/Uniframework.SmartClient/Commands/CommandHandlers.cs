using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Uniframework.SmartClient.WorkItems.Setting;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.XtraForms.Workspaces;
using Uniframework.XtraForms.SmartPartInfos;

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
        public void OnCommandClick(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Command name " + ((Command)sender).Name);
            sb.AppendLine("Hi, " + Thread.CurrentPrincipal.Identity.Name + " you click me at " + DateTime.Now.ToLocalTime());
            MessageBox.Show(sb.ToString());
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_EXIT)]
        public void OnUnframeworkExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 系统设置
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_SETTING)]
        public void OnUnframeworkSetting(object sender, EventArgs e)
        {
            WorkItem workItem = WorkItem.WorkItems.Get(WorkItemNames.Uniframework_Setting);
            if (workItem != null) {
                SettingView view = workItem.SmartParts.Get<SettingView>(SmartPartNames.SmartPart_Shell_SettingView);
                if (view == null)
                    view = workItem.SmartParts.AddNew<SettingView>(SmartPartNames.SmartPart_Shell_SettingView);

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
        }

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }
    }
}
