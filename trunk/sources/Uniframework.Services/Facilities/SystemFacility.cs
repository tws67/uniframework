using System;
using System.Collections.Generic;
using System.Text;

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;

namespace Uniframework.Services.Facilities
{
    /// <summary>
    /// 系统扩展块
    /// </summary>
    public class SystemFacility : AbstractFacility
    {
        /// <summary>
        /// 初始化方法
        /// </summary>
        protected override void Init()
        {
            Kernel.AddComponent("SystemService", typeof(ISystemService), typeof(SystemService));
            SystemComponentInspector inspector = new SystemComponentInspector(Kernel[typeof(ISystemService)] as ISystemService);
            Kernel.ComponentModelBuilder.AddContributor(inspector);
        }
    }
}
