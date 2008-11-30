using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using DevExpress.XtraTreeList;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// DevExpress tree list 打印适配器
    /// </summary>
    public class XtraTreeListPrintAdapter : IPrintHandler
    {
        private TreeList adaptee;
        private WorkItem workItem;
        private XtraPrintService xtrapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraTreeListPrintAdapter"/> class.
        /// </summary>
        /// <param name="adaptee">The adaptee.</param>
        /// <param name="workItem">The work item.</param>
        public XtraTreeListPrintAdapter(TreeList adaptee, WorkItem workItem)
        {
            this.adaptee = adaptee;
            this.workItem = workItem;
            xtrapService = new XtraPrintService(adaptee);

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
            get { return adaptee.Nodes.Count > 0; }
        }

        public void Print()
        {
            xtrapService.Print();
        }

        public bool CanQuickPrint
        {
            get { return adaptee.Nodes.Count > 0; }
        }

        public void QuickPrint()
        {
            xtrapService.QuickPrint();
        }

        public bool CanPreview
        {
            get { return adaptee.Nodes.Count > 0; }
        }

        public void Preview()
        {
            Form shell = workItem.Items.Get(UIExtensionSiteNames.Shell) as Form;
            if (shell != null)
                xtrapService.Preview(shell);
            else
                xtrapService.Preview(shell);
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
