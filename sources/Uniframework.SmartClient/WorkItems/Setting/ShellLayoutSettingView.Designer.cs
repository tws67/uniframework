namespace Uniframework.SmartClient.WorkItems.Setting
{
    partial class ShellLayoutSettingView
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
            this.gcWindowLayout = new DevExpress.XtraEditors.GroupControl();
            this.rgLayout = new DevExpress.XtraEditors.RadioGroup();
            this.gcNaviPane = new DevExpress.XtraEditors.GroupControl();
            this.rgNaviPane = new DevExpress.XtraEditors.RadioGroup();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cbeSkin = new DevExpress.XtraEditors.ComboBoxEdit();
            this.chkShowStatusBar = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gcWindowLayout)).BeginInit();
            this.gcWindowLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgLayout.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcNaviPane)).BeginInit();
            this.gcNaviPane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgNaviPane.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeSkin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowStatusBar.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gcWindowLayout
            // 
            this.gcWindowLayout.Controls.Add(this.rgLayout);
            this.gcWindowLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.gcWindowLayout.Location = new System.Drawing.Point(0, 0);
            this.gcWindowLayout.Name = "gcWindowLayout";
            this.gcWindowLayout.Size = new System.Drawing.Size(440, 76);
            this.gcWindowLayout.TabIndex = 0;
            this.gcWindowLayout.Text = "窗口布局样式";
            // 
            // rgLayout
            // 
            this.rgLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgLayout.EditValue = 0;
            this.rgLayout.Location = new System.Drawing.Point(2, 21);
            this.rgLayout.Name = "rgLayout";
            this.rgLayout.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "选项卡式(&T)"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "多窗口MDI式布局(&M)")});
            this.rgLayout.Size = new System.Drawing.Size(436, 53);
            this.rgLayout.TabIndex = 0;
            // 
            // gcNaviPane
            // 
            this.gcNaviPane.Controls.Add(this.rgNaviPane);
            this.gcNaviPane.Location = new System.Drawing.Point(0, 85);
            this.gcNaviPane.Name = "gcNaviPane";
            this.gcNaviPane.Size = new System.Drawing.Size(440, 76);
            this.gcNaviPane.TabIndex = 1;
            this.gcNaviPane.Text = "导航栏样式";
            // 
            // rgNaviPane
            // 
            this.rgNaviPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rgNaviPane.EditValue = "SkinNavigationPane";
            this.rgNaviPane.Location = new System.Drawing.Point(2, 21);
            this.rgNaviPane.Name = "rgNaviPane";
            this.rgNaviPane.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem("SkinNavigationPane", "Outlook样式(&O)"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("SkinExplorerBarView", "Windows浏览器样式(&E)")});
            this.rgNaviPane.Size = new System.Drawing.Size(436, 53);
            this.rgNaviPane.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(5, 167);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(89, 14);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "系统界面皮肤(&K)";
            // 
            // labelControl2
            // 
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.labelControl2.Location = new System.Drawing.Point(112, 175);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(328, 2);
            this.labelControl2.TabIndex = 3;
            // 
            // cbeSkin
            // 
            this.cbeSkin.Location = new System.Drawing.Point(5, 187);
            this.cbeSkin.Name = "cbeSkin";
            this.cbeSkin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeSkin.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbeSkin.Size = new System.Drawing.Size(173, 21);
            this.cbeSkin.TabIndex = 4;
            // 
            // chkShowStatusBar
            // 
            this.chkShowStatusBar.EditValue = true;
            this.chkShowStatusBar.Location = new System.Drawing.Point(3, 214);
            this.chkShowStatusBar.Name = "chkShowStatusBar";
            this.chkShowStatusBar.Properties.Caption = "显示系统状态栏(&S)";
            this.chkShowStatusBar.Size = new System.Drawing.Size(138, 19);
            this.chkShowStatusBar.TabIndex = 5;
            // 
            // ShellLayoutSettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkShowStatusBar);
            this.Controls.Add(this.cbeSkin);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.gcNaviPane);
            this.Controls.Add(this.gcWindowLayout);
            this.Name = "ShellLayoutSettingView";
            this.Size = new System.Drawing.Size(440, 270);
            this.Load += new System.EventHandler(this.ShellLayoutSettingView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcWindowLayout)).EndInit();
            this.gcWindowLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgLayout.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcNaviPane)).EndInit();
            this.gcNaviPane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgNaviPane.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeSkin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkShowStatusBar.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl gcWindowLayout;
        private DevExpress.XtraEditors.RadioGroup rgLayout;
        private DevExpress.XtraEditors.GroupControl gcNaviPane;
        private DevExpress.XtraEditors.RadioGroup rgNaviPane;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ComboBoxEdit cbeSkin;
        private DevExpress.XtraEditors.CheckEdit chkShowStatusBar;
    }
}
