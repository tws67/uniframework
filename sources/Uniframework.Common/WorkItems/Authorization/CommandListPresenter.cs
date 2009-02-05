using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniframework.SmartClient;
using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.Common.WorkItems.Authorization
{
    public class CommandListPresenter : DataListPresenter<CommandListView>
    {
        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        public override void Insert()
        {
            base.Insert();

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo() { 
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                ShowInTaskbar = false,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterParent,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            };

            ShowViewInWorkspace<CommandView>(SmartPartNames.AuthorizationCommandView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
        }

        public override bool CanEdit
        {
            get
            {
                bool flag = base.CanEdit;
                return flag;
            }
        }

        
    }
}
