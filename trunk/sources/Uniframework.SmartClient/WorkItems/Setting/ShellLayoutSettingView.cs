using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraTabbedMdi;
using Uniframework.XtraForms.Workspaces;
using DevExpress.XtraBars;
using Uniframework.XtraForms;

namespace Uniframework.SmartClient.WorkItems.Setting
{
    public partial class ShellLayoutSettingView : DevExpress.XtraEditors.XtraUserControl, ISetting
    {
        private const string NAVIBAR_OUTLOOKSTYLE = "SkinNavigationPane";
        private const string NAVIBAR_EXPLORERSTYLE = "SkinExplorerBarView";

        private Property property = null;

        public ShellLayoutSettingView()
        {
            InitializeComponent();


        }

        #region Dependency services

        [ServiceDependency]
        public IPropertyService PropertyService
        {
            get;
            set;
        }

        [ServiceDependency]
        public ISettingService SettingService
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

        #region ISetting Members

        public event EventHandler<EventArgs<object>> SettingChanged;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public Property Property
        {
            get {
                if (property == null)
                    property = new Property(UIExtensionSiteNames.Shell_Property_ShellLayout, new ShellLayout
                    {
                        Location = Shell.Location,
                        WindowState = Shell.WindowState,
                        Size = Shell.Size,
                        ShowStatusBar = true,
                        DefaultSkin = UserLookAndFeel.Default.ActiveSkinName,
                        NavPaintStyleName = NAVIBAR_OUTLOOKSTYLE,
                        WindowLayoutMode = WindowLayoutMode.Tabbed
                    });

                return property;
            }
        }

        /// <summary>
        /// Bindings the property.
        /// </summary>
        public void BindingProperty()
        {
            XtraTabbedMdiManager mdiManager = WorkItem.RootWorkItem.Items.Get<XtraTabbedMdiManager>(UIExtensionSiteNames.Shell_Manager_TabbedMdiManager);
            if (mdiManager != null) {
                rgLayout.SelectedIndex = mdiManager.MdiParent != null ? 0 : 1;
            }

            XtraNavBarWorkspace naviPane = WorkItem.RootWorkItem.Items.Get<XtraNavBarWorkspace>(UIExtensionSiteNames.Shell_NaviPane);
            if (naviPane != null) {
                switch (naviPane.PaintStyleName) {
                    case NAVIBAR_OUTLOOKSTYLE :
                        rgNaviPane.SelectedIndex = 0;
                        break;

                    case NAVIBAR_EXPLORERSTYLE :
                        rgNaviPane.SelectedIndex = 1;
                        break;

                    default :
                        rgNaviPane.SelectedIndex = 0;
                        break;
                }
            }

            BarManager barManager = WorkItem.RootWorkItem.Items.Get <BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
            if (barManager != null) {
                Bar bar = barManager.Bars["StatusBar"];
                if (bar != null)
                    chkShowStatusBar.Checked = bar.Visible;
            }

            cbeSkin.Text = UserLookAndFeel.Default.ActiveSkinName;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            ShellLayout layout;
            if (Shell != null) {
                layout = new ShellLayout { 
                    Location = Shell.Location,
                    WindowState = Shell.WindowState,
                    Size = Shell.Size,
                    ShowStatusBar = chkShowStatusBar.Checked,
                    DefaultSkin = cbeSkin.Text,
                    NavPaintStyleName = rgNaviPane.SelectedIndex == 0 ? NAVIBAR_OUTLOOKSTYLE : NAVIBAR_EXPLORERSTYLE,
                    WindowLayoutMode = rgLayout.SelectedIndex == 0 ? WindowLayoutMode.Tabbed : WindowLayoutMode.Windowed
                };
                ApplySetting(layout); // 应用当前设置
                Property.Current = layout;
                PropertyService.Set<ShellLayout>(UIExtensionSiteNames.Shell_Property_ShellLayout, layout);
                if (SettingChanged != null)
                    SettingChanged(this, new EventArgs<object>(layout)); // 触发事件
            }
        }

        public void LoadDefault()
        {
            rgLayout.SelectedIndex = 0;
            rgNaviPane.SelectedIndex = 0;
            chkShowStatusBar.Checked = true;
        }

        #endregion

        #region Assistant functions

        private List<string> GetSortedSkinNames()
        {
            var skinNames = new List<string>(SkinManager.Default.Skins.Count);

            foreach (SkinContainer skinContainer in SkinManager.Default.Skins)
                skinNames.Add(skinContainer.SkinName);

            skinNames.Sort();
            return skinNames;
        }

        /// <summary>
        /// 应用当前设置
        /// </summary>
        /// <param name="layout">The layout.</param>
        private void ApplySetting(ShellLayout layout)
        {
            BarManager barManager = WorkItem.RootWorkItem.Items.Get(UIExtensionSiteNames.Shell_Bar_Manager) as BarManager;
            if (barManager != null) {
                Bar bar = barManager.Bars["StatusBar"];
                if (bar != null)
                    bar.Visible = layout.ShowStatusBar;
            }

            XtraNavBarWorkspace naviPane = WorkItem.RootWorkItem.Items.Get<XtraNavBarWorkspace>(UIExtensionSiteNames.Shell_NaviPane);
            if (naviPane != null) {
                naviPane.PaintStyleName = layout.NavPaintStyleName;
            }

            XtraTabbedMdiManager mdiManager = WorkItem.RootWorkItem.Items.Get<XtraTabbedMdiManager>(UIExtensionSiteNames.Shell_Manager_TabbedMdiManager);
            if (Shell != null && mdiManager != null) {
                mdiManager.MdiParent = layout.WindowLayoutMode == WindowLayoutMode.Tabbed ? Shell : null;
            }

            UserLookAndFeel.Default.SetSkinStyle(layout.DefaultSkin);
        }

        private Form Shell
        {
            get { return WorkItem.RootWorkItem.Items.Get<Form>(UIExtensionSiteNames.Shell); }
        }

        #endregion

        private void ShellLayoutSettingView_Load(object sender, EventArgs e)
        {
            cbeSkin.Properties.Items.AddRange(GetSortedSkinNames());

            Property.SetDefault();
            BindingProperty();
        }
    }
}
