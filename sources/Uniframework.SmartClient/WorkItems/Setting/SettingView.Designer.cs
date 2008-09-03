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
            this.xtraNavBarWorkspace1 = new Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace();
            this.navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.deckWorkspace = new Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace();
            this.btnDefault = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraNavBarWorkspace1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraNavBarWorkspace1);
            this.panelControl1.Location = new System.Drawing.Point(11, 12);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(169, 283);
            this.panelControl1.TabIndex = 0;
            // 
            // xtraNavBarWorkspace1
            // 
            this.xtraNavBarWorkspace1.ActiveGroup = this.navBarGroup1;
            this.xtraNavBarWorkspace1.ContentButtonHint = null;
            this.xtraNavBarWorkspace1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraNavBarWorkspace1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroup1});
            this.xtraNavBarWorkspace1.Location = new System.Drawing.Point(2, 2);
            this.xtraNavBarWorkspace1.Name = "xtraNavBarWorkspace1";
            this.xtraNavBarWorkspace1.OptionsNavPane.ExpandedWidth = 165;
            this.xtraNavBarWorkspace1.Size = new System.Drawing.Size(165, 279);
            this.xtraNavBarWorkspace1.TabIndex = 0;
            this.xtraNavBarWorkspace1.Text = "xtraNavBarWorkspace1";
            // 
            // navBarGroup1
            // 
            this.navBarGroup1.Caption = "navBarGroup1";
            this.navBarGroup1.Expanded = true;
            this.navBarGroup1.Name = "navBarGroup1";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.deckWorkspace);
            this.panelControl2.Location = new System.Drawing.Point(186, 12);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(442, 283);
            this.panelControl2.TabIndex = 1;
            // 
            // deckWorkspace
            // 
            this.deckWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deckWorkspace.Location = new System.Drawing.Point(2, 2);
            this.deckWorkspace.Name = "deckWorkspace";
            this.deckWorkspace.Size = new System.Drawing.Size(438, 279);
            this.deckWorkspace.TabIndex = 0;
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(391, 301);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 23);
            this.btnDefault.TabIndex = 3;
            this.btnDefault.Text = "默认值(&D)";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(472, 301);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确定(&O)";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(553, 301);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            // 
            // SettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Name = "SettingView";
            this.Size = new System.Drawing.Size(639, 333);
            this.Load += new System.EventHandler(this.SettingView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraNavBarWorkspace1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace xtraNavBarWorkspace1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private Microsoft.Practices.CompositeUI.WinForms.DeckWorkspace deckWorkspace;
        private DevExpress.XtraEditors.SimpleButton btnDefault;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;

    }
}
