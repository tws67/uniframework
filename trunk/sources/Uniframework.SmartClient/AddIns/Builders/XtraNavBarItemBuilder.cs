using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// Xtra Navbar Control item builder
    /// </summary>
    public class XtraNavBarItemBuilder : IBuilder
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
            get { return "XtraNavBarItem"; }
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
            NavBarItem item = new NavBarItem(label);
            item.Name = element.Name;
            if (element.Configuration.Attributes["tooltip"] != null)
                item.Hint = element.Configuration.Attributes["tooltip"];

            if (element.Configuration.Attributes["largeimage"] != null) { 
                string largeImage = element.Configuration.Attributes["largeimage"];
                item.LargeImage = BuilderUtility.GetBitmap(context, largeImage, 32, 32);
            }
            if (element.Configuration.Attributes["imagefile"] != null) {
                string image = element.Configuration.Attributes["imagefile"];
                item.SmallImage = BuilderUtility.GetBitmap(context, image, 16, 16);
            }

            if (!String.IsNullOrEmpty(element.Path) && context.UIExtensionSites.Contains(element.Path))
                context.UIExtensionSites[element.Path].Add(item);
            Command cmd = BuilderUtility.GetCommand(context, element.Command);
            if (cmd != null)
                cmd.AddInvoker(item, "LinkClicked");
            return item;
        }

        #endregion
    }
}
