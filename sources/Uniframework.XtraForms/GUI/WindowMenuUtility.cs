using System;
using System.Windows.Forms;

using DevExpress.XtraTabbedMdi;

namespace Uniframework.XtraForms
{
    /// <summary>
    /// 窗口菜单工具类
    /// </summary>
    internal class WindowMenuUtility
    {
        /// <summary>
        /// 窗口布局模式
        /// </summary>
        public enum MdiMode
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

        private MdiMode mdiMode = MdiMode.Tabbed;
        private XtraTabbedMdiManager mdiManager;
        private Form shell;

        internal WindowMenuUtility() { }

        internal WindowMenuUtility(XtraTabbedMdiManager mdiManager, Form shell)
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
        /// MDIs the change mode.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void MdiChangeMode(object sender, EventArgs e)
        {
            mdiMode = mdiMode == MdiMode.Tabbed ? MdiMode.Windowed : MdiMode.Tabbed; // Toggle
            SetMdiMode(mdiMode);
        }

        /// <summary>
        /// Sets the MDI mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        private void SetMdiMode(MdiMode mode)
        {
            MdiManager.MdiParent = mode == MdiMode.Tabbed ? Shell : null;
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
            SetMdiMode(MdiMode.Windowed);
            MdiManager.MdiParent = null;
            Shell.LayoutMdi(layout);
        }
    }
}
