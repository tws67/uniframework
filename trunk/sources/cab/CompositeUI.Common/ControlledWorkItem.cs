using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Represents a WorkItem that uses a WorkItem controller to perform its business logic.
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    public sealed class ControlledWorkItem<TController> : WorkItem
        where TController : IWorkItemController
    {
        private TController controller;

        /// <summary>
        /// Gets the controller.
        /// </summary>
        public TController Controller
        {
            get { return controller; }
        }

        /// <summary>
        /// See <see cref="M:Microsoft.Practices.ObjectBuilder.IBuilderAware.OnBuiltUp(System.String)"/> for more information.
        /// </summary>
        public override void OnBuiltUp(string id)
        {
            base.OnBuiltUp(id);

            controller = Items.AddNew<TController>();
        }

        protected override void OnRunStarted()
        {
            controller.OnRunStarted();
        }
    }
}
