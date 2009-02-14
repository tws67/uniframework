using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Common.Services;
using Microsoft.Practices.CompositeUI.Services;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Db4o;
using Uniframework.SmartClient.Strategies;
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
        private readonly string dbPath = @"..\Data\";

        private IAdapterFactoryCatalog<IEditHandler> editFactoryCatalog;
        private IAdapterFactoryCatalog<IPrintHandler> printFactoryCatalog;
        private IAdapterFactoryCatalog<IDocumentHandler> documentFactoryCatalog;

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

            editFactoryCatalog = RootWorkItem.Services.AddNew<AdapterFactoryCatalog<IEditHandler>, IAdapterFactoryCatalog<IEditHandler>>();
            printFactoryCatalog = RootWorkItem.Services.AddNew<AdapterFactoryCatalog<IPrintHandler>, IAdapterFactoryCatalog<IPrintHandler>>();
            documentFactoryCatalog = RootWorkItem.Services.AddNew<AdapterFactoryCatalog<IDocumentHandler>, IAdapterFactoryCatalog<IDocumentHandler>>();
            
            RootWorkItem.Services.Add<IDb4oDatabaseService>(new Db4oDatabaseService(dbPath)); // Db4o数据库服务

            //RootWorkItem.Services.AddOnDemand<AdapterFactoryCatalog<IDataListHandler>, IAdapterFactoryCatalog<IDataListHandler>>();
            RootWorkItem.Services.AddOnDemand<AdapterFactoryCatalog<IDataListView>, IAdapterFactoryCatalog<IDataListView>>();
            RootWorkItem.Services.AddOnDemand<ImageService, IImageService>();
            RootWorkItem.Services.AddOnDemand<StringService, IStringService>();
            RootWorkItem.Services.AddOnDemand<PropertyService, IPropertyService>();
            RootWorkItem.Services.AddOnDemand<SettingService, ISettingService>();
            RootWorkItem.Services.AddOnDemand<EntityTranslatorService, IEntityTranslatorService>();
            RootWorkItem.Services.AddOnDemand<WorkspaceLocatorService, IWorkspaceLocatorService>();
            RootWorkItem.Services.AddOnDemand<UIExtensionService, IUIExtensionService>();

            AddCustomWorkItem(); // 加载自定义的工作项
            RegisterFactoryCatalog();
        }

        /// <summary>
        /// 添加自定义的策略构建器
        /// </summary>
        /// <param name="builder"></param>
        protected override void AddBuilderStrategies(Builder builder)
        {
            base.AddBuilderStrategies(builder);

            builder.Strategies.AddNew<TextEditAdapterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<XtraGridEditAdapterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<XtraPrintAdapterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<XtraDocumentAdapterStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<DataListViewStrategy>(BuilderStage.Initialization);
            builder.Strategies.AddNew<XtraContentMenuStrategy>(BuilderStage.Initialization);
        }

        #region Assistant function

        /// <summary>
        /// Adds the custom work item.
        /// </summary>
        private void AddCustomWorkItem()
        {
            ControlledWorkItem<TaskbarController> taskbarWorkItem = RootWorkItem.WorkItems.AddNew<ControlledWorkItem<TaskbarController>>();
            taskbarWorkItem.Run();
        }

        /// <summary>
        /// Registers the custom factory catalog.
        /// </summary>
        private void RegisterFactoryCatalog()
        {
            // 文档
            documentFactoryCatalog.RegisterFactory(new XtraDocumentAdapterFactory());

            // 编辑
            editFactoryCatalog.RegisterFactory(new TextEditAdapterFactory());
            editFactoryCatalog.RegisterFactory(new XtraGridEditAdapterFactory());

            // 打印
            printFactoryCatalog.RegisterFactory(new XtraPrintAdapterFactory(RootWorkItem));
        }

        #endregion

    }
}
