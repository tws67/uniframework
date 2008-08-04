using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.CompositeUI.Common
{
    public interface IWorkItemController
    {
        /// <summary>
        /// Called when the controller is ready to run.
        /// </summary>
        void OnRunStarted();
    }
}
