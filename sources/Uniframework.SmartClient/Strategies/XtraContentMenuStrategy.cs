using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;

namespace Uniframework.SmartClient.Strategies
{
    /// <summary>
    /// 右键菜单策略
    /// </summary>
    public class XtraContentMenuStrategy : BuilderStrategy
    {
        private static readonly string EMPTYROW = "EmptyRow";
        private WorkItem workItem = null;

        public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
        {
            workItem = StrategyUtility.GetWorkItem(context, existing);
            if (existing is Control)
                RegisterMouseEvent(existing as Control, true);

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        public override object TearDown(IBuilderContext context, object item)
        {
            if (item is Control)
                RegisterMouseEvent(item as Control, false);

            return base.TearDown(context, item);
        }

        #region Assistant functions

        /// <summary>
        /// 注册控件的鼠标事件
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="register">if set to <c>true</c> [register].</param>
        private void RegisterMouseEvent(Control ctrl, bool register)
        {
            Guard.ArgumentNotNull(ctrl, "control");

            if (register) {
                ctrl.MouseUp += new MouseEventHandler(Control_MouseUp);

                // 设置数据表格的BarManager以使相关的下拉菜单呈现相同的样式
                BarManager barManager = workItem.RootWorkItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
                if (barManager != null) {
                    if (ctrl is GridControl) // 表格控件
                        ((GridControl)ctrl).MenuManager = barManager;
                    if (ctrl is TreeList)    // 树视图
                        ((TreeList)ctrl).MenuManager = barManager;
                    if (ctrl is LayoutControl) // 布局控件
                        ((LayoutControl)ctrl).MenuManager = barManager;
                }
            }
            else
                ctrl.MouseUp -= new MouseEventHandler(Control_MouseUp);

            // 递归注册所有组件
            foreach (Control control in ctrl.Controls)
                RegisterMouseEvent(control, register);
        }

        /// <summary>
        /// 控件的鼠标释放事件在这里处理架构右键菜单
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (workItem != null && e.Button == MouseButtons.Right) {
                Control control = sender as Control;
                if (control != null) { 
                    string exPath = (string)control.Tag;
                    IContentMenuService contentService = workItem.Services.Get<IContentMenuService>(); // 上下文菜单服务
                    if (!String.IsNullOrEmpty(exPath) && contentService != null) {
                        PopupMenu content = contentService.GetContentMenu(exPath) as PopupMenu;
                        if (content != null) {
                            // DevExpress TreeList 控件
                            if (control is TreeList) {
                                TreeListHitInfo hi = ((TreeList)control).CalcHitInfo(new Point(e.X, e.Y));
                                if (hi.HitInfoType == HitInfoType.Row || hi.HitInfoType == HitInfoType.Cell || hi.HitInfoType == HitInfoType.Empty)
                                    content.ShowPopup(Control.MousePosition);
                            }
                            else if (control is GridControl) { // DevExpress GridControl控件
                                GridHitInfo hi = ((GridControl)control).MainView.CalcHitInfo(new Point(e.X, e.Y)) as GridHitInfo;
                                if (hi != null) {
                                    string hitTest = hi.HitTest.ToString();
                                    if (hi.InRow || hi.InRowCell || hitTest == EMPTYROW)
                                        content.ShowPopup(Control.MousePosition);
                                }
                                else
                                    content.ShowPopup(Control.MousePosition);
                            }
                            else
                                content.ShowPopup(Control.MousePosition);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
