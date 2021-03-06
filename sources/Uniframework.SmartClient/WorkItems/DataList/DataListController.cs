﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Services;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表控制器
    /// </summary>
    public class DataListController : WorkItemController
    {
        private List<object> smartParts = new List<object>();

        #region Dependency services

        [ServiceDependency]
        public ILayoutService LayoutService
        {
            get;
            set;
        }

        #endregion

        protected override void AddServices()
        {
            base.AddServices();
            
            // 注册WorkSpace的事件
            MdiWorkspace mdiWp = WorkItem.Workspaces[UIExtensionSiteNames.Shell_Workspace_Main] as MdiWorkspace;
            if (mdiWp != null) {
                mdiWp.SmartPartActivated += new EventHandler<WorkspaceEventArgs>(DataList_SmartPartActivated);
                mdiWp.SmartPartClosing += new EventHandler<WorkspaceCancelEventArgs>(DataList_SmartPartClosing);
            }

            DockManagerWorkspace dockWp = WorkItem.Workspaces[UIExtensionSiteNames.Shell_Workspace_Dockable] as DockManagerWorkspace;
            if (dockWp != null) { 
                dockWp.SmartPartActivated +=new EventHandler<WorkspaceEventArgs>(DataList_SmartPartActivated);
                dockWp.SmartPartClosing += new EventHandler<WorkspaceCancelEventArgs>(DataList_SmartPartClosing);
            }
        }

        /// <summary>
        /// 数据列表关闭事件
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Practices.CompositeUI.SmartParts.WorkspaceCancelEventArgs"/> instance containing the event data.</param>
        private void DataList_SmartPartClosing(object sender, WorkspaceCancelEventArgs e)
        {
            AuthorizationAttribute[] attrs = (AuthorizationAttribute[])e.SmartPart.GetType().GetCustomAttributes(typeof(AuthorizationAttribute), true);
            if (attrs.Length > 0) {
                try {
                    StoreLayout((Control)e.SmartPart, attrs[0]);
                }
                catch { }
            }
        }

        /// <summary>
        /// 数据列表激活事件
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Practices.CompositeUI.SmartParts.WorkspaceEventArgs"/> instance containing the event data.</param>
        private void DataList_SmartPartActivated(object sender, WorkspaceEventArgs e)
        {
            if (!smartParts.Contains(e.SmartPart)) {
                AuthorizationAttribute[] attrs = (AuthorizationAttribute[])e.SmartPart.GetType().GetCustomAttributes(typeof(AuthorizationAttribute), true);
                if (attrs.Length > 0) {
                    try {
                        ResotreLayout((Control)e.SmartPart, attrs[0]);
                    }
                    catch { }
                }
                smartParts.Add(e.SmartPart);
            }    
        }

        /// <summary>
        /// 保存列表控件的布局信息
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="attr">授权属性</param>
        private void StoreLayout(Control control, AuthorizationAttribute attr)
        {
            // 表格控件
            if (control is GridControl) {
                foreach (BaseView view in ((GridControl)control).Views) {
                    using (MemoryStream stream = new MemoryStream()) {
                        view.SaveLayoutToStream(stream);
                        Layout layout = new Layout(Thread.CurrentPrincipal.Identity.Name, ConbinPath(attr.AuthorizationUri, view.Name));
                        layout.Data = stream.ToArray();
                        LayoutService.StoreLayout(layout); // 保存布局
                    }
                }
            }

            // 列表控件
            if (control is TreeList) {
                using (MemoryStream stream = new MemoryStream()) {
                    TreeList list = (TreeList)control;
                    list.SaveLayoutToStream(stream);
                    Layout layout = new Layout(Thread.CurrentPrincipal.Identity.Name, ConbinPath(attr.AuthorizationUri, list.Name));
                    layout.Data = stream.ToArray();
                    LayoutService.StoreLayout(layout); // 保存布局
                }
            }

            // 递归处理所有的列表控件
            foreach (Control ctrl in control.Controls) {
                StoreLayout(ctrl, attr);
            }
        }

        /// <summary>
        /// 恢复列表控件的布局信息
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="attr">授权属性</param>
        private void ResotreLayout(Control control, AuthorizationAttribute attr)
        {
            // 表格控件
            if (control is GridControl) { 
                GridControl grid = (GridControl)control;
                foreach (BaseView view in grid.Views) {
                    Layout layout = LayoutService.RestoreLayout(Thread.CurrentPrincipal.Identity.Name, ConbinPath(attr.AuthorizationUri, view.Name));
                    if (layout != null)
                        view.RestoreLayoutFromStream(new MemoryStream(layout.Data));
                }
            }

            // 列表控件
            if (control is TreeList) {
                TreeList list = (TreeList)control;
                Layout layout = LayoutService.RestoreLayout(Thread.CurrentPrincipal.Identity.Name, ConbinPath(attr.AuthorizationUri, list.Name));
                list.RestoreLayoutFromStream(new MemoryStream(layout.Data));
            }

            // 递归处理所有的列表控件
            foreach (Control ctrl in control.Controls)
                ResotreLayout(ctrl, attr);
        }

        /// <summary>
        /// 连接两个路径
        /// </summary>
        /// <param name="path1">路径1</param>
        /// <param name="path2">路径2</param>
        /// <returns></returns>
        private string ConbinPath(string path1, string path2)
        {
            return path1.EndsWith("/") ? path1 + path2 : path1 + "/" + path2;
        }
    }
}
