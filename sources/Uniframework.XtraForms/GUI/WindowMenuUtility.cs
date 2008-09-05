using System;
using System.Windows.Forms;

using DevExpress.XtraTabbedMdi;
using DevExpress.XtraBars;

namespace Uniframework.XtraForms
{
    /// <summary>
    /// 窗口菜单工具类
    /// </summary>
    internal class WindowMenuUtility
    {
        private WindowLayoutMode layoutMode = WindowLayoutMode.Tabbed;
        private XtraTabbedMdiManager mdiManager;
        private Form shell;

        public WindowMenuUtility() { }

        public WindowMenuUtility(XtraTabbedMdiManager mdiManager, Form shell)
            : this()
        {
            MdiManager = mdiManager;
            Shell = shell;
        }

        /// <summary>
        /// Gets or sets the MDI manager.
        /// </summary>
        /// <value>The MDI manager.</value>
        public XtraTabbedMdiManager MdiManager
        {
            get { return mdiManager; }
            set { mdiManager = value; }
        }

        /// <summary>
        /// Gets or sets the shell.
        /// </summary>
        /// <value>The shell.</value>
        public Form Shell
        {
            get { return shell; }
            set { shell = value; }
        }

        /// <summary>
        /// Gets the MDI mode.
        /// </summary>
        /// <value>The MDI mode.</value>
        public WindowLayoutMode LayoutMode
        {
            get { return layoutMode; }
        }

        /// <summary>
        /// MDIs the change mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void MdiChangeMode(object sender, EventArgs e)
        {
            bool tabbedLayout = mdiManager.MdiParent != null;
            SetLayoutMode(tabbedLayout == true ? WindowLayoutMode.Windowed : WindowLayoutMode.Tabbed);
        }

        /// <summary>
        /// Sets the MDI mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        private void SetLayoutMode(WindowLayoutMode mode)
        {
            MdiManager.MdiParent = mode == WindowLayoutMode.Tabbed ? Shell : null;
            layoutMode = mode;
        }

        /// <summary>
        /// MDIs the layout cascade.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void MdiLayoutCascade(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        /// <summary>
        /// MDIs the layout tile horizontal.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void MdiLayoutTileHorizontal(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        /// <summary>
        /// MDIs the layout tile vertical.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void MdiLayoutTileVertical(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        /// <summary>
        /// Layouts the MDI.
        /// </summary>
        /// <param name="layout">The layout.</param>
        private void LayoutMdi(MdiLayout layout)
        {
            SetLayoutMode(WindowLayoutMode.Windowed);
            MdiManager.MdiParent = null;
            Shell.LayoutMdi(layout);
        }
    }

    /// <summary>
    /// 窗口布局模式
    /// </summary>
    public enum WindowLayoutMode
    {
        /// <summary>
        /// 选项板
        /// </summary>
        Tabbed,
        /// <summary>
        /// MDI窗口
        /// </summary>
        Windowed
    }
}
