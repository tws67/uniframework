using System;
using System.Collections.Generic;
using System.Text;

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.ModelBuilder;

namespace Uniframework.Services.Facilities
{
    public class SystemComponentInspector : IContributeComponentModelConstruction
    {
        private ISystemService systemManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemComponentInspector"/> class.
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        public SystemComponentInspector(ISystemService systemManager)
        {
            this.systemManager = systemManager;
        }

        #region IContributeComponentModelConstruction Members

        /// <summary>
        /// Usually the implementation will look in the configuration property
        /// of the model or the service interface, or the implementation looking for
        /// something.
        /// </summary>
        /// <param name="kernel">The kernel instance</param>
        /// <param name="model">The component model</param>
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            systemManager.InspectService(model.Service);
        }

        #endregion
    }
}
