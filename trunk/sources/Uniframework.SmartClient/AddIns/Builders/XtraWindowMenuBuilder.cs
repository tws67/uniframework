using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraBars;
using DevExpress.XtraTabbedMdi;
using Microsoft.Practices.CompositeUI;
using Uniframework.XtraForms;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统窗口管理菜单构建器
    /// </summary>
    public class XtraWindowMenuBuilder : IBuilder
    {
        #region IBuilder Members

        /// <summary>
        /// 条件处理标志
        /// </summary>
        /// <value></value>
        public bool HandleConditions
        {
            get { return false; }
        }

        /// <summary>
        /// 构建器所处理的类的名称
        /// </summary>
        /// <value></value>
        public string ClassName
        {
            get { return "XtraWindowMenu"; }
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
            Guard.ArgumentNotNull(context, "插件构建上下文对象");

            if (element.Configuration.Attributes["label"] == null)
                throw new AddInException(String.Format("没有为类型为 \"{0}\" 的插件单元{1}提供label属性。",
                    element.ClassName, element.Id));

            string label = element.Configuration.Attributes["label"];
            Form shell = context.Items.Get<Form>(UIExtensionSiteNames.Shell);
            Bar bar = context.Items.Get<Bar>(UIExtensionSiteNames.Shell_Bar_Mainmenu);
            XtraTabbedMdiManager mdiManager = context.Items.Get<XtraTabbedMdiManager>(UIExtensionSiteNames.Shell_Manager_TabbedMdiManager);
            Guard.ArgumentNotNull(bar, "Main menu bar.");
            Guard.ArgumentNotNull(shell, "Shell");
            Guard.ArgumentNotNull(mdiManager, "Tabbed mdi manager.");

            XtraWindowMenu item = new XtraWindowMenu(bar, mdiManager, shell);
            item.Caption = label;

            if (!String.IsNullOrEmpty(element.Path) && context.UIExtensionSites.Contains(element.Path))
                context.UIExtensionSites[element.Path].Add(item);
            // 注册插件单元
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
