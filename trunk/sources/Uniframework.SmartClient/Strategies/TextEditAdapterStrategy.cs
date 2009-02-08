using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

using DevExpress.XtraEditors;

namespace Uniframework.SmartClient.Strategies
{
    /// <summary>
    /// 
    /// </summary>
    public class TextEditAdapterStrategy : BuilderStrategy
    {
        /// <summary>
        /// See <see cref="IBuilderStrategy.BuildUp"/> for more information.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="typeToBuild"></param>
        /// <param name="existing"></param>
        /// <param name="idToBuild"></param>
        /// <returns></returns>
        public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, existing);
            if (workItem != null) {
                IEditableService editableService = workItem.Services.Get<IEditableService>();
                if (editableService != null && existing is Control)
                    RegisterTextEdit(editableService, (Control)existing, true);
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        /// <summary>
        /// See <see cref="IBuilderStrategy.TearDown"/> for more information.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public override object TearDown(IBuilderContext context, object item)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, item);
            if (workItem != null) {
                IEditableService editableService = workItem.Services.Get<IEditableService>();
                if (editableService != null && item is Control)
                    RegisterTextEdit(editableService, (Control)item, false);
            }
            return base.TearDown(context, item);
        }

        /// <summary>
        /// Registers the text edit.
        /// </summary>
        /// <param name="editableService">The editable service.</param>
        /// <param name="control">The control.</param>
        /// <param name="register">if set to <c>true</c> [register].</param>
        private void RegisterTextEdit(IEditableService editableService, Control control, bool register)
        {
            Guard.ArgumentNotNull(editableService, "editableService");
            Guard.ArgumentNotNull(control, "control");

            foreach (Control ctrl in control.Controls) {
                if (ctrl is TextEdit)
                {
                    if (register) {
                        editableService.Register(ctrl);
                        ((TextEdit)ctrl).EnterMoveNextControl = true; // 设置可以通过Enter键将焦点移到下一控件
                    }
                    else
                        editableService.UnRegister(ctrl);
                }

                if (ctrl.Controls.Count > 0)
                    RegisterTextEdit(editableService, ctrl, register);
            }
        }
    }
}
