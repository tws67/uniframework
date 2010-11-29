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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.MenuBar = new DevExpress.XtraBars.Bar();
            this.StatusBar = new DevExpress.XtraBars.Bar();
            this.tlabStatus = new DevExpress.XtraBars.BarStaticItem();
            this.tlabCustomPanel1 = new DevExpress.XtraBars.BarStaticItem();
            this.tlabCustomPanel2 = new DevExpress.XtraBars.BarStaticItem();
            this.ProgressBar = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.tbtnNetworkStatus = new DevExpress.XtraBars.BarButtonItem();
            this.tlabRequestSize = new DevExpress.XtraBars.BarStaticItem();
            this.ZoomBar = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemZoomTrackBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemZoomTrackBar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.DockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.tlabUser = new DevExpress.XtraBars.BarStaticItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.edtAddress = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemMRUEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMRUEdit();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.tabbedMdiManager = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
            this.naviWorkspace = new Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace();
            this.SplitterControl = new DevExpress.XtraEditors.SplitterControl();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DockManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMRUEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
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
            this.tbtnNetworkStatus,
            this.edtAddress});
            this.barManager.MainMenu = this.MenuBar;
            this.barManager.MaxItemId = 11;
            this.barManager.MdiMenuMergeStyle = DevExpress.XtraBars.BarMdiMenuMergeStyle.Always;
            this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemProgressBar1,
            this.repositoryItemZoomTrackBar1,
            this.repositoryItemMRUEdit1,
            this.repositoryItemButtonEdit1});
            this.barManager.StatusBar = this.StatusBar;
            // 
            // MenuBar
            // 
            this.MenuBar.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.MenuBar.BarName = "MenuBar";
            this.MenuBar.CanDockStyle = ((DevExpress.XtraBars.BarCanDockStyle)((((((DevExpress.XtraBars.BarCanDockStyle.Floating | DevExpress.XtraBars.BarCanDockStyle.Left)
                        | DevExpress.XtraBars.BarCanDockStyle.Top)
                        | DevExpress.XtraBars.BarCanDockStyle.Right)
                        | DevExpress.XtraBars.BarCanDockStyle.Bottom)
                        | DevExpress.XtraBars.BarCanDockStyle.Standalone)));
            this.MenuBar.DockCol = 0;
            this.MenuBar.DockRow = 0;
            this.MenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.MenuBar.OptionsBar.AllowQuickCustomization = false;
            this.MenuBar.OptionsBar.BarState = DevExpress.XtraBars.BarState.Expanded;
            this.MenuBar.OptionsBar.DisableClose = true;
            this.MenuBar.OptionsBar.DisableCustomization = true;
            this.MenuBar.OptionsBar.UseWholeRow = true;
            this.MenuBar.Text = "主菜单";
            // 
            // StatusBar
            // 
            this.StatusBar.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.StatusBar.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.StatusBar.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.StatusBar.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.StatusBar.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.StatusBar.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.StatusBar.BarName = "StatusBar";
            this.StatusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.StatusBar.DockCol = 0;
            this.StatusBar.DockRow = 0;
            this.StatusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.StatusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabStatus),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabCustomPanel1, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabCustomPanel2),
            new DevExpress.XtraBars.LinkPersistInfo(((DevExpress.XtraBars.BarLinkUserDefines)((DevExpress.XtraBars.BarLinkUserDefines.PaintStyle | DevExpress.XtraBars.BarLinkUserDefines.Width))), this.ProgressBar, "", true, true, true, 106, null, DevExpress.XtraBars.BarItemPaintStyle.Standard),
            new DevExpress.XtraBars.LinkPersistInfo(this.tbtnNetworkStatus, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.tlabRequestSize, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.ZoomBar)});
            this.StatusBar.OptionsBar.AllowQuickCustomization = false;
            this.StatusBar.OptionsBar.BarState = DevExpress.XtraBars.BarState.Expanded;
            this.StatusBar.OptionsBar.DrawDragBorder = false;
            this.StatusBar.OptionsBar.UseWholeRow = true;
            this.StatusBar.Text = "状态栏(&S)";
            // 
            // tlabStatus
            // 
            this.tlabStatus.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.tlabStatus.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabStatus.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabStatus.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
            this.tlabStatus.Caption = "就绪";
            this.tlabStatus.Id = 0;
            this.tlabStatus.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.tlabStatus.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.tlabStatus.Name = "tlabStatus";
            this.tlabStatus.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabStatus.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            this.tlabStatus.Width = 32;
            // 
            // tlabCustomPanel1
            // 
            this.tlabCustomPanel1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.tlabCustomPanel1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabCustomPanel1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabCustomPanel1.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Content;
            this.tlabCustomPanel1.Caption = "Custom Panel1";
            this.tlabCustomPanel1.Id = 1;
            this.tlabCustomPanel1.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.tlabCustomPanel1.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.tlabCustomPanel1.Name = "tlabCustomPanel1";
            this.tlabCustomPanel1.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabCustomPanel1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // tlabCustomPanel2
            // 
            this.tlabCustomPanel2.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.tlabCustomPanel2.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabCustomPanel2.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabCustomPanel2.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Content;
            this.tlabCustomPanel2.Caption = "Custom Panel2";
            this.tlabCustomPanel2.Id = 2;
            this.tlabCustomPanel2.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.tlabCustomPanel2.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.tlabCustomPanel2.Name = "tlabCustomPanel2";
            this.tlabCustomPanel2.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabCustomPanel2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.ProgressBar.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.ProgressBar.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.ProgressBar.CaptionAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.ProgressBar.Edit = this.repositoryItemProgressBar1;
            this.ProgressBar.EditorShowMode = DevExpress.Utils.EditorShowMode.Default;
            this.ProgressBar.EditValue = 0;
            this.ProgressBar.Hint = "显示当前工作完成进度";
            this.ProgressBar.Id = 5;
            this.ProgressBar.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.ProgressBar.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.ProgressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.ProgressBar.Width = 100;
            // 
            // repositoryItemProgressBar1
            // 
            this.repositoryItemProgressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Default;
            this.repositoryItemProgressBar1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemProgressBar1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemProgressBar1.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemProgressBar1.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemProgressBar1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemProgressBar1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemProgressBar1.AppearanceDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemProgressBar1.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemProgressBar1.AppearanceDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemProgressBar1.AppearanceDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemProgressBar1.AppearanceDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemProgressBar1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemProgressBar1.AppearanceFocused.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemProgressBar1.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemProgressBar1.AppearanceFocused.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemProgressBar1.AppearanceFocused.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemProgressBar1.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemProgressBar1.AppearanceFocused.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemProgressBar1.AppearanceReadOnly.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemProgressBar1.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemProgressBar1.AppearanceReadOnly.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemProgressBar1.AppearanceReadOnly.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemProgressBar1.AppearanceReadOnly.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemProgressBar1.AppearanceReadOnly.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemProgressBar1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.repositoryItemProgressBar1.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.repositoryItemProgressBar1.ExportMode = DevExpress.XtraEditors.Repository.ExportMode.Default;
            this.repositoryItemProgressBar1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            this.repositoryItemProgressBar1.ProgressKind = DevExpress.XtraEditors.Controls.ProgressKind.Horizontal;
            this.repositoryItemProgressBar1.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Broken;
            this.repositoryItemProgressBar1.Step = 1;
            // 
            // tbtnNetworkStatus
            // 
            this.tbtnNetworkStatus.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.tbtnNetworkStatus.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tbtnNetworkStatus.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tbtnNetworkStatus.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Default;
            this.tbtnNetworkStatus.Glyph = global::Uniframework.StartUp.Properties.Resources.link;
            this.tbtnNetworkStatus.Id = 8;
            this.tbtnNetworkStatus.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.tbtnNetworkStatus.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.tbtnNetworkStatus.Name = "tbtnNetworkStatus";
            this.tbtnNetworkStatus.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            this.tbtnNetworkStatus.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.tbtnNetworkStatus_ItemDoubleClick);
            // 
            // tlabRequestSize
            // 
            this.tlabRequestSize.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.tlabRequestSize.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabRequestSize.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabRequestSize.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Content;
            this.tlabRequestSize.Caption = "指令队列(0)";
            this.tlabRequestSize.Hint = "当前未提交到服务器的命令";
            this.tlabRequestSize.Id = 6;
            this.tlabRequestSize.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.tlabRequestSize.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.tlabRequestSize.Name = "tlabRequestSize";
            this.tlabRequestSize.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabRequestSize.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // ZoomBar
            // 
            this.ZoomBar.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.ZoomBar.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.ZoomBar.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.ZoomBar.AutoFillWidth = true;
            this.ZoomBar.CaptionAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.ZoomBar.Edit = this.repositoryItemZoomTrackBar1;
            this.ZoomBar.EditorShowMode = DevExpress.Utils.EditorShowMode.Default;
            this.ZoomBar.EditValue = 0;
            this.ZoomBar.Hint = "缩放、显示比例";
            this.ZoomBar.Id = 7;
            this.ZoomBar.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.ZoomBar.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.ZoomBar.Name = "ZoomBar";
            this.ZoomBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.ZoomBar.Width = 100;
            // 
            // repositoryItemZoomTrackBar1
            // 
            this.repositoryItemZoomTrackBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Default;
            this.repositoryItemZoomTrackBar1.Alignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemZoomTrackBar1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemZoomTrackBar1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemZoomTrackBar1.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemZoomTrackBar1.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemZoomTrackBar1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemZoomTrackBar1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemZoomTrackBar1.AppearanceDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemZoomTrackBar1.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemZoomTrackBar1.AppearanceDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemZoomTrackBar1.AppearanceDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemZoomTrackBar1.AppearanceDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemZoomTrackBar1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemZoomTrackBar1.AppearanceFocused.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemZoomTrackBar1.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemZoomTrackBar1.AppearanceFocused.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemZoomTrackBar1.AppearanceFocused.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemZoomTrackBar1.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemZoomTrackBar1.AppearanceFocused.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemZoomTrackBar1.AppearanceReadOnly.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemZoomTrackBar1.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemZoomTrackBar1.AppearanceReadOnly.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemZoomTrackBar1.AppearanceReadOnly.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemZoomTrackBar1.AppearanceReadOnly.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemZoomTrackBar1.AppearanceReadOnly.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemZoomTrackBar1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.repositoryItemZoomTrackBar1.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.repositoryItemZoomTrackBar1.ExportMode = DevExpress.XtraEditors.Repository.ExportMode.Default;
            this.repositoryItemZoomTrackBar1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.repositoryItemZoomTrackBar1.Name = "repositoryItemZoomTrackBar1";
            this.repositoryItemZoomTrackBar1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.repositoryItemZoomTrackBar1.ScrollThumbStyle = DevExpress.XtraEditors.Repository.ScrollThumbStyle.ArrowDownRight;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.barDockControlTop.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.barDockControlTop.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.barDockControlTop.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.barDockControlTop.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.barDockControlTop.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(924, 25);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.barDockControlBottom.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.barDockControlBottom.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.barDockControlBottom.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.barDockControlBottom.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.barDockControlBottom.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 634);
            this.barDockControlBottom.Size = new System.Drawing.Size(924, 31);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.barDockControlLeft.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.barDockControlLeft.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.barDockControlLeft.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.barDockControlLeft.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.barDockControlLeft.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 25);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 609);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.barDockControlRight.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.barDockControlRight.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.barDockControlRight.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.barDockControlRight.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.barDockControlRight.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(924, 25);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 609);
            // 
            // DockManager
            // 
            this.DockManager.DockMode = DevExpress.XtraBars.Docking.Helpers.DockMode.VS2005;
            this.DockManager.Form = this;
            this.DockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // tlabUser
            // 
            this.tlabUser.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.tlabUser.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabUser.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tlabUser.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Content;
            this.tlabUser.Caption = "User";
            this.tlabUser.Glyph = global::Uniframework.StartUp.Properties.Resources.user1_earth;
            this.tlabUser.Hint = "当前登录的系统的用户名称";
            this.tlabUser.Id = 3;
            this.tlabUser.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.tlabUser.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.tlabUser.Name = "tlabUser";
            this.tlabUser.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.tlabUser.TextAlignment = System.Drawing.StringAlignment.Near;
            this.tlabUser.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.barStaticItem1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.barStaticItem1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.barStaticItem1.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Content;
            this.barStaticItem1.Caption = "Progress";
            this.barStaticItem1.Id = 4;
            this.barStaticItem1.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.barStaticItem1.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.barStaticItem1.Name = "barStaticItem1";
            this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
            this.barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            // 
            // edtAddress
            // 
            this.edtAddress.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Default;
            this.edtAddress.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.edtAddress.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.edtAddress.AutoFillWidth = true;
            this.edtAddress.Caption = "地址(&D):";
            this.edtAddress.CaptionAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.edtAddress.Edit = this.repositoryItemMRUEdit1;
            this.edtAddress.EditorShowMode = DevExpress.Utils.EditorShowMode.Default;
            this.edtAddress.Id = 9;
            this.edtAddress.IEBehavior = true;
            this.edtAddress.ItemClickFireMode = DevExpress.XtraBars.BarItemEventFireMode.Default;
            this.edtAddress.MergeType = DevExpress.XtraBars.BarMenuMerge.Add;
            this.edtAddress.Name = "edtAddress";
            this.edtAddress.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            // 
            // repositoryItemMRUEdit1
            // 
            this.repositoryItemMRUEdit1.AccessibleRole = System.Windows.Forms.AccessibleRole.Default;
            this.repositoryItemMRUEdit1.AllowNullInput = DevExpress.Utils.DefaultBoolean.Default;
            this.repositoryItemMRUEdit1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemMRUEdit1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemMRUEdit1.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemMRUEdit1.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemMRUEdit1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemMRUEdit1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemMRUEdit1.AppearanceDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemMRUEdit1.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemMRUEdit1.AppearanceDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemMRUEdit1.AppearanceDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemMRUEdit1.AppearanceDropDown.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemMRUEdit1.AppearanceDropDown.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceDropDown.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemMRUEdit1.AppearanceDropDown.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemMRUEdit1.AppearanceDropDown.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceDropDown.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemMRUEdit1.AppearanceFocused.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemMRUEdit1.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceFocused.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemMRUEdit1.AppearanceFocused.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemMRUEdit1.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceFocused.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemMRUEdit1.AppearanceReadOnly.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemMRUEdit1.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceReadOnly.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemMRUEdit1.AppearanceReadOnly.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemMRUEdit1.AppearanceReadOnly.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemMRUEdit1.AppearanceReadOnly.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemMRUEdit1.AutoHeight = false;
            this.repositoryItemMRUEdit1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            serializableAppearanceObject1.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemMRUEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            this.repositoryItemMRUEdit1.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.repositoryItemMRUEdit1.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.repositoryItemMRUEdit1.ExportMode = DevExpress.XtraEditors.Repository.ExportMode.Default;
            this.repositoryItemMRUEdit1.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Default;
            this.repositoryItemMRUEdit1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.repositoryItemMRUEdit1.Name = "repositoryItemMRUEdit1";
            this.repositoryItemMRUEdit1.PopupResizeMode = DevExpress.XtraEditors.Controls.ResizeMode.Default;
            this.repositoryItemMRUEdit1.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.SingleClick;
            this.repositoryItemMRUEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AccessibleRole = System.Windows.Forms.AccessibleRole.Default;
            this.repositoryItemButtonEdit1.AllowNullInput = DevExpress.Utils.DefaultBoolean.Default;
            this.repositoryItemButtonEdit1.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemButtonEdit1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemButtonEdit1.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemButtonEdit1.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemButtonEdit1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemButtonEdit1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemButtonEdit1.AppearanceDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemButtonEdit1.AppearanceDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemButtonEdit1.AppearanceDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemButtonEdit1.AppearanceDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemButtonEdit1.AppearanceDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemButtonEdit1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemButtonEdit1.AppearanceFocused.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemButtonEdit1.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemButtonEdit1.AppearanceFocused.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemButtonEdit1.AppearanceFocused.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemButtonEdit1.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemButtonEdit1.AppearanceFocused.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemButtonEdit1.AppearanceReadOnly.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemButtonEdit1.AppearanceReadOnly.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemButtonEdit1.AppearanceReadOnly.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.repositoryItemButtonEdit1.AppearanceReadOnly.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.repositoryItemButtonEdit1.AppearanceReadOnly.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.repositoryItemButtonEdit1.AppearanceReadOnly.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            serializableAppearanceObject2.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            serializableAppearanceObject3.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            serializableAppearanceObject4.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, "", null, null, false),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject3, "", null, null, true),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Right, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject4, "", null, null, true)});
            this.repositoryItemButtonEdit1.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.repositoryItemButtonEdit1.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.repositoryItemButtonEdit1.ExportMode = DevExpress.XtraEditors.Repository.ExportMode.Default;
            this.repositoryItemButtonEdit1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.repositoryItemButtonEdit1.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.Default;
            this.repositoryItemButtonEdit1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            // 
            // tabbedMdiManager
            // 
            this.tabbedMdiManager.AllowDragDrop = DevExpress.Utils.DefaultBoolean.Default;
            this.tabbedMdiManager.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.tabbedMdiManager.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tabbedMdiManager.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.tabbedMdiManager.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.tabbedMdiManager.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.tabbedMdiManager.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tabbedMdiManager.AppearancePage.Header.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.tabbedMdiManager.AppearancePage.Header.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tabbedMdiManager.AppearancePage.Header.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.tabbedMdiManager.AppearancePage.Header.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.tabbedMdiManager.AppearancePage.Header.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.tabbedMdiManager.AppearancePage.Header.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tabbedMdiManager.AppearancePage.HeaderActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.tabbedMdiManager.AppearancePage.HeaderActive.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tabbedMdiManager.AppearancePage.HeaderActive.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.tabbedMdiManager.AppearancePage.HeaderActive.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.tabbedMdiManager.AppearancePage.HeaderActive.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.tabbedMdiManager.AppearancePage.HeaderActive.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tabbedMdiManager.AppearancePage.HeaderDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.tabbedMdiManager.AppearancePage.HeaderDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tabbedMdiManager.AppearancePage.HeaderDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.tabbedMdiManager.AppearancePage.HeaderDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.tabbedMdiManager.AppearancePage.HeaderDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.tabbedMdiManager.AppearancePage.HeaderDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tabbedMdiManager.AppearancePage.HeaderHotTracked.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.tabbedMdiManager.AppearancePage.HeaderHotTracked.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tabbedMdiManager.AppearancePage.HeaderHotTracked.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.tabbedMdiManager.AppearancePage.HeaderHotTracked.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.tabbedMdiManager.AppearancePage.HeaderHotTracked.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.tabbedMdiManager.AppearancePage.HeaderHotTracked.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tabbedMdiManager.AppearancePage.PageClient.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.tabbedMdiManager.AppearancePage.PageClient.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.tabbedMdiManager.AppearancePage.PageClient.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.tabbedMdiManager.AppearancePage.PageClient.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.tabbedMdiManager.AppearancePage.PageClient.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.tabbedMdiManager.AppearancePage.PageClient.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.tabbedMdiManager.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.tabbedMdiManager.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.tabbedMdiManager.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.Default;
            this.tabbedMdiManager.FloatOnDoubleClick = DevExpress.Utils.DefaultBoolean.Default;
            this.tabbedMdiManager.FloatOnDrag = DevExpress.Utils.DefaultBoolean.Default;
            this.tabbedMdiManager.HeaderAutoFill = DevExpress.Utils.DefaultBoolean.Default;
            this.tabbedMdiManager.HeaderButtons = DevExpress.XtraTab.TabButtons.Default;
            this.tabbedMdiManager.HeaderButtonsShowMode = DevExpress.XtraTab.TabButtonShowMode.Default;
            this.tabbedMdiManager.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Top;
            this.tabbedMdiManager.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Default;
            this.tabbedMdiManager.MdiParent = this;
            this.tabbedMdiManager.MultiLine = DevExpress.Utils.DefaultBoolean.Default;
            this.tabbedMdiManager.PageImagePosition = DevExpress.XtraTab.TabPageImagePosition.Near;
            this.tabbedMdiManager.SetNextMdiChildMode = DevExpress.XtraTabbedMdi.SetNextMdiChildMode.Default;
            this.tabbedMdiManager.ShowHeaderFocus = DevExpress.Utils.DefaultBoolean.Default;
            this.tabbedMdiManager.ShowToolTips = DevExpress.Utils.DefaultBoolean.Default;
            // 
            // naviWorkspace
            // 
            this.naviWorkspace.ActiveGroup = null;
            this.naviWorkspace.Appearance.Background.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.Background.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.Background.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.Background.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.Background.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.Background.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.Button.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.Button.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.Button.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.Button.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.Button.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.Button.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ButtonDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ButtonDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ButtonDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ButtonDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ButtonDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ButtonDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ButtonHotTracked.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ButtonHotTracked.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ButtonHotTracked.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ButtonHotTracked.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ButtonHotTracked.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ButtonHotTracked.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ButtonPressed.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ButtonPressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ButtonPressed.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ButtonPressed.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ButtonPressed.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ButtonPressed.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.GroupBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.GroupBackground.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.GroupBackground.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.GroupBackground.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.GroupBackground.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.GroupBackground.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.GroupHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.GroupHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeader.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.GroupHeader.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.GroupHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.GroupHeaderActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.GroupHeaderActive.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeaderActive.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.GroupHeaderActive.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.GroupHeaderActive.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeaderActive.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.GroupHeaderHotTracked.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.GroupHeaderHotTracked.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeaderHotTracked.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.GroupHeaderHotTracked.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.GroupHeaderHotTracked.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeaderHotTracked.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.GroupHeaderPressed.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.GroupHeaderPressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeaderPressed.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.GroupHeaderPressed.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.GroupHeaderPressed.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.GroupHeaderPressed.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.Hint.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.Hint.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.Hint.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.Hint.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.Hint.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.Hint.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.Item.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.Item.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.Item.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.Item.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.Item.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.Item.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ItemActive.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ItemActive.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ItemActive.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ItemActive.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ItemActive.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ItemActive.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ItemDisabled.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ItemDisabled.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ItemDisabled.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ItemDisabled.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ItemDisabled.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ItemDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ItemHotTracked.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ItemHotTracked.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ItemHotTracked.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ItemHotTracked.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ItemHotTracked.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ItemHotTracked.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.ItemPressed.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.ItemPressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.ItemPressed.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.ItemPressed.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.ItemPressed.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.ItemPressed.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.LinkDropTarget.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.LinkDropTarget.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.LinkDropTarget.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.LinkDropTarget.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.LinkDropTarget.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.LinkDropTarget.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.NavigationPaneHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.NavigationPaneHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.NavigationPaneHeader.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.NavigationPaneHeader.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.NavigationPaneHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.NavigationPaneHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.NavPaneContentButton.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.NavPaneContentButton.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButton.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.NavPaneContentButton.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.NavPaneContentButton.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButton.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonHotTracked.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.NavPaneContentButtonHotTracked.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonHotTracked.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonHotTracked.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonHotTracked.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonHotTracked.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonPressed.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.NavPaneContentButtonPressed.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonPressed.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonPressed.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonPressed.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonPressed.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonReleased.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.naviWorkspace.Appearance.NavPaneContentButtonReleased.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonReleased.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonReleased.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonReleased.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.naviWorkspace.Appearance.NavPaneContentButtonReleased.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.naviWorkspace.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.naviWorkspace.ContentButtonHint = null;
            this.naviWorkspace.Dock = System.Windows.Forms.DockStyle.Left;
            this.naviWorkspace.DragDropFlags = ((DevExpress.XtraNavBar.NavBarDragDrop)((DevExpress.XtraNavBar.NavBarDragDrop.AllowDrag | DevExpress.XtraNavBar.NavBarDragDrop.AllowDrop)));
            this.naviWorkspace.Location = new System.Drawing.Point(0, 25);
            this.naviWorkspace.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.naviWorkspace.Name = "naviWorkspace";
            this.naviWorkspace.OptionsNavPane.ExpandButtonMode = DevExpress.Utils.Controls.ExpandButtonMode.Normal;
            this.naviWorkspace.OptionsNavPane.ExpandedWidth = 169;
            this.naviWorkspace.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
            this.naviWorkspace.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.Default;
            this.naviWorkspace.Size = new System.Drawing.Size(197, 609);
            this.naviWorkspace.SkinExplorerBarViewScrollStyle = DevExpress.XtraNavBar.SkinExplorerBarViewScrollStyle.Default;
            this.naviWorkspace.TabIndex = 6;
            this.naviWorkspace.View = new DevExpress.XtraNavBar.ViewInfo.SkinNavigationPaneViewInfoRegistrator();
            // 
            // SplitterControl
            // 
            this.SplitterControl.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.SplitterControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.SplitterControl.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.SplitterControl.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.SplitterControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.SplitterControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.SplitterControl.Location = new System.Drawing.Point(197, 25);
            this.SplitterControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.SplitterControl.Name = "SplitterControl";
            this.SplitterControl.Size = new System.Drawing.Size(6, 609);
            this.SplitterControl.TabIndex = 7;
            this.SplitterControl.TabStop = false;
            // 
            // ShellForm
            // 
            this.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.Appearance.TextOptions.HotkeyPrefix = DevExpress.Utils.HKeyPrefix.Default;
            this.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.Default;
            this.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Default;
            this.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Default;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 665);
            this.Controls.Add(this.SplitterControl);
            this.Controls.Add(this.naviWorkspace);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.IsMdiContainer = true;
            this.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            this.Name = "ShellForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShellForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShellForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShellForm_FormClosed);
            this.Load += new System.EventHandler(this.ShellForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemZoomTrackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DockManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMRUEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabbedMdiManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.naviWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

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
        private DevExpress.XtraBars.BarEditItem edtAddress;
        private DevExpress.XtraEditors.Repository.RepositoryItemMRUEdit repositoryItemMRUEdit1;
        private DevExpress.XtraBars.Bar StatusBar;
        private DevExpress.XtraBars.Bar MenuBar;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
    }
}