using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Microsoft.Practices.CompositeUI.Common.Workspaces
{
    public class MdiWorkspace : WindowWorkspace
    {
        private Form parentMdiForm;

        /// <summary>
        /// Constructor specifying the parent form of the MDI child.
        /// </summary>
        public MdiWorkspace(Form parentForm)
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
        protected override void OnShow(Control smartPart, WindowSmartPartInfo smartPartInfo)
        {
            Form mdiChild = this.GetOrCreateForm(smartPart);
            mdiChild.MdiParent = parentMdiForm;

            this.SetWindowProperties(mdiChild, smartPartInfo);
            mdiChild.Show();
            this.SetWindowLocation(mdiChild, smartPartInfo);
            mdiChild.BringToFront();
        }
    }
}
