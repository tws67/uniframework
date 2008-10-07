using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraBars;
using Microsoft.Practices.CompositeUI;
using Uniframework.XtraForms;
using Uniframework.XtraForms.UIElements;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 工具栏列表菜单项构建器
    /// </summary>
    public class XtraBarListItemBuilder : IBuilder
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
            get { return "XtraBarListItem"; }
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
            BarItem item;
            bool register = false;
            if (element.Configuration.Attributes["register"] != null)
                register = bool.Parse(element.Configuration.Attributes["register"]);
            Bar bar = context.Items.Get<Bar>(UIExtensionSiteNames.Shell_Bar_Mainmenu);
            if (bar == null)
                throw new UniframeworkException("没有定义系统主菜单条无法创建系统皮肤菜单项。");

            item = new XtraBarListMenu(bar);
            item.Caption = label;
            item.Name = element.Name; 
            BarManager barManager = BuilderUtility.GetBarManager(context);
            if (barManager != null)
                item.Id = barManager.GetNewItemId(); // 为BarItem设置Id方便正确的保存和恢复其状态
            
            BarItemExtend extend = new BarItemExtend();
            if (element.Configuration.Attributes["begingroup"] != null)
            {
                bool beginGroup = bool.Parse(element.Configuration.Attributes["begingroup"]);
                extend.BeginGroup = beginGroup;
            }
            if (element.Configuration.Attributes["insertbefore"] != null)
                extend.InsertBefore = element.Configuration.Attributes["insertbefore"];
            item.Tag = extend;

            // 添加插件单元到系统中
            if (!String.IsNullOrEmpty(element.Path) && context.UIExtensionSites.Contains(element.Path))
                context.UIExtensionSites[element.Path].Add(item);

            // 注册此路径的插件
            if (register)
                context.UIExtensionSites.RegisterSite(BuilderUtility.CombinPath(element.Path, element.Id), item);
            return item;
        }

        #endregion
    }
}
