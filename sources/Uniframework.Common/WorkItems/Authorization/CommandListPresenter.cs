using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Uniframework.SmartClient;
using Uniframework.XtraForms.SmartPartInfos;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using Uniframework.Security;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.Common.WorkItems.Authorization
{
    public class CommandListPresenter : DataListPresenter<CommandListView>
    {
        [ServiceDependency]
        public IAuthorizationStoreService AuthorizationStoreService
        {
            get;
            set;
        }

        /// <summary>
        /// 当前命令项
        /// </summary>
        /// <value>The current command.</value>
        public AuthorizationCommand CurrentCommand
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化数据列表操作只在数据列表第一次加载时使用
        /// </summary>
        public override void Initilize()
        {
            base.Initilize();

            RefreshDataSource();
        }
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
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                Title = "新建命令"
            };

            ShowViewInWorkspace<CommandView>(SmartPartNames.AuthorizationCommandView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
        }

        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public override bool CanEdit
        {
            get
            {
                bool flag = base.CanEdit;
                flag &= DefaultView.SelectedRowsCount > 0;
                return flag;
            }
        }

        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public override void Edit()
        {
            base.Edit();

            XtraWindowSmartPartInfo spi = new XtraWindowSmartPartInfo() { 
                MaximizeBox = false,
                MinimizeBox = false,
                Modal = true,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog, 
                Title = "属性"
            };

            ShowViewInWorkspace<CommandView>(SmartPartNames.AuthorizationCommandView, UIExtensionSiteNames.Shell_Workspace_Window, spi);
            RefreshDataSource();
        }

        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.</value>
        public override bool CanDelete
        {
            get
            {
                bool flag = base.CanDelete;
                flag &= DefaultView.SelectedRowsCount > 0;
                return flag;
            }
        }

        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        public override void Delete()
        {
            base.Delete();

            if (XtraMessageBox.Show("您真的要删除当前选定的命令项吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                try {
                    if (CurrentCommand != null)
                        AuthorizationStoreService.DeleteCommand(CurrentCommand);
                }
                catch (Exception ex) {
                    XtraMessageBox.Show("删除命令项失败，" + ex.Message);
                }
            }
            RefreshDataSource();
        }

        /// <summary>
        /// 刷新数据列表视图
        /// </summary>
        public override void RefreshDataSource()
        {
            base.RefreshDataSource();
            using (WaitCursor cursor = new WaitCursor(true)) {
                try {
                    View.DataGrid.BeginUpdate();
                    View.DataSource.DataSource = AuthorizationStoreService.GetCommands();
                }
                finally {
                    View.DataGrid.EndUpdate();
                }
            }
        }

        /// <summary>
        /// 视图准备好方法用于在Presenter中初始化视图
        /// </summary>
        public override void OnViewReady()
        {
            base.OnViewReady();
            Initilize();
        }

        /// <summary>
        /// 默认列表视图
        /// </summary>
        /// <value>The default view.</value>
        private ColumnView DefaultView
        {
            get { return View.DataGrid.FocusedView as ColumnView; }
        }
    }
}
