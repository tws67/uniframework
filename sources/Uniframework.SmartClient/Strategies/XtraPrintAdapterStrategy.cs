using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.SmartClient.Strategies
{
    public class XtraPrintAdapterStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, existing);
            if (workItem != null) {
                IPrintableService printableService = workItem.Services.Get<IPrintableService>();
                if (printableService != null && existing is Control)
                    RegisterPrintableAdapter(workItem, printableService, (Control)existing, true);
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }


        public override object TearDown(IBuilderContext context, object item)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, item);
            if (workItem != null)
            {
                IPrintableService printableService = workItem.Services.Get<IPrintableService>();
                if (printableService != null && item is Control)
                    RegisterPrintableAdapter(workItem, printableService, (Control)item, false);
            }

            return base.TearDown(context, item);
        }

        private void RegisterPrintableAdapter(WorkItem workItem, IPrintableService printableService, Control control, bool register)
        {
            Guard.ArgumentNotNull(printableService, "printableService");
            Guard.ArgumentNotNull(control, "control");

            foreach (Control ctrl in control.Controls) {
                if (ctrl is GridControl | ctrl is TreeList) {
                    if (register)
                        printableService.Register(ctrl);
                    else
                        printableService.UnRegister(ctrl);

                    // 设置数据表格的BarManager以使相关的下拉菜单呈现相同的样式
                    if (ctrl is GridControl && register) {
                        BarManager barManager = workItem.RootWorkItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
                        if (barManager != null)
                            ((GridControl)ctrl).MenuManager = barManager;
                    }
                }

                if (ctrl.Controls.Count > 0)
                    RegisterPrintableAdapter(workItem, printableService, ctrl, register);
            }
        }

    }
}
