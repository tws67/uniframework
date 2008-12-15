/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using System.Windows.Forms;
using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    public partial class WizardOptions : Form
    {
        public WizardOptions()
        {
            InitializeComponent();
        }

        private void WizardOptions_Load(object sender, EventArgs e)
        {
            chkPluralize.Checked = List.AutoPluralize;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List.AutoPluralize = chkPluralize.Checked;
        }
    }
}
