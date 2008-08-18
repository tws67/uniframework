using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件路径，用于表示当前插件下的功能模块的路径
    /// </summary>
    public sealed class ExtensionPath
    {
        private AddIn addIn;
        private string name;
        private string label;
        private bool buildStartUp = false;
        private IList<AddInElement> addInElements = new List<AddInElement>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="addIn">插件</param>
        /// <param name="name">路径</param>
        internal ExtensionPath(AddIn addIn, string name)
        {
            this.addIn = addIn;
            this.name = name;
        }

        /// <summary>
        /// 插件
        /// </summary>
        public AddIn AddIn
        {
            get { return addIn; }
        }

        /// <summary>
        /// 立即构建标志，如果BuildStartUp为true在分析当前路径时便马上构建相关的插件单元
        /// </summary>
        public bool BuildStartUp
        {
            get { return buildStartUp; }
            set { buildStartUp = value; }
        }

        /// <summary>
        /// 路径名称，表示实际的路径名
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 路径标签构建系统权限系统时使用
        /// </summary>
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        /// 当前路径下包含的插件单元
        /// </summary>
        public IList<AddInElement> AddInElements
        {
            get { return addInElements; }
        }

        /// <summary>
        /// 设置当前插件路径
        /// </summary>
        /// <param name="exPath">插件路径</param>
        /// <param name="configuration">配置项</param>
        public static void SetUp(ExtensionPath exPath, IConfiguration configuration)
        {
            ILog logger = exPath.AddIn.WorkItem.Services.Get<ILog>();
            logger.Debug("解析插件路径，" + exPath);

            /// 
            Stack<ICondition> conditionStack = new Stack<ICondition>();
            foreach (IConfiguration config in configuration.Children)
            {
                if (config.Name == "Condition")
                    conditionStack.Push(new Condition(config.Name, config));
                else if (config.Name == "ComplexCondition")
                    conditionStack.Push(new ComplexCondition(config.Name, config));
                else {
                    List<ICondition> conditions = new List<ICondition>();
                    conditions.AddRange(conditionStack.ToArray());
                    AddInElement element = new AddInElement(exPath, config, conditions);
                    logger.Debug("　　添加插件单元，" + element.ToString());
                    if (exPath.BuildStartUp) // 立即创建插件单元
                        element.BuildSelf();
                    exPath.AddInElements.Add(element);
                    if (config.Children.Count > 0)
                    {
                        string pathStr = exPath.Name.EndsWith("/") ? exPath.Name + element.Id : exPath.Name + "/" + element.Id;
                        ExtensionPath subPath = exPath.AddIn.GetExtensionPath(pathStr);
                        subPath.BuildStartUp = exPath.BuildStartUp;
                        subPath.Label = element.Label;
                        SetUp(subPath, config);
                    }
                }

            }
        }

        public override string ToString()
        {
            return "ExtensionPath : " + Name;
        }
    }
}
