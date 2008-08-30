using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Xtrabar构建器
    /// </summary>
    public class XtraBarBuilder : IBuilder
    {
        #region IBuilder Members

        /// <summary>
        /// 条件处理标志
        /// </summary>
        /// <value></value>
        public bool HandleConditions
        {
            get { return true; }
        }

        /// <summary>
        /// 构建器所处理的类的名称
        /// </summary>
        /// <value></value>
        public string ClassName
        {
            get { return "XtraBar"; }
        }

        /// <summary>
        /// 构建插件单元
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <param name="context">上下文，用于存放在构建时需要的组件</param>
        /// <param name="element">插件单元</param>
        /// <param name="subItems">被构建的子对象列表</param>
        /// <returns>构建好的插件单元</returns>
        public object BuildItem(object caller, WorkItem context, AddInElement element, ArrayList subItems)
        {
            if (element.Configuration.Attributes["label"] == null)
                throw new AddInException(String.Format("没有为类型为 \"{0}\" 的插件单元{1}提供label属性。",
                    element.ClassName, element.Id));

            string label = element.Configuration.Attributes["label"];
            BarManager barManager = context.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
            if (barManager == null)
                throw new UniframeworkException("未定义框架外壳的工具条管理器。");

            Bar item = new Bar(barManager, label);
            item.DockStyle = BarDockStyle.Top; // 默认停靠在顶部
            if (element.Configuration.Attributes["dockstyle"] != null) {
                string dockStyle = element.Configuration.Attributes["dockstyle"];
                item.DockStyle = (BarDockStyle)Enum.Parse(typeof(BarDockStyle), dockStyle);
            }
            if (element.Configuration.Attributes["register"] != null) {
                bool register = bool.Parse(element.Configuration.Attributes["register"]);
                if (register)
                    context.UIExtensionSites.RegisterSite(BuilderUtility.CombinPath(element.Path, element.Id), item);
            }

            return item;
        }

        #endregion
    }
}
