using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient
{
    public class XtraContentMenuBuilder : IBuilder
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
            get { return "XtraContentMenu"; }
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
            IContentMenuService cmbService = context.Services.Get<IContentMenuService>();
            if (cmbService == null)
                throw new UniframeworkException(String.Format("未注册IContentMenuBarService无法创建上下文菜单 \"{0}\"。", element.Name));

            BarSubItem item = new BarSubItem();
            item.Name = element.Name;
            item.Tag = element.Path;
            cmbService.RegisterContentMenu(item.Tag as string, item);
            if (!String.IsNullOrEmpty(element.Command))
            {
                Command cmd = BuilderUtility.GetCommand(context, element.Command);
                if (cmd != null)
                    cmd.AddInvoker(item, "Popup");
            }
            context.UIExtensionSites.RegisterSite(BuilderUtility.CombinPath(element.Path, element.Id), item);
            return item;
        }

        #endregion
    }
}
