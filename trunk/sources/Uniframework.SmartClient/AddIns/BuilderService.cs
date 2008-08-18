using System;
using System.Collections.Generic;
using System.Text;
using log4net;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件构建服务
    /// </summary>
    public class BuilderService : IBuilderService
    {
        private WorkItem workItem;
        private ILog logger = null;
        private IDictionary<string, IBuilder> builders = new Dictionary<string, IBuilder>();

        public BuilderService([ServiceDependency]WorkItem workItem)
        {
            this.workItem = workItem;
            this.logger = workItem.Services.Get<ILog>();
        }

        #region IBuilderService Members

        public void RegisterAdapterFactory(IUIElementAdapterFactory factory)
        {
            IUIElementAdapterFactoryCatalog catalog = workItem.Services.Get<IUIElementAdapterFactoryCatalog>();
            if (catalog != null)
                catalog.RegisterFactory(factory);
        }

        public void RegisterCommandAdapter(Type invokerType, Type adapterType)
        {
            ICommandAdapterMapService camSrv = workItem.Services.Get<ICommandAdapterMapService>();
            if (camSrv != null)
                camSrv.Register(invokerType, adapterType);
        }

        public void RegisterBuilder(IBuilder builder)
        {
            Guard.ArgumentNotNull(builder, "builder");
            if (!builders.ContainsKey(builder.ClassName.ToLower()))
                builders.Add(builder.ClassName.ToLower(), builder);
        }

        public void UnRegisterBuilder(string className)
        {
            if (builders.ContainsKey(className.ToLower()))
                builders.Remove(className.ToLower());
        }

        public IBuilder GetBuilder(string className)
        {
            if (builders.ContainsKey(className.ToLower()))
                return builders[className.ToLower()];
            return null;
        }

        #endregion
    }
}
