namespace Uniframework.Common.WorkItems.Authorization
{
    partial class AuthorizationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorizationControl));
            this.tlAuth = new DevExpress.XtraTreeList.TreeList();
            this.colNode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilAuth = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlAuth)).BeginInit();
            this.SuspendLayout();
            // 
            // tlAuth
            // 
            this.tlAuth.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colNode});
            this.tlAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlAuth.Location = new System.Drawing.Point(0, 0);
            this.tlAuth.Name = "tlAuth";
            this.tlAuth.OptionsBehavior.Editable = false;
            this.tlAuth.OptionsView.ShowHorzLines = false;
            this.tlAuth.OptionsView.ShowIndicator = false;
            this.tlAuth.OptionsView.ShowVertLines = false;
            this.tlAuth.Size = new System.Drawing.Size(189, 418);
            this.tlAuth.TabIndex = 0;
            // 
            // colNode
            // 
            this.colNode.Caption = "½ÚµãÃû³Æ";
            this.colNode.FieldName = "Name";
            this.colNode.Name = "colNode";
            this.colNode.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Count;
            this.colNode.Visible = true;
            this.colNode.VisibleIndex = 0;
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
            // AuthorizationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlAuth);
            this.Name = "AuthorizationControl";
            this.Size = new System.Drawing.Size(189, 418);
            ((System.ComponentModel.ISupportInitialize)(this.tlAuth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlAuth;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colNode;
        private System.Windows.Forms.ImageList ilAuth;
    }
}
