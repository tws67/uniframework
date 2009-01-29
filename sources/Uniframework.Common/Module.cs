using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.Common
{
    /// <summary>
    /// 框架公共程序入口类
    /// </summary>
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

            ControlledWorkItem<CommonController> commonWorkItem = WorkItem.WorkItems.AddNew<ControlledWorkItem<CommonController>>("Uniframework.CommController");
            commonWorkItem.Run();
        }
    }
}
