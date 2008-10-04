using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraNavBar;
using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 快速链接任务服务接口
    /// </summary>
    public interface ITaskbarService
    {
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
        NavBarGroup AddNaviBar(string extPath, string id, string text, NavBarGroupStyle style, string imagefile, string largeimage);
        /// <summary>
        /// Removes the navi bar.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        void RemoveNaviBar(string extPath, string id);
        /// <summary>
        /// Sets the navi bar visible.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        void SetNaviBarVisible(string extPath, string id, bool visible);

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
        NavBarItem AddNaviBarItem(string extPath, string id, string text, string command, string imagefile, string largeimage);
        /// <summary>
        /// Removes the navi bar item.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        void RemoveNaviBarItem(string extPath, string id);
        /// <summary>
        /// Sets the navi bar item status.
        /// </summary>
        /// <param name="extPath">The ext path.</param>
        /// <param name="id">The id.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        void SetNaviBarItemStatus(string extPath, string id, bool enabled);

        /// <summary>
        /// Shows the smart part.
        /// </summary>
        /// <param name="smartPart">The smart part.</param>
        /// <param name="id">The id.</param>
        /// <param name="spi">The spi.</param>
        TView ShowSmartPart<TView>(string id, XtraNavBarGroupSmartPartInfo spi);
        /// <summary>
        /// Closes the smart part.
        /// </summary>
        /// <param name="id">The id.</param>
        void CloseSmartPart<TView>(string id);
    }
}