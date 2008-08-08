using System;
using System.Collections.Generic;
using System.Text;

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;

namespace Uniframework.Services.Facilities
{
    /// <summary>
    /// ϵͳ��չ��
    /// </summary>
    public class SystemFacility : AbstractFacility
    {
        /// <summary>
        /// ��ʼ������
        /// </summary>
        protected override void Init()
        {
            Kernel.AddComponent("SystemService", typeof(ISystemService), typeof(SystemService));
            SystemComponentInspector inspector = new SystemComponentInspector(Kernel[typeof(ISystemService)] as ISystemService);
            Kernel.ComponentModelBuilder.AddContributor(inspector);
        }
    }
}
