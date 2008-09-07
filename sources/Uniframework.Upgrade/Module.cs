using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.Upgrade
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

            ControlledWorkItem<UpgradeController> upgradeWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<UpgradeController>>("UpgradeWorkItem");
            upgradeWorkItem.Run();
        }
    }
}
