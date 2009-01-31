using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI.WinForms;
using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.XtraForms.Workspaces
{
    public class XtraMdiWorkspace : XtraWindowWorkspace
    {
        private Form parentMdiForm;

        /// <summary>
        /// Constructor specifying the parent form of the MDI child.
        /// </summary>
        public XtraMdiWorkspace(Form parentForm)
            : base()
        {
            this.parentMdiForm = parentForm;
            this.parentMdiForm.IsMdiContainer = true;
        }

        /// <summary>
        /// Gets the parent MDI form.
        /// </summary>
        public Form ParentMdiForm
        {
            get { return parentMdiForm; }
        }

        /// <summary>
        /// Shows the form as a child of the specified <see cref="ParentMdiForm"/>.
        /// </summary>
        /// <param name="smartPart">The <see cref="Control"/> to show in the workspace.</param>
        /// <param name="smartPartInfo">The information to use to show the smart part.</param>
        protected override void OnShow(Control smartPart, XtraWindowSmartPartInfo spi)
        {
            Form child = GetOrCreateForm(smartPart);
            child.MdiParent = parentMdiForm;

            SetWindowProperties(child, spi);
            child.Show();
            child.BringToFront();
        }
    }
}
