using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraBars;
using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 用户界面扩展服务接口
    /// </summary>
    public interface IUIExtensionService
    {
        /// <summary>
        /// 增加一个工具栏
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        /// <param name="text">标题</param>
        /// <param name="dockStyle">停靠样式</param>
        /// <returns>返回创建好的工具栏</returns>
        Bar AddBar(string extPath, string id, string text, BarDockStyle dockStyle);
        /// <summary>
        /// 删除一个工具栏
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        void RemoveBar(string extPath, string id);
        /// <summary>
        /// 设置工具栏的可见性
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="visible">是否可见</param>
        void SetBarVisible(string id, bool visible);

        /// <summary>
        /// 增加一个导航分组
        /// </summary>
        /// <param name="naviPane">导航控件名称</param>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        /// <param name="text">The text.</param>
        /// <param name="groupStyle">样式</param>
        /// <param name="imagefile">显示的小图标文件名，以"${...}"括起来</param>
        /// <param name="largeimage">显示的大图标文件名</param>
        /// <returns></returns>
        NavBarGroup AddNaviBar(string naviPane, string extPath, string id, string text, NavBarGroupStyle groupStyle, string imagefile, string largeimage);
        /// <summary>
        /// 删除一个导航分组
        /// </summary>
        /// <param name="naviPane">导航控件名称</param>
        /// <param name="extPath">路径</param>
        /// <param name="name">名称</param>
        void RemoveNaviBar(string naviPane, string extPath, string id);
        void SetNaviBarVisible(string naviPane, string extPath, string id, bool visible);

        NavBarItem AddNaviBarItem(string extPath, string id, string text, string command, string imagefile, string largeimage);
        void RemoveNaviBarItem(string extPath, string id);
        void SetNaviBarItemStatus(string extPath, string id, bool enabled);

        /// <summary>
        /// 增加一个菜单项、按钮
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        /// <param name="text">标题</param>
        /// <param name="command">命令</param>
        /// <param name="regisger">是否注册此菜单项以便可以在其下增加子菜单项</param>
        /// <returns>返回创建好的菜单项/按钮</returns>
        BarItem AddButton(string extPath, string id, string text, string command, bool regisger);
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
        BarItem AddButton(string extPath, string id, string text, string command, bool regisger, bool beginGroup, string insertBefore);
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
        BarItem AddButton(string extPath, string id, string text, string command, bool register, bool beginGroup, string insertBefore, string imagefile, string largeimage, string shortcut);
        /// <summary>
        /// 删除一个菜单项、按钮
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        void RemoveButton(string extPath, string id);
        /// <summary>
        /// 设置菜单项/按钮的状态
        /// </summary>
        /// <param name="extPath">路径</param>
        /// <param name="id">标识</param>
        /// <param name="enabled">状态</param>
        void SetButtonStatus(string extPath, string id, bool enabled);
    }
}
