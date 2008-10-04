using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Uniframework.XtraForms.SmartPartInfos;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.SmartClient
{
    public class TaskbarService : ITaskbarService
    {
        #region Dependency services

        [ServiceDependency]
        public IUIExtensionService UIExtensionService
        {
            get;
            set;
        }

        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #endregion

        #region ITaskbarService Members

        /// <summary>
        /// Adds the navi bar.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        /// <param name="text">The text.</param>
        /// <param name="style">The style.</param>
        /// <param name="imagefile">The imagefile.</param>
        /// <param name="largeimage">The largeimage.</param>
        /// <returns></returns>
        public NavBarGroup AddNaviBar(string extPath, string id, string text, NavBarGroupStyle style, string imagefile, string largeimage)
        {
            return UIExtensionService.AddNaviBar(UIExtensionSiteNames.Shell_NaviPane_Taskbar, extPath, id, text, style, imagefile, largeimage);
        }

        /// <summary>
        /// Removes the navi bar.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        public void RemoveNaviBar(string extPath, string id)
        {
            UIExtensionService.RemoveNaviBar(UIExtensionSiteNames.Shell_NaviPane_Taskbar, extPath, id);
        }

        /// <summary>
        /// Sets the navi bar visible.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        public void SetNaviBarVisible(string extPath, string id, bool visible)
        {
            UIExtensionService.SetNaviBarVisible(UIExtensionSiteNames.Shell_NaviPane_Taskbar, extPath, id, visible);
        }

        /// <summary>
        /// Adds the navi bar item.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        /// <param name="text">The text.</param>
        /// <param name="command">The command.</param>
        /// <param name="imagefile">The imagefile.</param>
        /// <param name="largeimage">The largeimage.</param>
        /// <returns></returns>
        public NavBarItem AddNaviBarItem(string extPath, string id, string text, string command, string imagefile, string largeimage)
        {
            return UIExtensionService.AddNaviBarItem(extPath, id, text, command, imagefile, largeimage);
        }

        /// <summary>
        /// Removes the navi bar item.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        public void RemoveNaviBarItem(string extPath, string id)
        {
            UIExtensionService.RemoveNaviBarItem(extPath, id);
        }

        /// <summary>
        /// Sets the navi bar item status.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public void SetNaviBarItemStatus(string extPath, string id, bool enabled)
        {
            UIExtensionService.SetNaviBarItemStatus(extPath, id, enabled);
        }

        /// <summary>
        /// Shows the smart part.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="id">The id.</param>
        /// <param name="spi">The spi.</param>
        /// <returns></returns>
        public TView ShowSmartPart<TView>(string id, XtraNavBarGroupSmartPartInfo spi)
        {
            TView view = default(TView);
            view = WorkItem.SmartParts.Get<TView>(id);
            if (view == null)
                view = WorkItem.SmartParts.AddNew<TView>(id);

            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Taskbar);
            if (wp != null)
                wp.Show(view, spi);

            return view;
        }

        /// <summary>
        /// Closes the smart part.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="id">The id.</param>
        public void CloseSmartPart<TView>(string id)
        {
            IWorkspace wp = WorkItem.Workspaces.Get(UIExtensionSiteNames.Shell_Workspace_Taskbar);
            if (wp != null) {
                TView view = WorkItem.SmartParts.Get<TView>(id);
                if (view != null)
                    wp.Close(view);
            }
        }

        #endregion
    }
}
