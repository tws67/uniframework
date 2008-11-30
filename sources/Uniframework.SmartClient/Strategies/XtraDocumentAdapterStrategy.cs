using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;

namespace Uniframework.SmartClient.Strategies
{
    public class XtraDocumentAdapterStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context, Type typeToBuild, object existing, string idToBuild)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, existing);
            if (workItem != null) {
                IDocumentService documentService = workItem.Services.Get<IDocumentService>();
                if (documentService != null && existing is Control) {
                    RegisterDocumentAdapter(documentService, (Control)existing, true);
                }
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        public override object TearDown(IBuilderContext context, object item)
        {
            WorkItem workItem = StrategyUtility.GetWorkItem(context, item);
            if (workItem != null) {
                IDocumentService documentService = workItem.Services.Get<IDocumentService>();
                if (documentService != null && item is Control) {
                    RegisterDocumentAdapter(documentService, (Control)item, false);
                }
            }
            return base.TearDown(context, item);
        }

        private void RegisterDocumentAdapter(IDocumentService documentService, Control control, bool register)
        {
            Guard.ArgumentNotNull(documentService, "documentService");
            Guard.ArgumentNotNull(control, "control");

            foreach (Control ctrl in control.Controls) { 
                if (ctrl is GridControl || ctrl is TreeList) {
                    if (register)
                        documentService.Register(ctrl);
                    else
                        documentService.UnRegister(ctrl);
                }

                if (ctrl.Controls.Count > 0)
                    RegisterDocumentAdapter(documentService, ctrl, register);
            }
        }

    }
}
