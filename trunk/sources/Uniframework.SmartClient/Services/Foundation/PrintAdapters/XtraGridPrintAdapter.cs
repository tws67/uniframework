using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using Microsoft.Practices.CompositeUI;
using System.Windows.Forms;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// DevExpress 表格控件打印适配器
    /// </summary>
    public class XtraGridPrintAdapter : IPrintHandler
    {
        private GridControl adaptee;
        private WorkItem workItem;
        private XtraPrintService xtrapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraGridPrintAdapter"/> class.
        /// </summary>
        /// <param name="adaptee">The adaptee.</param>
        /// <param name="workItem">The work item.</param>
        public XtraGridPrintAdapter(GridControl adaptee, WorkItem workItem)
        {
            this.adaptee = adaptee;
            this.workItem = workItem;
            xtrapService = new XtraPrintService(adaptee, adaptee.DefaultView.ViewCaption);

            RegisterEvent();
        }

        #region Assistant functions

        private void RegisterEvent()
        {
            adaptee.Enter += new EventHandler(AdapterEnter);
            adaptee.Leave += new EventHandler(AdapterLeave);
        }

        private void AdapterEnter(object sender, EventArgs e)
        {
            OnEnter(e);
        }

        private void OnEnter(EventArgs e)
        {
            if (Enter != null)
                Enter(this, e);
        }

        private void AdapterLeave(object sender, EventArgs e)
        {
            OnLeave(e);
        }

        private void OnLeave(EventArgs e)
        {
            if (Leave != null)
                Leave(this, e);
        }

        #endregion

        #region IPrintHandler Members

        public event EventHandler Enter;

        public event EventHandler Leave;

        public bool CanPrint
        {
            get { return !(adaptee.DefaultView as ColumnView).IsEmpty; }
        }

        public void Print()
        {
            xtrapService.Print();
        }

        public bool CanQuickPrint
        {
            get { return !(adaptee.DefaultView as ColumnView).IsEmpty; }
        }

        public void QuickPrint()
        {
            xtrapService.QuickPrint();
        }

        public bool CanPreview
        {
            get { return !(adaptee.DefaultView as ColumnView).IsEmpty; }
        }

        public void Preview()
        {
            Guard.ArgumentNotNull(workItem, "workItem");

            Form shell = workItem.Items.Get(UIExtensionSiteNames.Shell) as Form;
            if (shell != null)
                xtrapService.Preview(shell);
            else
                xtrapService.Preview();
        }

        public bool CanPageSetup
        {
            get { return true; }
        }

        public void PageSetup()
        {
            xtrapService.PageSetup();
        }

        public bool CanDesign
        {
            get { return false; }
        }

        public void Design()
        {
        }

        #endregion
    }
}
