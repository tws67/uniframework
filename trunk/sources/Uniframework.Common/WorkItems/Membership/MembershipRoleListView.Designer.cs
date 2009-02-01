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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MembershipRoleListView));
            this.tlRole = new DevExpress.XtraTreeList.TreeList();
            this.colRoleName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilRoles = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlRole)).BeginInit();
            this.SuspendLayout();
            // 
            // tlRole
            // 
            this.tlRole.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colRoleName});
            this.tlRole.ColumnsImageList = this.ilRoles;
            this.tlRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlRole.Location = new System.Drawing.Point(0, 0);
            this.tlRole.Name = "tlRole";
            this.tlRole.OptionsBehavior.AutoMoveRowFocus = true;
            this.tlRole.OptionsBehavior.Editable = false;
            this.tlRole.OptionsBehavior.KeepSelectedOnClick = false;
            this.tlRole.OptionsBehavior.SmartMouseHover = false;
            this.tlRole.OptionsView.ShowHorzLines = false;
            this.tlRole.OptionsView.ShowIndicator = false;
            this.tlRole.OptionsView.ShowRoot = false;
            this.tlRole.OptionsView.ShowVertLines = false;
            this.tlRole.Size = new System.Drawing.Size(690, 435);
            this.tlRole.StateImageList = this.ilRoles;
            this.tlRole.TabIndex = 1;
            this.tlRole.Tag = "/Shell/Module/Foundation/Common/Membership/Role/ContentMenu";
            this.tlRole.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tlRole_AfterFocusNode);
            // 
            // colRoleName
            // 
            this.colRoleName.Caption = "½ÇÉ«Ãû³Æ";
            this.colRoleName.FieldName = "RoleName";
            this.colRoleName.Name = "colRoleName";
            this.colRoleName.OptionsColumn.AllowSize = false;
            this.colRoleName.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.colRoleName.Visible = true;
            this.colRoleName.VisibleIndex = 0;
            this.colRoleName.Width = 256;
            // 
            // ilRoles
            // 
            this.ilRoles.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilRoles.ImageStream")));
            this.ilRoles.TransparentColor = System.Drawing.Color.Transparent;
            this.ilRoles.Images.SetKeyName(0, "businessmen.ico");
            // 
            // MembershipRoleListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlRole);
            this.Name = "MembershipRoleListView";
            this.Size = new System.Drawing.Size(690, 435);
            this.Load += new System.EventHandler(this.MembershipRoleListView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tlRole)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.TreeList tlRole;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colRoleName;
        private System.Windows.Forms.ImageList ilRoles;
    }
}
