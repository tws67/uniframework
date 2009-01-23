using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraBars;

namespace Uniframework.SmartClient.Strategies
{
    /// <summary>
    /// 右键菜单策略
    /// </summary>
    public class XtraContentMenuStrategy : BuilderStrategy
    {
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
        private void RegisterMouseEvent(Control control, bool register)
        {
            Guard.ArgumentNotNull(control, "control");

            if (register)
                control.MouseUp += new MouseEventHandler(Control_MouseUp);
            else
                control.MouseUp -= new MouseEventHandler(Control_MouseUp);

            foreach (Control ctrl in control.Controls)
                RegisterMouseEvent(ctrl, register);
        }

        /// <summary>
        /// 控件的鼠标释放事件
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
                        if (content != null)
                            content.ShowPopup(Control.MousePosition);
                    }
                }
            }
        }

        #endregion
    }
}
