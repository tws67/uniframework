using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.DemoCenter.Client
{
    public class Module : ModuleInit
    {
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        public override void Load()
        {
            base.Load();

            ControlledWorkItem<DemoCenterController> demoCenter = WorkItem.WorkItems.AddNew<ControlledWorkItem<DemoCenterController>>();
            demoCenter.Run();
        }
    }
}
