namespace Uniframework.Common.WorkItems.Membership
{
    partial class MembershipUserListView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MembershipUserListView));
            this.tlBlending = new DevExpress.XtraTreeList.Blending.XtraTreeListBlending();
            this.tlUser = new DevExpress.XtraTreeList.TreeList();
            this.colUserName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colEmail = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colLastLoginDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreateDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilUsers = new System.Windows.Forms.ImageList(this.components);
            this.bsUser = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsUser)).BeginInit();
            this.SuspendLayout();
            // 
            // tlBlending
            // 
            this.tlBlending.TreeListControl = this.tlUser;
            // 
            // tlUser
            // 
            this.tlUser.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colUserName,
            this.colEmail,
            this.colLastLoginDate,
            this.colCreateDate});
            this.tlUser.ColumnsImageList = this.ilUsers;
            this.tlUser.DataSource = this.bsUser;
            this.tlUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlUser.KeyFieldName = "UserName";
            this.tlUser.Location = new System.Drawing.Point(0, 0);
            this.tlUser.Name = "tlUser";
            this.tlUser.OptionsBehavior.Editable = false;
            this.tlUser.OptionsBehavior.EnableFiltering = true;
            this.tlUser.OptionsView.ShowHorzLines = false;
            this.tlUser.OptionsView.ShowIndicator = false;
            this.tlUser.OptionsView.ShowRoot = false;
            this.tlUser.OptionsView.ShowVertLines = false;
            this.tlUser.ParentFieldName = "";
            this.tlUser.SelectImageList = this.ilUsers;
            this.tlUser.Size = new System.Drawing.Size(779, 463);
            this.tlUser.TabIndex = 0;
            this.tlUser.Tag = "/Shell/Foundation/Common/Membership/User/ContentMenu";
            this.tlUser.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlUser_FocusedNodeChanged);
            // 
            // colUserName
            // 
            this.colUserName.Caption = "用户名称";
            this.colUserName.FieldName = "UserName";
            this.colUserName.MinWidth = 128;
            this.colUserName.Name = "colUserName";
            this.colUserName.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.colUserName.Visible = true;
            this.colUserName.VisibleIndex = 0;
            this.colUserName.Width = 133;
            // 
            // colEmail
            // 
            this.colEmail.Caption = "邮件地址";
            this.colEmail.FieldName = "Email";
            this.colEmail.MinWidth = 256;
            this.colEmail.Name = "colEmail";
            this.colEmail.Visible = true;
            this.colEmail.VisibleIndex = 1;
            this.colEmail.Width = 291;
            // 
            // colLastLoginDate
            // 
            this.colLastLoginDate.Caption = "上次登录时间";
            this.colLastLoginDate.FieldName = "LastLoginDate";
            this.colLastLoginDate.MinWidth = 72;
            this.colLastLoginDate.Name = "colLastLoginDate";
            this.colLastLoginDate.Visible = true;
            this.colLastLoginDate.VisibleIndex = 2;
            this.colLastLoginDate.Width = 159;
            // 
            // colCreateDate
            // 
            this.colCreateDate.Caption = "创建时间";
            this.colCreateDate.FieldName = "CreationDate";
            this.colCreateDate.MinWidth = 72;
            this.colCreateDate.Name = "colCreateDate";
            this.colCreateDate.Visible = true;
            this.colCreateDate.VisibleIndex = 3;
            this.colCreateDate.Width = 192;
            // 
            // ilUsers
            // 
            this.ilUsers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilUsers.ImageStream")));
            this.ilUsers.TransparentColor = System.Drawing.Color.Transparent;
            this.ilUsers.Images.SetKeyName(0, "businessman.ico");
            // 
            // MembershipUserListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlUser);
            this.Name = "MembershipUserListView";
            this.Size = new System.Drawing.Size(779, 463);
            this.Load += new System.EventHandler(this.MembershipUserListView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tlUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsUser)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTreeList.Blending.XtraTreeListBlending tlBlending;
        private DevExpress.XtraTreeList.TreeList tlUser;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colEmail;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colLastLoginDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreateDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colUserName;
        private System.Windows.Forms.BindingSource bsUser;
        private System.Windows.Forms.ImageList ilUsers;
    }
}
