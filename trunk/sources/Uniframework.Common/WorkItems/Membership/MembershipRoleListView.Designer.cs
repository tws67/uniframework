namespace Uniframework.Common.WorkItems.Membership
{
    partial class MembershipRoleListView
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
            this.tlRole = new DevExpress.XtraTreeList.TreeList();
            this.colRole = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.bsRole = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlRole)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsRole)).BeginInit();
            this.SuspendLayout();
            // 
            // tlRole
            // 
            this.tlRole.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colRole});
            this.tlRole.DataSource = this.bsRole;
            this.tlRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlRole.Location = new System.Drawing.Point(0, 0);
            this.tlRole.Name = "tlRole";
            this.tlRole.Size = new System.Drawing.Size(670, 383);
            this.tlRole.TabIndex = 0;
            this.tlRole.Tag = "/Shell/Module/Foundation/ContentMenu/MembershipRole";
            // 
            // colRole
            // 
            this.colRole.Caption = "½ÇÉ«Ãû³Æ";
            this.colRole.Name = "colRole";
            this.colRole.OptionsColumn.AllowSize = false;
            this.colRole.Visible = true;
            this.colRole.VisibleIndex = 0;
            // 
            // MembershipRoleListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlRole);
            this.Name = "MembershipRoleListView";
            this.Size = new System.Drawing.Size(670, 383);
            this.Load += new System.EventHandler(this.MembershipRoleListView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tlRole)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsRole)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlRole;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colRole;
        private System.Windows.Forms.BindingSource bsRole;

    }
}
