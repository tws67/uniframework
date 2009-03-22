using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using log4net;

using Uniframework.Services;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件单元用于表示系统中最小的可供插、拔的插件元素
    /// </summary>
    public class AddInElement
    {
        private AddIn addIn;
        private string id;
        private string name;
        private string label;
        private string className;
        private string command;
        private string path;
        private object objectRef = null;
        private IConfiguration configuration;
        private WorkItem workItem;
        private List<ICondition> conditions = null;
        private ILog logger;

        /// <summary>
        /// 插件单元构造函数
        /// </summary>
        /// <param name="exPath">插件路径</param>
        /// <param name="configuration">插件单元配置项</param>
        /// <param name="conditions">条件项</param>
        public AddInElement(ExtensionPath exPath, IConfiguration configuration, List<ICondition> conditions)
        {
            this.addIn = exPath.AddIn;
            this.workItem = exPath.AddIn.WorkItem;
            this.path = exPath.Name;
            this.configuration = configuration;
            this.conditions = conditions;
            this.logger = workItem.Services.Get<ILog>();

            this.id = configuration.Attributes["id"];
            this.name = configuration.Attributes["name"];
            this.label = configuration.Attributes["label"];
            this.className = configuration.Attributes["classname"].ToLower(); // 小写
            this.command = configuration.Attributes["command"];
            this.conditions.Add(new AuthorizationCondition(this)); // 加入默认的权限管理条件表达式
        }

        /// <summary>
        /// 插件单元标识
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// 插件单元名称
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 标签
        /// </summary>
        public string Label
        {
            get { return label; }
        }

        /// <summary>
        /// 插件单元关联的控件类型名称，通过此名称来决定采用什么构建器进行创建
        /// </summary>
        public string ClassName
        {
            get { return className; }
        }

        /// <summary>
        /// 插件单元关系的操作命令标识
        /// </summary>
        public string Command
        {
            get { return command; }
        }

        /// <summary>
        /// 插件单元路径
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// 插件单元属性
        /// </summary>
        /// <param name="key">属性名称</param>
        /// <returns>属性值</returns>
        public string this[string key]
        {
            get { return configuration.Attributes[key]; }
        }

        /// <summary>
        /// 插件单元配置项
        /// </summary>
        public IConfiguration Configuration
        {
            get { return configuration; }
        }

        /// <summary>
        /// 工作项，组件容器
        /// </summary>
        public WorkItem WorkItem
        {
            get { return workItem; }
        }

        /// <summary>
        /// 插件单元所包含的条件
        /// </summary>
        public IList<ICondition> Conditions
        {
            get { return conditions; }
        }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILog Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// 返回条件失败时的处理动作
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <returns>处理动作</returns>
        public ConditionFailedAction GetFailedAction(object caller)
        {
            return Condition.GetFailedAction(conditions, caller, workItem);
        }

        /// <summary>
        /// 构建插件单元
        /// </summary>
        /// <param name="owner">拥有者</param>
        /// <param name="subItems">子对象</param>
        /// <returns>构建好的插件单元</returns>
        public object BuildItem(object owner, ArrayList subItems)
        {
            IBuilderService builderService = WorkItem.Services.Get<IBuilderService>();
            if (builderService != null)
            {
                IBuilder builder = builderService.GetBuilder(className);
                if (builder == null)
                    throw new AddInException(String.Format("系统未注册类型为 [{0}] 的插件单元构建器", className));

                if (builder.HandleConditions && conditions.Count > 0)
                {
                    ConditionFailedAction action = GetFailedAction(owner);
                    if (action != ConditionFailedAction.Nothing)
                        return null;
                }

                if (objectRef == null)
                    objectRef = builder.BuildItem(owner, workItem, this, subItems);
                return objectRef;
            }
            return null;
        }

        /// <summary>
        /// 构建插件单元自身，主要用于创建菜单项、按钮等不需要传入拥有者的元素
        /// </summary>
        /// <returns>构建好的插件单元</returns>
        public object BuildSelf()
        {
            return BuildItem(null, new ArrayList());
        }

        public override string ToString()
        {
            return String.Format("AddInElement: id[{0}], name[{1}], command[{2}], classname[{3}]", id, name, command, className);
        }
    }
}
