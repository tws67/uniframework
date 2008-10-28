using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Base class for a WorkItem controller.
    /// </summary>
    public class WorkItemController : IWorkItemController
    {
        private WorkItem workItem;

        /// <summary>
        /// Gets or sets the work item.
        /// </summary>
        /// <value>The work item.</value>
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set { workItem = value; }
        }
        
        /// <summary>
        /// Called when the controller is ready to run.
        /// </summary>
        public virtual void OnRunStarted()
        {
            AddServices();
            AddViews();
            AddUIElements();
        }

        /// <summary>
        /// Add custom services on this controller
        /// </summary>
        protected virtual void AddServices()
        { }

        /// <summary>
        /// Add custom views on this controller
        /// </summary>
        protected virtual void AddViews()
        { }

        /// <summary>
        /// Add custom uielements on this workitem or rootworkitem
        /// </summary>
        protected virtual void AddUIElements()
        { }

        /// <summary>
        /// Creates and shows a smart part on the specified workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the smart part to create and show.</typeparam>
        /// <param name="workspaceName">The name of the workspace in which to show the smart part.</param>
        /// <returns>The new smart part instance.</returns>
        protected virtual TView ShowViewInWorkspace<TView>(string workspaceName)
        {
            TView view = WorkItem.SmartParts.AddNew<TView>();
            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view);
            return view;
        }

        /// <summary>
        /// Shows the view in workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="workspaceName">Name of the workspace.</param>
        /// <param name="spi">The spi.</param>
        /// <returns></returns>
        protected virtual TView ShowViewInWorkspace<TView>(string workspaceName, ISmartPartInfo spi)
        {
            TView view = WorkItem.SmartParts.AddNew<TView>();
            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view, spi);
            return view;
        }

        /// <summary>
        /// Shows a specific smart part in the workspace. If a smart part with the specified id
        /// is not found in the <see cref="WorkItem.SmartParts"/> collection, a new instance
        /// will be created; otherwise, the existing instance will be re used.
        /// </summary>
        /// <typeparam name="TView">The type of the smart part to show.</typeparam>
        /// <param name="viewId">The id of the smart part in the <see cref="WorkItem.SmartParts"/> collection.</param>
        /// <param name="workspaceName">The name of the workspace in which to show the smart part.</param>
        /// <returns>The smart part instance.</returns>
        protected virtual TView ShowViewInWorkspace<TView>(string viewId, string workspaceName)
        {
            TView view = default(TView);
            if (WorkItem.SmartParts.Contains(viewId)) {
                view = WorkItem.SmartParts.Get<TView>(viewId);
            }
            else {
                view = WorkItem.SmartParts.AddNew<TView>(viewId);
            }

            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view);

            return view;
        }

        /// <summary>
        /// Shows the view in workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="viewId">The view id.</param>
        /// <param name="workspaceName">Name of the workspace.</param>
        /// <param name="spi">The spi.</param>
        /// <returns></returns>
        protected virtual TView ShowViewInWorkspace<TView>(string viewId, string workspaceName, ISmartPartInfo spi)
        {
            TView view = default(TView);
            if (WorkItem.SmartParts.Contains(viewId)) {
                view = WorkItem.SmartParts.Get<TView>(viewId);
            }
            else {
                view = WorkItem.SmartParts.AddNew<TView>(viewId);
            }

            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view, spi);

            return view;
        }
    }
}
