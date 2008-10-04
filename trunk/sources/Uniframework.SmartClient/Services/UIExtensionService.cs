using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Uniframework.XtraForms.UIElements;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 用户界面扩展服务，用于方便的创建相关UI元素
    /// </summary>
    public class UIExtensionService : IUIExtensionService
    {
        #region Dependency Service

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #endregion

        #region IUIExtensionService Members

        /// <summary>
        /// 增加一个工具栏
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        /// <param name="text"></param>
        /// <param name="dockStyle">停靠样式</param>
        /// <returns>返回创建好的工具栏</returns>
        public Bar AddBar(string extPath, string id, string text, BarDockStyle dockStyle)
        {
            Bar bar = null;
            BarManager barManager = WorkItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
            if (barManager == null)
                throw new UniframeworkException("未定义框架外壳的工具条管理器。");

            bar = new Bar(barManager, text);
            bar.BarName = id;
            bar.DockStyle = dockStyle;
            WorkItem.UIExtensionSites.RegisterSite(extPath, bar);
            return bar;
        }

        /// <summary>
        /// 删除一个工具栏
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        public void RemoveBar(string extPath, string id)
        {
            BarManager barManager = WorkItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
            if (barManager == null)
                return;

            foreach (Bar bar in barManager.Bars) {
                if (bar.BarName == id && WorkItem.UIExtensionSites.Contains(extPath)) {
                    WorkItem.UIExtensionSites[extPath].Clear();
                    barManager.Bars.RemoveAt(barManager.Bars.IndexOf(bar));
                    return;
                }
            }
        }

        /// <summary>
        /// 设置工具栏的可见性
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="visible">是否可见</param>
        public void SetBarVisible(string id, bool visible)
        {
            BarManager barManager = WorkItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
            if (barManager == null)
                return;

            foreach (Bar bar in barManager.Bars) {
                if (bar.BarName == id)
                    bar.Visible = visible;
            }
        }

        /// <summary>
        /// 增加一个导航分组
        /// </summary>
        /// <param name="naviPane"></param>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        /// <param name="text"></param>
        /// <param name="groupStyle">样式</param>
        /// <param name="imagefile">显示的小图标文件名，以"${...}"括起来</param>
        /// <param name="largeimage">显示的大图标文件名</param>
        /// <returns></returns>
        public NavBarGroup AddNaviBar(string naviPane, string extPath, string id, string text, NavBarGroupStyle groupStyle, string imagefile, string largeimage)
        {
            NavBarGroup group = null;

            NavBarControl naviBar = WorkItem.Items.Get<NavBarControl>(naviPane);
            if (naviBar == null)
                throw new UniframeworkException(String.Format("未定义导航栏 \"{0}\"", naviPane));

            group = new NavBarGroup(text);
            group.Name = id;
            group.GroupStyle = groupStyle;
            naviBar.Groups.Add(group);
            if (!String.IsNullOrEmpty(imagefile))
                group.SmallImage = BuilderUtility.GetBitmap(WorkItem, imagefile, 16, 16);
            if (!String.IsNullOrEmpty(largeimage))
                group.LargeImage = BuilderUtility.GetBitmap(WorkItem, largeimage, 32, 32);
            WorkItem.UIExtensionSites.RegisterSite(extPath, group);
            return group;
        }

        /// <summary>
        /// 删除一个导航分组
        /// </summary>
        /// <param name="naviPane"></param>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        public void RemoveNaviBar(string naviPane, string extPath, string id)
        {
            NavBarControl naviBar = WorkItem.Items.Get<NavBarControl>(naviPane);
            if (naviBar == null)
                return;

            foreach (NavBarGroup group in naviBar.Groups) {
                if (group.Name == id && WorkItem.UIExtensionSites.Contains(extPath)) {
                    WorkItem.UIExtensionSites[extPath].Clear();
                    group.ItemLinks.Clear();
                    naviBar.Groups.Remove(group);
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the navi bar visible.
        /// </summary>
        /// <param name="naviPane">The navi pane.</param>
        /// <param name="extPath">The extPath.</param>
        /// <param name="name">The name.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        public void SetNaviBarVisible(string naviPane, string extPath, string id, bool visible)
        {
            NavBarControl naviBar = WorkItem.Items.Get<NavBarControl>(naviPane);
            if (naviBar == null)
                return;

            foreach (NavBarGroup group in naviBar.Groups) {
                if (group.Name == id && WorkItem.UIExtensionSites.Contains(extPath))
                    group.Visible = visible;
                return;
            }
        }

        public NavBarItem AddNaviBarItem(string extPath, string id, string text, string command, string imagefile, string largeimage)
        {
            Guard.ArgumentNotNull(extPath, "extPath");

            NavBarItem item = new NavBarItem(text);
            item.Name = id;
            if (!String.IsNullOrEmpty(imagefile))
                item.SmallImage = BuilderUtility.GetBitmap(WorkItem, imagefile, 16, 16);
            if (!String.IsNullOrEmpty(largeimage))
                item.LargeImage = BuilderUtility.GetBitmap(WorkItem, largeimage, 32, 32);
            Command cmd = BuilderUtility.GetCommand(WorkItem, command);
            if (cmd != null)
                cmd.AddInvoker(item, "LinkClicked");
            if (!String.IsNullOrEmpty(extPath) && WorkItem.UIExtensionSites.Contains(extPath))
                WorkItem.UIExtensionSites[extPath].Add(item);

            return item;
        }

        public void RemoveNaviBarItem(string extPath, string id)
        {
            Guard.ArgumentNotNull(extPath, "extPath");

            if (WorkItem.UIExtensionSites.Contains(extPath)) {
                foreach (object item in WorkItem.UIExtensionSites[extPath]) {
                    NavBarItem naviItem = item as NavBarItem;
                    if (naviItem != null && naviItem.Name == id) {
                        WorkItem.UIExtensionSites[extPath].Remove(item);
                        return;
                    }
                }
            }
        }
        
        public void SetNaviBarItemStatus(string extPath, string id, bool enabled)
        {
            Guard.ArgumentNotNull(extPath, "extPath");

            if (WorkItem.UIExtensionSites.Contains(extPath)) {
                foreach (object item in WorkItem.UIExtensionSites[extPath]) {
                    NavBarItem naviItem = item as NavBarItem;
                    if (naviItem != null && naviItem.Name == id) {
                        naviItem.Enabled = enabled;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 增加一个菜单项、按钮
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        /// <param name="text">标题</param>
        /// <param name="command">命令</param>
        /// <param name="regisger">是否注册此菜单项以便可以在其下增加子菜单项</param>
        /// <returns>返回创建好的菜单项/按钮</returns>
        public BarItem AddButton(string extPath, string id, string text, string command, bool regisger)
        {
            BarItem item = CreateButton(extPath, id, text, command, regisger);
            if (!String.IsNullOrEmpty(extPath) && WorkItem.UIExtensionSites.Contains(extPath))
                WorkItem.UIExtensionSites[extPath].Add(item);

            return item;
        }

        /// <summary>
        /// 增加一个菜单项、按钮
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        /// <param name="text">标题</param>
        /// <param name="command">命令</param>
        /// <param name="regisger">是否注册此菜单项以便可以在其下增加子菜单项</param>
        /// <param name="beginGroup">是否添加分隔条</param>
        /// <param name="insertBefore">插入位置</param>
        /// <returns>返回创建好的菜单项/按钮</returns>
        public BarItem AddButton(string extPath, string id, string text, string command, bool regisger, bool beginGroup, string insertBefore)
        {
            BarItem item = CreateButton(extPath, id, text, command, regisger);
            BarItemExtend extend = new BarItemExtend();
            extend.BeginGroup = beginGroup;
            extend.InsertBefore = insertBefore;
            item.Tag = extend;
            if (!String.IsNullOrEmpty(extPath) && WorkItem.UIExtensionSites.Contains(extPath))
                WorkItem.UIExtensionSites[extPath].Add(item);
            return item;
        }

        /// <summary>
        /// 增加一个菜单项、按钮
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        /// <param name="text">标题</param>
        /// <param name="command">命令</param>
        /// <param name="register">是否注册此菜单项以便可以在其下增加子菜单项</param>
        /// <param name="beginGroup">是否添加分隔条</param>
        /// <param name="insertBefore">插入位置</param>
        /// <param name="imagefile">小图标文件名称</param>
        /// <param name="largeimage">大图标文件名称</param>
        /// <param name="shortcut">快捷键</param>
        /// <returns>返回创建好的菜单项/按钮</returns>
        public BarItem AddButton(string extPath, string id, string text, string command, bool register, bool beginGroup, string insertBefore, string imagefile, string largeimage, string shortcut)
        {
            BarItem item = AddButton(extPath, id, text, command, register, beginGroup, insertBefore);
            if (!String.IsNullOrEmpty(imagefile))
                item.Glyph = BuilderUtility.GetBitmap(WorkItem, imagefile, 16, 16);
            if (!String.IsNullOrEmpty(largeimage))
                item.LargeGlyph = BuilderUtility.GetBitmap(WorkItem, largeimage, 32, 32);
            return item;
        }

        /// <summary>
        /// 删除一个菜单项、按钮
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        public void RemoveButton(string extPath, string id)
        {
            if (WorkItem.UIExtensionSites.Contains(extPath)) {
                foreach (object item in WorkItem.UIExtensionSites[extPath]) {
                    BarItem button = item as BarItem;
                    if (button != null && button.Name == id) {
                        WorkItem.UIExtensionSites[extPath].Remove(item);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 设置菜单项/按钮的状态
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        /// <param name="enabled">状态</param>
        public void SetButtonStatus(string extPath, string id, bool enabled)
        {
            if (WorkItem.UIExtensionSites.Contains(extPath)) {
                foreach (object item in WorkItem.UIExtensionSites[extPath]) {
                    BarItem button = item as BarItem;
                    if (button != null && button.Name == id) {
                        button.Enabled = enabled;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Assistant functions

        private BarItem CreateButton(string extPath, string id, string text, string command, bool regisger)
        {
            BarItem item = null;
            if (regisger)
            {
                item = new BarSubItem();
                WorkItem.UIExtensionSites.RegisterSite(BuilderUtility.CombinPath(extPath, id), item);
            }
            else
                item = new BarButtonItem();
            item.Name = id;
            item.Caption = text;
            item.Hint = text;
            Command cmd = BuilderUtility.GetCommand(WorkItem, command);
            if (cmd != null)
                cmd.AddInvoker(item, "ItemClick");

            return item;
        }

        #endregion
    }
}
