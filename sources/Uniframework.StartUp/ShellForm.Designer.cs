namespace Uniframework.StartUp
{
    partial class ShellForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.MenuBar = new DevExpress.XtraBars.Bar();
            this.StatusBar = new DevExpress.XtraBars.Bar();
            this.tlabStatus = new DevExpress.XtraBars.BarStaticItem();
            this.tlabCustomPanel1 = new DevExpress.XtraBars.BarStaticItem();
            this.tlabCustomPanel2 = new DevExpress.XtraBars.BarStaticItem();
            this.ProgressBar = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.tlabUser = new DevExpress.XtraBars.BarStaticItem();
            this.tbtnNetworkStatus = new DevExpress.XtraBars.BarButtonItem();
            this.tlabRequestSize = new DevExpress.XtraBars.BarStaticItem();
            this.ZoomBar = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemZoomTrackBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.DockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.tabbedMdiManager = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.DeckWorkspace = new Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace();
            this.naviWorkspace = new Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace();
            this.SplitterControl = new DevExpress.XtraEditors.SplitterControl();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DockManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedMdiManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.naviWorkspace)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.MenuBar,
            this.StatusBar});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.DockManager = this.DockManager;
            this.barManager.DockWindowTabFont = new System.Drawing.Font("SimSun", 9F);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.tlabStatus,
            this.tlabCustomPanel1,
            this.tlabCustomPanel2,
            this.tlabUser,
            this.barStaticItem1,
            this.ProgressBar,
            this.tlabRequestSize,
            this.ZoomBar,
            this.tbtnNetworkStatus});
            this.barManager.MainMenu = this.MenuBar;
            this.barManager.MaxItemId = 9;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemProgressBar1,
            this.repositoryItemZoomTrackBar1});
            this.barManager.StatusBar = this.StatusBar;
            // 
            // MenuBar
            // 
            this.MenuBar.BarName = "Main menu";
            this.MenuBar.DockCol = 0;
            this.MenuBar.DockRow = 0;
            this.MenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.MenuBar.OptionsBar.AllowQuickCustomization = false;
            this.MenuBar.OptionsBar.DisableClose = true;
            this.MenuBar.OptionsBar.DisableCustomization = true;
            this.MenuBar.OptionsBar.UseWholeRow = true;
            this.MenuBar.Text = "主菜单";
            // 
            // StatusBar
            // 
            this.StatusBar.BarName = "Status bar";
            this.StatusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.StatusBar.DockCol = 0;
            this.StatusBar.DockRow = 0;
            this.StatusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.StatusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabStatus),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabCustomPanel1, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabCustomPanel2),
            new DevExpress.XtraBars.LinkPersistInfo(((DevExpress.XtraBars.BarLinkUserDefines)((DevExpress.XtraBars.BarLinkUserDefines.PaintStyle | DevExpress.XtraBars.BarLinkUserDefines.Width))), this.ProgressBar, "", true, true, true, 106, null, DevExpress.XtraBars.BarItemPaintStyle.Standard),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabUser, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.tbtnNetworkStatus, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabRequestSize, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.ZoomBar)});
            this.StatusBar.OptionsBar.AllowQuickCustomization = false;
            this.StatusBar.OptionsBar.DrawDragBorder = false;
            this.StatusBar.OptionsBar.UseWholeRow = true;
            this.StatusBar.Text = "状态栏";
            // 
            // tlabStatus
            // 
            this.tlabStatus.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
            this.tlabStatus.Caption = "就绪";
            this.tlabStatus.Id = 0;
            this.tlabStatus.Name = "tlabStatus";
            this.tlabStatus.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabStatus.Width = 32;
            // 
            // tlabCustomPanel1
            // 
            this.tlabCustomPanel1.Caption = "Custom Panel1";
            this.tlabCustomPanel1.Id = 1;
            this.tlabCustomPanel1.Name = "tlabCustomPanel1";
            this.tlabCustomPanel1.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabCustomPanel1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // tlabCustomPanel2
            // 
            this.tlabCustomPanel2.Caption = "Custom Panel2";
            this.tlabCustomPanel2.Id = 2;
            this.tlabCustomPanel2.Name = "tlabCustomPanel2";
            this.tlabCustomPanel2.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabCustomPanel2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Edit = this.repositoryItemProgressBar1;
            this.ProgressBar.EditValue = 0;
            this.ProgressBar.Hint = "显示当前工作完成进度";
            this.ProgressBar.Id = 5;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.ProgressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.ProgressBar.Width = 100;
            // 
            // repositoryItemProgressBar1
            // 
            this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            this.repositoryItemProgressBar1.Step = 1;
            // 
            // tlabUser
            // 
            this.tlabUser.Caption = "User";
            this.tlabUser.Glyph = global::Uniframework.StartUp.Properties.Resources.user1_earth;
            this.tlabUser.Hint = "当前登录的系统的用户名称";
            this.tlabUser.Id = 3;
            this.tlabUser.Name = "tlabUser";
            this.tlabUser.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.tlabUser.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // tbtnNetworkStatus
            // 
            this.tbtnNetworkStatus.Glyph = global::Uniframework.StartUp.Properties.Resources.link;
            this.tbtnNetworkStatus.Id = 8;
            this.tbtnNetworkStatus.Name = "tbtnNetworkStatus";
            this.tbtnNetworkStatus.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.tbtnNetworkStatus_ItemDoubleClick);
            // 
            // tlabRequestSize
            // 
            this.tlabRequestSize.Caption = "指令队列(0)";
            this.tlabRequestSize.Hint = "当前未提交到服务器的命令";
            this.tlabRequestSize.Id = 6;
            this.tlabRequestSize.Name = "tlabRequestSize";
            this.tlabRequestSize.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // ZoomBar
            // 
            this.ZoomBar.AutoFillWidth = true;
            this.ZoomBar.Edit = this.repositoryItemZoomTrackBar1;
            this.ZoomBar.EditValue = 0;
            this.ZoomBar.Hint = "缩放、显示比例";
            this.ZoomBar.Id = 7;
            this.ZoomBar.Name = "ZoomBar";
            this.ZoomBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.ZoomBar.Width = 100;
            // 
            // repositoryItemZoomTrackBar1
            // 
            this.repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
            this.repositoryItemZoomTrackBar1.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            // 
            // DockManager
            // 
            this.DockManager.Form = this;
            this.DockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "Progress";
            this.barStaticItem1.Id = 4;
            this.barStaticItem1.Name = "barStaticItem1";
            this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // tabbedMdiManager
            // 
            this.tabbedMdiManager.MdiParent = this;
            // 
            // DeckWorkspace
            // 
            this.DeckWorkspace.Dock = System.Windows.Forms.DockStyle.Top;
            this.DeckWorkspace.Location = new System.Drawing.Point(0, 25);
            this.DeckWorkspace.Name = "DeckWorkspace";
            this.DeckWorkspace.Size = new System.Drawing.Size(792, 60);
            this.DeckWorkspace.TabIndex = 5;
            // 
            // naviWorkspace
            // 
            this.naviWorkspace.ActiveGroup = null;
            this.naviWorkspace.AllowSelectedLink = true;
            this.naviWorkspace.ContentButtonHint = null;
            this.naviWorkspace.Dock = System.Windows.Forms.DockStyle.Left;
            this.naviWorkspace.Location = new System.Drawing.Point(0, 85);
            this.naviWorkspace.Name = "naviWorkspace";
            this.naviWorkspace.OptionsNavPane.ExpandedWidth = 169;
            this.naviWorkspace.Size = new System.Drawing.Size(169, 460);
            this.naviWorkspace.TabIndex = 6;
            this.naviWorkspace.View = new DevExpress.XtraNavBar.ViewInfo.SkinNavigationPaneViewInfoRegistrator();
            // 
            // SplitterControl
            // 
            this.SplitterControl.Location = new System.Drawing.Point(169, 85);
            this.SplitterControl.MinExtra = 80;
            this.SplitterControl.MinSize = 80;
            this.SplitterControl.Name = "SplitterControl";
            this.SplitterControl.Size = new System.Drawing.Size(6, 460);
            this.SplitterControl.TabIndex = 7;
            this.SplitterControl.TabStop = false;
            // 
            // ShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.SplitterControl);
            this.Controls.Add(this.naviWorkspace);
            this.Controls.Add(this.DeckWorkspace);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IsMdiContainer = true;
            this.Name = "ShellForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShellForm";
            this.Load += new System.EventHandler(this.ShellForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShellForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DockManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedMdiManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.naviWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Bar MenuBar;
        private DevExpress.XtraBars.Bar StatusBar;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        public DevExpress.XtraBars.BarManager barManager;
        public DevExpress.XtraTabbedMdi.XtraTabbedMdiManager tabbedMdiManager;
        private DevExpress.XtraBars.BarStaticItem tlabStatus;
        private DevExpress.XtraBars.BarStaticItem tlabCustomPanel1;
        private DevExpress.XtraBars.BarStaticItem tlabCustomPanel2;
        private DevExpress.XtraBars.BarStaticItem tlabUser;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarEditItem ProgressBar;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar1;
        private DevExpress.XtraBars.BarStaticItem tlabRequestSize;
        private DevExpress.XtraBars.BarEditItem ZoomBar;
        private DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar repositoryItemZoomTrackBar1;
        public DevExpress.XtraBars.Docking.DockManager DockManager;
        private DevExpress.XtraBars.BarButtonItem tbtnNetworkStatus;
        private Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace naviWorkspace;
        private DevExpress.XtraEditors.SplitterControl SplitterControl;
        public Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace DeckWorkspace;
    }
}