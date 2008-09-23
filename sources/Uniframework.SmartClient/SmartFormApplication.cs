using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Common.Services;
using Microsoft.Practices.CompositeUI.Services;

using Uniframework.Db4o;
using Uniframework.SmartClient.WorkItems.Setting;
using Uniframework.XtraForms;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 智能客户端应用程序
    /// </summary>
    /// <typeparam name="TWorkItem">The type of the work item.</typeparam>
    /// <typeparam name="TShell">The type of the shell.</typeparam>
    public class SmartFormApplication<TWorkItem, TShell> : XtraFormApplication<TWorkItem, TShell>
        where TWorkItem : WorkItem, new()
        where TShell : Form
    {
        /// <summary>
        /// Must be overriden. This method is called when the application is fully created and
        /// ready to run.
        /// </summary>
        protected override void Start() {
        }

        /// <summary>
        /// 添加自定义服务，请参考 <see cref="CabApplication{TWorkItem}.AddServices"/>
        /// </summary>
        protected override void AddServices()
        {
            base.AddServices();

            RootWorkItem.Services.AddOnDemand<AdapterFactoryCatalog<IEditHandler>, IAdapterFactoryCatalog<IEditHandler>>();
            RootWorkItem.Services.AddOnDemand<AdapterFactoryCatalog<IPrintHandler>, IAdapterFactoryCatalog<IPrintHandler>>();
            RootWorkItem.Services.AddOnDemand<ImageService, IImageService>();
            RootWorkItem.Services.AddOnDemand<Db4oDatabaseService, IDb4oDatabaseService>();
            RootWorkItem.Services.AddOnDemand<PropertyService, IPropertyService>();
            RootWorkItem.Services.AddOnDemand<SettingService, ISettingService>();
            RootWorkItem.Services.AddOnDemand<EntityTranslatorService, IEntityTranslatorService>();
            RootWorkItem.Services.AddOnDemand<WorkspaceLocatorService, IWorkspaceLocatorService>();

        }
    }
}
