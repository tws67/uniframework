using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;

using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统插件描述类，对应外部的.addin文件
    /// </summary>
    public class AddIn
    {
        private static readonly String AddinsPath = @"\AddIns\";
        private static readonly String ResourcesPath = "Resources/";
        private static readonly String BuildersPath = "Builders/";
        private static readonly String AddInPath = "AddIns/";

        private string name;
        private string author;
        private string copyright;
        private string description;
        private string url;
        private bool hidenInManager;
        private string addInFileName = string.Empty;
        private AddInAction action = AddInAction.Disable;
        private bool enabled;

        private IDictionary<string, ExtensionPath> paths = new Dictionary<string, ExtensionPath>();
        private WorkItem workItem;
        private ILog logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">插件描述文件</param>
        /// <param name="workItem">组件容器</param>
        public AddIn(string filename, WorkItem workItem)
        {
            Guard.ArgumentNotNull(filename, "filename");
            Guard.ArgumentNotNull(workItem, "workItem");

            enabled = true;
            addInFileName = filename;
            this.workItem = workItem;
            logger = workItem.Services.Get<ILog>();
            Load(filename); // 加载并解析插件描述文件
        }

        #region AddIn Members

        public string Name
        {
            get { return name; }
        }

        public string Author
        {
            get { return author; }
        }

        public string Copyright
        {
            get { return copyright; }
        }

        public string Description
        {
            get { return description; }
        }

        public string Url
        {
            get { return url; }
        }

        public bool HidenInManager
        {
            get { return hidenInManager; }
        }

        public string FileName
        {
            get { return addInFileName; }
        }

        public AddInAction Action
        {
            get { return action; }
            set { action = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                action = value ? AddInAction.Enable : AddInAction.Disable;
            }
        }

        public IDictionary<string, ExtensionPath> Paths
        {
            get { return paths; }
        }

        /// <summary>
        /// 组件容器
        /// </summary>
        public WorkItem WorkItem
        {
            get { return workItem; }
        }

        public ExtensionPath GetExtensionPath(string name)
        {
            if (!paths.ContainsKey(name))
                return paths[name] = new ExtensionPath(this, name);
            return paths[name];
        }

        public override string ToString()
        {
            return "AddIn: " + Name;
        }
        
        #endregion

        #region Assistant functions

        /// <summary>
        /// 从插件描述文件加载插件
        /// </summary>
        /// <param name="filename"></param>
        private void Load(string filename)
        {
            string addInfile = filename;
            if (!File.Exists(addInfile))
            {
                List<string> files = FileUtility.SearchDirectory(Directory.GetParent(Environment.CurrentDirectory) + AddinsPath, addInfile);
                if (files.Count > 0)
                {
                    addInfile = files[0];
                    addInFileName = addInfile;
                }
                else
                    return;
            }

            XMLConfigurationService configService = new XMLConfigurationService(addInfile);
            logger.Info(String.Format("开始解析插件描述文件 \"{0}\"：", Path.GetFileName(addInfile)));
            IConfiguration configuration = null;

            // 解析插件描述文件信息
            configuration = new XMLConfiguration(configService.GetItem(""));
            if (configuration != null)
            {
                if (configuration.Attributes["name"] != null)
                    name = configuration.Attributes["name"];
                if (configuration.Attributes["author"] != null)
                    author = configuration.Attributes["author"];
                if (configuration.Attributes["copyright"] != null)
                    copyright = configuration.Attributes["copyright"];
                if (configuration.Attributes["description"] != null)
                    description = configuration.Attributes["description"];
                if (configuration.Attributes["url"] != null)
                    url = configuration.Attributes["url"];
                if (configuration.Attributes["hideninmanager"] != null)
                    hidenInManager = bool.Parse(configuration.Attributes["hideninmanager"]);
                else
                    hidenInManager = false;
            }

            // 解析图象、字符串资源
            if (configService.Exists(ResourcesPath))
            {
                configuration = new XMLConfiguration(configService.GetItem(ResourcesPath));
                ParseResource(configuration);
            }

            // 解析构建器
            if (configService.Exists(BuildersPath))
            {
                configuration = new XMLConfiguration(configService.GetItem(BuildersPath));
                ParseBuilders(configuration);
            }

            // 解析插件
            if (configService.Exists(AddInPath))
            {
                configuration = new XMLConfiguration(configService.GetItem(AddInPath));
                ParseAddIn(configuration);
            }
            logger.Info(String.Format("完成解析插件描述文件 \"{0}\"", Path.GetFileName(addInfile)));
        }

        /// <summary>
        /// 解析插件
        /// </summary>
        /// <param name="configuration">配置项</param>
        private void ParseAddIn(IConfiguration configuration)
        {
            Guard.ArgumentNotNull(configuration, "configuration");
            foreach (IConfiguration config in configuration.Children)
            {
                switch (config.Name)
                {
                    case "Path" : // 插件单元路径，此节点下定义具体的插件单元
                        if (config.Attributes["name"] == null)
                            throw new AddInException("必须为Path节点定义\"name\"属性。");

                        string name = config.Attributes["name"];
                        ExtensionPath exPath = GetExtensionPath(name);
                        if (config.Attributes["label"] != null)
                            exPath.Label = config.Attributes["label"];
                        if (config.Attributes["buildstartup"] != null)
                            exPath.BuildStartUp = bool.Parse(config.Attributes["buildstartup"]);
                        ExtensionPath.SetUp(exPath, config);
                        break;
                }
            }
        }

        /// <summary>
        /// 解析构建器，在这里对注册到系统中的UI类工厂、命令适配器、构建器进行处理
        /// </summary>
        /// <param name="configuration">配置项</param>
        private void ParseBuilders(IConfiguration configuration)
        {
            Guard.ArgumentNotNull(configuration, "configuration");
            IBuilderService builderService = workItem.Services.Get<IBuilderService>();
            if (builderService != null)
            {
                foreach (IConfiguration config in configuration.Children)
                {
                    switch (config.Name)
                    {
                        case "UIAdapterFactories" : // UI类工厂
                            foreach (IConfiguration factory in config.Children)
                            {
                                if (factory.Attributes["name"] != null && factory.Attributes["type"] != null)
                                {
                                    logger.Debug("注册UI类工厂，" + factory.ToString());
                                    // 根据配置文件注册UI类工厂
                                    Type factoryType = Type.GetType(factory.Attributes["type"]);
                                    if (factoryType != null)
                                    {
                                        try
                                        {
                                            builderService.RegisterAdapterFactory((IUIElementAdapterFactory)Activator.CreateInstance(factoryType));
                                        }
                                        catch
                                        {
                                            logger.Error("注册UI类工厂，" + factory.ToString() + "失败！");
                                        }
                                    }
                                }
                            }
                            break;

                        case "CommandAdapters": // 命令适配器
                            foreach (IConfiguration adapter in config.Children)
                            {
                                if (adapter.Attributes["name"] != null && adapter.Attributes["invokertype"] != null && adapter.Attributes["adaptertype"] != null)
                                {
                                    //logger.Debug("注册命令适配器，Name[" + adapter.Attributes["name"] + "]，InvokerType[" + adapter.Attributes["invokertype"]
                                    //    + "]，AdapterType[" + adapter.Attributes["adapterType"] + "]");
                                    logger.Debug("注册命令适配器，" + adapter.ToString());
                                    Type invokerType = Type.GetType(adapter.Attributes["invokertype"]);
                                    Type adapterType = Type.GetType(adapter.Attributes["adaptertype"]);
                                    if (invokerType != null && adapterType != null)
                                    {
                                        try
                                        {
                                            builderService.RegisterCommandAdapter(invokerType, adapterType);
                                        }
                                        catch
                                        {
                                            logger.Debug("注册命令适配器，" + adapter.ToString() + "失败！");
                                        }
                                    }
                                }
                            }
                            break;

                        case "UIBuilders" : // 插件构建器
                            foreach (IConfiguration builder in config.Children)
                            {
                                if (builder.Attributes["name"] != null && builder.Attributes["type"] != null)
                                {
                                    logger.Debug("注册插件单元构建器，" + builder.ToString());
                                    Type builderType = Type.GetType(builder.Attributes["type"]);
                                    if (builderType != null)
                                    {
                                        try
                                        {
                                            IBuilder uibuilder = (IBuilder)Activator.CreateInstance(builderType);
                                            builderService.RegisterBuilder(uibuilder);
                                        }
                                        catch
                                        {
                                            logger.Debug("注册插件单元构建器，" + builder.ToString() + "失败!");
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 解析图象、字符串资源
        /// </summary>
        /// <param name="configuration">配置项</param>
        private void ParseResource(IConfiguration configuration)
        {
            Guard.ArgumentNotNull(configuration, "configuration");
            IStringService strService = workItem.Services.Get<IStringService>();
            if (strService != null)
            {
                foreach (IConfiguration config in configuration.Children)
                {
                    switch (config.Name)
                    {
                        //case "Images": // Images
                        //    foreach (IConfiguration image in config.Children)
                        //    {
                        //        if (image.Attributes["key"] != null && image.Attributes["value"] != null)
                        //            strService.RegisterImages(image.Attributes["key"], image.Attributes["value"]);
                        //    }
                        //    break;

                        // 当前还没有实现直接注册字符串资源
                        case "Strings" : // Strings
                            foreach (IConfiguration str in config.Children)
                            {
                                if (str.Attributes["key"] != null && str.Attributes["value"] != null)
                                    strService.Register(str.Attributes["key"], str.Attributes["value"]);
                            }
                            break;
                    }
                }
            }
        }

        #endregion
    }

    #region AddIn action enum

    /// <summary>
    /// 插件动作
    /// </summary>
    public enum AddInAction
    { 
        /// <summary>
        /// 可用
        /// </summary>
        Enable,
        /// <summary>
        /// 禁用
        /// </summary>
        Disable,
        /// <summary>
        /// 安装
        /// </summary>
        Install,
        /// <summary>
        /// 卸载
        /// </summary>
        UnInstall,
        /// <summary>
        /// 更新
        /// </summary>
        Update
    }
    
    #endregion

}
