using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraTreeList;

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
                    RegisterPrintableAdapter(printableService, (Control)existing, true);
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
                    RegisterPrintableAdapter(printableService, (Control)item, false);
            }

            return base.TearDown(context, item);
        }

        private void RegisterPrintableAdapter(IPrintableService printableService, Control control, bool register)
        {
            Guard.ArgumentNotNull(printableService, "printableService");
            Guard.ArgumentNotNull(control, "control");

            foreach (Control ctrl in control.Controls) {
                if (ctrl is GridControl | ctrl is TreeList) {
                    if (register)
                        printableService.Register(ctrl);
                    else
                        printableService.UnRegister(ctrl);
                }

                if (ctrl.Controls.Count > 0)
                    RegisterPrintableAdapter(printableService, ctrl, register);
            }
        }

    }
}
