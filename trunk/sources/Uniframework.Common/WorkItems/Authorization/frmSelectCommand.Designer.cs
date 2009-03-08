namespace Uniframework.Common.WorkItems.Authorization
{
    partial class frmSelectCommand
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectCommand));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.tlCommands = new DevExpress.XtraTreeList.TreeList();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCommandUri = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilCommands = new System.Windows.Forms.ImageList(this.components);
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.tlCommands)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(64, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "操作列表(&L)";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(82, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(458, 2);
            this.label1.TabIndex = 1;
            // 
            // tlCommands
            // 
            this.tlCommands.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName,
            this.colCommandUri});
            this.tlCommands.ColumnsImageList = this.ilCommands;
            this.tlCommands.Location = new System.Drawing.Point(12, 32);
            this.tlCommands.Name = "tlCommands";
            this.tlCommands.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.tlCommands.OptionsBehavior.Editable = false;
            this.tlCommands.OptionsBehavior.EnableFiltering = true;
            this.tlCommands.OptionsSelection.MultiSelect = true;
            this.tlCommands.OptionsView.ShowCheckBoxes = true;
            this.tlCommands.OptionsView.ShowHorzLines = false;
            this.tlCommands.OptionsView.ShowIndicator = false;
            this.tlCommands.OptionsView.ShowVertLines = false;
            this.tlCommands.SelectImageList = this.ilCommands;
            this.tlCommands.Size = new System.Drawing.Size(528, 211);
            this.tlCommands.TabIndex = 2;
            this.tlCommands.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.tlCommands_BeforeCheckNode);
            this.tlCommands.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tlCommands_AfterCheckNode);
            // 
            // colName
            // 
            this.colName.Caption = "名称";
            this.colName.FieldName = "Name";
            this.colName.MinWidth = 35;
            this.colName.Name = "colName";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 194;
            // 
            // colCommandUri
            // 
            this.colCommandUri.Caption = "操作命令";
            this.colCommandUri.FieldName = "CommandUri";
            this.colCommandUri.Name = "colCommandUri";
            this.colCommandUri.Visible = true;
            this.colCommandUri.VisibleIndex = 1;
            this.colCommandUri.Width = 330;
            // 
            // ilCommands
            // 
            this.ilCommands.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilCommands.ImageStream")));
            this.ilCommands.TransparentColor = System.Drawing.Color.Transparent;
            this.ilCommands.Images.SetKeyName(0, "folder_closed.ico");
            this.ilCommands.Images.SetKeyName(1, "folder.ico");
            this.ilCommands.Images.SetKeyName(2, "folder_gear.ico");
            this.ilCommands.Images.SetKeyName(3, "gear.ico");
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(384, 255);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定(&O)";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(465, 255);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            // 
            // frmSelectCommand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 290);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tlCommands);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectCommand";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "选择";
            ((System.ComponentModel.ISupportInitialize)(this.tlCommands)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraTreeList.TreeList tlCommands;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCommandUri;
        private System.Windows.Forms.ImageList ilCommands;
    }
}