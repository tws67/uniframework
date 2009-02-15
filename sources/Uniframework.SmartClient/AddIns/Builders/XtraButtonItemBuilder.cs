using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using System.Windows.Forms;
using Uniframework.XtraForms.UIElements;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 菜单项、工具栏按钮构建器
    /// </summary>
    public class XtraButtonItemBuilder : IBuilder
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
            get { return "XtraButtonItem"; }
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
            BarItem item;
            bool register = false;
            if (element.Configuration.Attributes["register"] != null)
                register = bool.Parse(element.Configuration.Attributes["register"]);
            if (register)
                item = new BarSubItem();
            else
                item = new BarButtonItem();
            item.Caption = BuilderUtility.GetStringRES(context, label);
            item.Name = element.Name;
            BarManager barManager = BuilderUtility.GetBarManager(context);
            if (barManager != null)
                item.Id = barManager.GetNewItemId(); // 为BarItem设置Id方便正确的保存和恢复其状态

            if (element.Configuration.Attributes["alignment"] != null)
                item.Alignment = (BarItemLinkAlignment)Enum.Parse(typeof(BarItemLinkAlignment), element.Configuration.Attributes["alignment"]);
            if (element.Configuration.Attributes["paintstyle"] != null)
                item.PaintStyle = (BarItemPaintStyle)Enum.Parse(typeof(BarItemPaintStyle), element.Configuration.Attributes["paintstyle"]);
            if (element.Configuration.Attributes["tooltip"] != null)
                item.Hint = element.Configuration.Attributes["tooltip"];
            else
                item.Hint = BuilderUtility.GetStringRES(context, label);
            if (element.Configuration.Attributes["largeimage"] != null) {
                string largeImage = element.Configuration.Attributes["largeimage"];
                item.LargeGlyph = BuilderUtility.GetBitmap(context, largeImage, 32, 32);
            }
            if (element.Configuration.Attributes["imagefile"] != null) {
                string image = element.Configuration.Attributes["imagefile"];
                item.Glyph = BuilderUtility.GetBitmap(context, image, 16, 16);
            }
            if (element.Configuration.Attributes["shortcut"] != null) {
                string key = element.Configuration.Attributes["shortcut"];
                try {
                    item.ItemShortcut = new BarShortcut((Shortcut)Enum.Parse(typeof(Shortcut), key));
                }
                catch { 
                }
            }

            BarItemExtend extend = new BarItemExtend();
            if (element.Configuration.Attributes["begingroup"] != null) { 
                bool beginGroup = bool.Parse(element.Configuration.Attributes["begingroup"]);
                extend.BeginGroup = beginGroup;
            }
            if (element.Configuration.Attributes["insertbefore"] != null)
                extend.InsertBefore = element.Configuration.Attributes["insertbefore"];
            item.Tag = extend;

            // 设置菜单项/按钮为选择项
            if ((element.Configuration.Attributes["checked"] != null) && (item is BarButtonItem)) {
                ((BarButtonItem)item).ButtonStyle = BarButtonStyle.Check;
                bool check = bool.Parse(element.Configuration.Attributes["checked"]);
                ((BarButtonItem)item).Down = check;
                if (element.Configuration.Attributes["optiongroup"] != null)
                    ((BarButtonItem)item).GroupIndex = int.Parse(element.Configuration.Attributes["optiongroup"]);
            }

            // 添加插件单元到系统中
            if (!String.IsNullOrEmpty(element.Path) && context.UIExtensionSites.Contains(element.Path))
                context.UIExtensionSites[element.Path].Add(item);
            Command cmd = BuilderUtility.GetCommand(context, element.Command);
            if (cmd != null)
                cmd.AddInvoker(item, "ItemClick");

            // 注册此路径的插件
            if (register)
                context.UIExtensionSites.RegisterSite(BuilderUtility.CombinPath(element.Path, element.Id), item);
            return item;
        }

        #endregion
    }
}
