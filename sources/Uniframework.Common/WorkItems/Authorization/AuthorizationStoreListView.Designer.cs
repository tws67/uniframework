namespace Uniframework.Common.WorkItems.Authorization
{
    partial class AuthorizationStoreListView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorizationStoreListView));
            this.tlAuth = new DevExpress.XtraTreeList.TreeList();
            this.colNode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.tlCommands = new DevExpress.XtraTreeList.TreeList();
            this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colAuthorizationUri = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colImageFile = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilAuth = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlAuth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlCommands)).BeginInit();
            this.SuspendLayout();
            // 
            // tlAuth
            // 
            this.tlAuth.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colNode});
            this.tlAuth.Dock = System.Windows.Forms.DockStyle.Left;
            this.tlAuth.Location = new System.Drawing.Point(0, 0);
            this.tlAuth.Name = "tlAuth";
            this.tlAuth.OptionsBehavior.Editable = false;
            this.tlAuth.OptionsView.ShowHorzLines = false;
            this.tlAuth.OptionsView.ShowIndicator = false;
            this.tlAuth.OptionsView.ShowVertLines = false;
            this.tlAuth.Size = new System.Drawing.Size(189, 468);
            this.tlAuth.TabIndex = 1;
            // 
            // colNode
            // 
            this.colNode.Caption = "节点名称";
            this.colNode.FieldName = "Name";
            this.colNode.Name = "colNode";
            this.colNode.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Count;
            this.colNode.Visible = true;
            this.colNode.VisibleIndex = 0;
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(189, 0);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(6, 468);
            this.splitterControl1.TabIndex = 2;
            this.splitterControl1.TabStop = false;
            // 
            // tlCommands
            // 
            this.tlCommands.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colName,
            this.colAuthorizationUri,
            this.colImageFile});
            this.tlCommands.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlCommands.Location = new System.Drawing.Point(195, 0);
            this.tlCommands.Name = "tlCommands";
            this.tlCommands.OptionsView.ShowCheckBoxes = true;
            this.tlCommands.OptionsView.ShowHorzLines = false;
            this.tlCommands.OptionsView.ShowIndicator = false;
            this.tlCommands.OptionsView.ShowRoot = false;
            this.tlCommands.OptionsView.ShowVertLines = false;
            this.tlCommands.Size = new System.Drawing.Size(586, 468);
            this.tlCommands.TabIndex = 3;
            // 
            // colName
            // 
            this.colName.Caption = "名称";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.FixedWidth = true;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 108;
            // 
            // colAuthorizationUri
            // 
            this.colAuthorizationUri.Caption = "操作的命令";
            this.colAuthorizationUri.FieldName = "AuthorizationUri";
            this.colAuthorizationUri.Name = "colAuthorizationUri";
            this.colAuthorizationUri.Visible = true;
            this.colAuthorizationUri.VisibleIndex = 1;
            this.colAuthorizationUri.Width = 375;
            // 
            // colImageFile
            // 
            this.colImageFile.Caption = "图标文件";
            this.colImageFile.FieldName = "ImageFile";
            this.colImageFile.Name = "colImageFile";
            this.colImageFile.Visible = true;
            this.colImageFile.VisibleIndex = 2;
            this.colImageFile.Width = 108;
            // 
            // ilAuth
            // 
            this.ilAuth.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilAuth.ImageStream")));
            this.ilAuth.TransparentColor = System.Drawing.Color.Transparent;
            this.ilAuth.Images.SetKeyName(0, "folder_closed.ico");
            this.ilAuth.Images.SetKeyName(1, "folder.ico");
            this.ilAuth.Images.SetKeyName(2, "folder_gear.ico");
            this.ilAuth.Images.SetKeyName(3, "gear.ico");
            // 
            // AuthorizationStoreListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlCommands);
            this.Controls.Add(this.splitterControl1);
            this.Controls.Add(this.tlAuth);
            this.Name = "AuthorizationStoreListView";
            this.Size = new System.Drawing.Size(781, 468);
            ((System.ComponentModel.ISupportInitialize)(this.tlAuth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlCommands)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlAuth;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colNode;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraTreeList.TreeList tlCommands;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colAuthorizationUri;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colImageFile;
        private System.Windows.Forms.ImageList ilAuth;
    }
}
