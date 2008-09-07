namespace Uniframework.SmartClient.WorkItems.Setting
{
    partial class SettingView
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.naviWorkspace = new Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace();
            this.defaultGroup = new DevExpress.XtraNavBar.NavBarGroup();
            this.btnDefault = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.deckWorkspace = new Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.naviWorkspace)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.naviWorkspace);
            this.panelControl1.Location = new System.Drawing.Point(11, 12);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(169, 281);
            this.panelControl1.TabIndex = 0;
            // 
            // naviWorkspace
            // 
            this.naviWorkspace.ActiveGroup = this.defaultGroup;
            this.naviWorkspace.ContentButtonHint = null;
            this.naviWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.naviWorkspace.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.defaultGroup});
            this.naviWorkspace.Location = new System.Drawing.Point(2, 2);
            this.naviWorkspace.Name = "naviWorkspace";
            this.naviWorkspace.OptionsNavPane.ExpandedWidth = 165;
            this.naviWorkspace.Size = new System.Drawing.Size(165, 277);
            this.naviWorkspace.TabIndex = 2;
            this.naviWorkspace.View = new DevExpress.XtraNavBar.ViewInfo.SkinExplorerBarViewInfoRegistrator();
            // 
            // defaultGroup
            // 
            this.defaultGroup.Caption = "系统默认配置";
            this.defaultGroup.Expanded = true;
            this.defaultGroup.Name = "defaultGroup";
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(391, 301);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 23);
            this.btnDefault.TabIndex = 4;
            this.btnDefault.Text = "默认值(&D)";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(472, 301);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定(&O)";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(553, 301);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "取消";
            // 
            // deckWorkspace
            // 
            this.deckWorkspace.Location = new System.Drawing.Point(186, 12);
            this.deckWorkspace.Name = "deckWorkspace";
            this.deckWorkspace.Size = new System.Drawing.Size(440, 270);
            this.deckWorkspace.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(185, 291);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(443, 2);
            this.label1.TabIndex = 8;
            // 
            // SettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.deckWorkspace);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.panelControl1);
            this.Name = "SettingView";
            this.Size = new System.Drawing.Size(639, 330);
            this.Load += new System.EventHandler(this.SettingView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.naviWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace naviWorkspace;
        private DevExpress.XtraNavBar.NavBarGroup defaultGroup;
        private DevExpress.XtraEditors.SimpleButton btnDefault;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace deckWorkspace;
        private System.Windows.Forms.Label label1;

    }
}
