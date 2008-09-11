namespace Uniframework.SmartClient.Views
{
    partial class frmAbout
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
            this.TopPanel = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labProduct = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labVersion = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labNetVersion = new DevExpress.XtraEditors.LabelControl();
            this.btnCopyInfo = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labCompany = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labCopyright = new DevExpress.XtraEditors.LabelControl();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnSysInfo = new DevExpress.XtraEditors.SimpleButton();
            this.labWarning = new DevExpress.XtraEditors.LabelControl();
            this.lvModules = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.TopPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(552, 60);
            this.TopPanel.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 66);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(52, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "软件名称:";
            // 
            // labProduct
            // 
            this.labProduct.Location = new System.Drawing.Point(70, 66);
            this.labProduct.Name = "labProduct";
            this.labProduct.Size = new System.Drawing.Size(61, 14);
            this.labProduct.TabIndex = 2;
            this.labProduct.Text = "<Product>";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(36, 86);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(28, 14);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "版本:";
            // 
            // labVersion
            // 
            this.labVersion.Location = new System.Drawing.Point(70, 86);
            this.labVersion.Name = "labVersion";
            this.labVersion.Size = new System.Drawing.Size(58, 14);
            this.labVersion.TabIndex = 4;
            this.labVersion.Text = "<Version>";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(274, 66);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(91, 14);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = ".NET Framework";
            // 
            // labNetVersion
            // 
            this.labNetVersion.Location = new System.Drawing.Point(371, 66);
            this.labNetVersion.Name = "labNetVersion";
            this.labNetVersion.Size = new System.Drawing.Size(89, 14);
            this.labNetVersion.TabIndex = 6;
            this.labNetVersion.Text = "<.NET Version>";
            // 
            // btnCopyInfo
            // 
            this.btnCopyInfo.Location = new System.Drawing.Point(465, 106);
            this.btnCopyInfo.Name = "btnCopyInfo";
            this.btnCopyInfo.Size = new System.Drawing.Size(75, 23);
            this.btnCopyInfo.TabIndex = 8;
            this.btnCopyInfo.Text = "复制(&C)";
            this.btnCopyInfo.Click += new System.EventHandler(this.btnCopyInfo_Click);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(12, 198);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(52, 14);
            this.labelControl4.TabIndex = 9;
            this.labelControl4.Text = "软件公司:";
            // 
            // labCompany
            // 
            this.labCompany.Location = new System.Drawing.Point(70, 198);
            this.labCompany.Name = "labCompany";
            this.labCompany.Size = new System.Drawing.Size(68, 14);
            this.labCompany.TabIndex = 10;
            this.labCompany.Text = "<Company>";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(12, 218);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(52, 14);
            this.labelControl5.TabIndex = 11;
            this.labelControl5.Text = "版权信息:";
            // 
            // labCopyright
            // 
            this.labCopyright.Location = new System.Drawing.Point(70, 218);
            this.labCopyright.Name = "labCopyright";
            this.labCopyright.Size = new System.Drawing.Size(70, 14);
            this.labCopyright.TabIndex = 12;
            this.labCopyright.Text = "<Copyright>";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(465, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSysInfo
            // 
            this.btnSysInfo.Location = new System.Drawing.Point(465, 255);
            this.btnSysInfo.Name = "btnSysInfo";
            this.btnSysInfo.Size = new System.Drawing.Size(75, 23);
            this.btnSysInfo.TabIndex = 14;
            this.btnSysInfo.Text = "系统信息(&S)";
            this.btnSysInfo.Click += new System.EventHandler(this.btnSysInfo_Click);
            // 
            // labWarning
            // 
            this.labWarning.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labWarning.Location = new System.Drawing.Point(12, 238);
            this.labWarning.Name = "labWarning";
            this.labWarning.Size = new System.Drawing.Size(447, 42);
            this.labWarning.TabIndex = 15;
            this.labWarning.Text = "警告：本计算机程序受著作权法和国际公约的保护，未经授权擅自复制或散布本程序的部分或全部，将承受严厉的民事和刑事处罚，对已知的违反者将给予法律范围内的全面制裁。";
            // 
            // lvModules
            // 
            this.lvModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvModules.FullRowSelect = true;
            this.lvModules.Location = new System.Drawing.Point(10, 106);
            this.lvModules.Name = "lvModules";
            this.lvModules.Size = new System.Drawing.Size(449, 86);
            this.lvModules.TabIndex = 16;
            this.lvModules.TabStop = false;
            this.lvModules.UseCompatibleStateImageBehavior = false;
            this.lvModules.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "已加载的插件";
            this.columnHeader1.Width = 308;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "版本";
            this.columnHeader2.Width = 96;
            // 
            // frmAbout
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 290);
            this.Controls.Add(this.lvModules);
            this.Controls.Add(this.labWarning);
            this.Controls.Add(this.btnSysInfo);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labCopyright);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.labCompany);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.btnCopyInfo);
            this.Controls.Add(this.labNetVersion);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.labVersion);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labProduct);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.TopPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "关于";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TopPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl TopPanel;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labProduct;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labVersion;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labNetVersion;
        private DevExpress.XtraEditors.SimpleButton btnCopyInfo;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labCompany;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labCopyright;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnSysInfo;
        private DevExpress.XtraEditors.LabelControl labWarning;
        private System.Windows.Forms.ListView lvModules;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}