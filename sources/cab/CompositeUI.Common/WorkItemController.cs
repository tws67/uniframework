using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

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
    }
}
