namespace Uniframework.SmartClient
{
    partial class DynamicHelpView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnBack = new DevExpress.XtraBars.BarButtonItem();
            this.btnForward = new DevExpress.XtraBars.BarButtonItem();
            this.btnHome = new DevExpress.XtraBars.BarButtonItem();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveAs = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager
            // 
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnBack,
            this.btnForward,
            this.btnHome,
            this.btnRefresh,
            this.btnSaveAs});
            this.barManager.MaxItemId = 10;
            // 
            // bar1
            // 
            this.bar1.BarName = "Navibar";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnBack),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnForward),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnHome),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRefresh, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveAs, true)});
            this.bar1.Text = "Navibar";
            // 
            // btnBack
            // 
            this.btnBack.Caption = "向后(&B)";
            this.btnBack.Glyph = global::Uniframework.SmartClient.Properties.Resources.nav_left_green;
            this.btnBack.Hint = "向后";
            this.btnBack.Id = 5;
            this.btnBack.Name = "btnBack";
            this.btnBack.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnBack_ItemClick);
            // 
            // btnForward
            // 
            this.btnForward.Caption = "向前(&F)";
            this.btnForward.Glyph = global::Uniframework.SmartClient.Properties.Resources.nav_right_green;
            this.btnForward.Hint = "向前";
            this.btnForward.Id = 6;
            this.btnForward.Name = "btnForward";
            this.btnForward.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnForward_ItemClick);
            // 
            // btnHome
            // 
            this.btnHome.Caption = "主页(&H)";
            this.btnHome.Glyph = global::Uniframework.SmartClient.Properties.Resources.home;
            this.btnHome.Hint = "主页";
            this.btnHome.Id = 7;
            this.btnHome.Name = "btnHome";
            this.btnHome.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnHome_ItemClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "刷新(&R)";
            this.btnRefresh.Glyph = global::Uniframework.SmartClient.Properties.Resources.refresh;
            this.btnRefresh.Hint = "刷新";
            this.btnRefresh.Id = 8;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRefresh_ItemClick);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Caption = "另存为(&S)";
            this.btnSaveAs.Glyph = global::Uniframework.SmartClient.Properties.Resources.disk_blue_window;
            this.btnSaveAs.Hint = "另存为";
            this.btnSaveAs.Id = 9;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveAs_ItemClick);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 26);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(270, 343);
            this.webBrowser.TabIndex = 4;
            // 
            // DynamicHelpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "DynamicHelpView";
            this.Size = new System.Drawing.Size(270, 369);
            this.Load += new System.EventHandler(this.DynamicHelpView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        public System.Windows.Forms.WebBrowser webBrowser;
        private DevExpress.XtraBars.BarButtonItem btnBack;
        private DevExpress.XtraBars.BarButtonItem btnForward;
        private DevExpress.XtraBars.BarButtonItem btnHome;
        private DevExpress.XtraBars.BarButtonItem btnRefresh;
        private DevExpress.XtraBars.BarButtonItem btnSaveAs;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
