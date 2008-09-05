using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.WinForms;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    public class SettingPresenter : Presenter<SettingView>
    {
        [ServiceDependency]
        public ISettingService SettingService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public ISetting Current { get; private set; }

        [EventSubscription(EventNames.Uniframework_SettingViewChanged)]
        public void OnShowSettingView(object sender, EventArgs<ISetting> e)
        {
            if (e.Data != null) {
                Current = e.Data;
            }
        }

        /// <summary>
        /// 保存所有设置项目
        /// </summary>
        public void Save()
        {
            SettingService.Save();
        }

        /// <summary>
        /// 为当前设置项目加载默认的设置值
        /// </summary>
        public void LoadDefault()
        {
            if (Current != null)
                SettingService.LoadDefault(Current);
        }

        /// <summary>
        /// 显示系统外壳样式设置面板
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_SHOW_SHELLL_AYOUTSETTING)]
        public void OnShowShellLayoutSetting(object sender, EventArgs e)
        {
            ShellLayoutSettingView view = WorkItem.SmartParts.Get<ShellLayoutSettingView>(SmartPartNames.SmartPart_Shell_LayoutSettingView);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<ShellLayoutSettingView>(SmartPartNames.SmartPart_Shell_LayoutSettingView);

            IWorkspace wp = WorkItem.Workspaces.Get<DeckWorkspace>(UIExtensionSiteNames.Shell_Workspace_SettingDeck);
            if (wp != null)
            {
                wp.Show(view);
                view.BindingProperty();
                SettingService.RaiseSettingViewChanged(view);
            }
        }
    }
}
