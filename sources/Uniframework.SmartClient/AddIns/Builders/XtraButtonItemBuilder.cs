﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using System.Windows.Forms;

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
            item.Caption = label;
            item.Name = element.Name;
            if (element.Configuration.Attributes["tooltip"] != null)
                item.Hint = element.Configuration.Attributes["tooltip"];
            else
                item.Hint = label;
            if (element.Configuration.Attributes["largeimage"] != null) {
                string largeImage = element.Configuration.Attributes["largeimage"];
                item.LargeGlyph = BuilderUtility.GetBitmap(context, largeImage);
            }
            if (element.Configuration.Attributes["imagefile"] != null) {
                string image = element.Configuration.Attributes["imagefile"];
                item.Glyph = BuilderUtility.GetBitmap(context, image);
            }
            if (element.Configuration.Attributes["shortcut"] != null) {
                string key = element.Configuration.Attributes["shortcut"];
                item.ItemShortcut = new BarShortcut((Keys)Enum.Parse(typeof(Keys), key));
            }
            item.Tag = false;
            if (element.Configuration.Attributes["begingroup"] != null) { 
                bool beginGroup = bool.Parse(element.Configuration.Attributes["begingroup"]);
                item.Tag = beginGroup;
            }

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