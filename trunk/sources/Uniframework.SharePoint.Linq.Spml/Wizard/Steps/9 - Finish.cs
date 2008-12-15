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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// Finish step of the entity generator wizard.
    /// </summary>
    partial class Finish : UserControl, IWizardStep
    {
        #region Private members

        /// <summary>
        /// Wizard context.
        /// </summary>
        private WizardContext ctx;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the finish step.
        /// </summary>
        /// <param name="context">Wizard context.</param>
        public Finish(WizardContext context)
        {
            this.ctx = context;

            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Title of the wizard step.
        /// </summary>
        public string Title
        {
            get { return "Entity mapping generation completed"; }
        }

        /// <summary>
        /// Indicates whether the step allows to go to the next step currently.
        /// </summary>
        public bool CanNext
        {
            get { return false; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the state of the step has changed, e.g. when CanNext has changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Event raised when the step is performing work.
        /// </summary>
        public event EventHandler Working;

        /// <summary>
        /// Event raised when the step has completed its work.
        /// </summary>
        public event EventHandler WorkCompleted;

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raises the Working event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OnWorking()
        {
            if (Working != null)
                Working(this, new EventArgs());
        }

        /// <summary>
        /// Raises the WorkCompleted event.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OnWorkCompleted()
        {
            if (WorkCompleted != null)
                WorkCompleted(this, new EventArgs());
        }

        #endregion

        #region Event handlers

        private void Finish_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            //
            // Connection info.
            //
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, 
                Strings.Connect, 
                ctx.ResultContext.Connection.Url,
                ctx.ResultContext.Connection.CustomAuthentication ? Strings.CustomCredentials : Strings.DefaultNetworkCredentials
                ));
            sb.AppendLine();

            //
            // Lists.
            //
            foreach (List list in ctx.ResultContext.Lists)
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, Strings.GenerateEntity, list.Name));

            //
            // Context (optional).
            //
            if (!string.IsNullOrEmpty(ctx.ResultContext.Name))
            {
                sb.AppendLine();
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, Strings.GenerateContext, ctx.ResultContext.Name));
            }
            
            txtSummary.Text = sb.ToString();
            txtSummary.Select(0, 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Instructs the step to try to cancel a pending operation. This will try to stop a pending background connection operation, if any.
        /// </summary>
        public void Cancel()
        {
        }

        #endregion
    }
}
