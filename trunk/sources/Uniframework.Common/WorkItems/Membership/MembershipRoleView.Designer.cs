namespace Uniframework.Common.WorkItems.Membership
{
    partial class MembershipRoleView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MembershipRoleView));
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.tlMembers = new DevExpress.XtraTreeList.TreeList();
            this.colUserName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilMembers = new System.Windows.Forms.ImageList(this.components);
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tlAuth = new DevExpress.XtraTreeList.TreeList();
            this.colNode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colId = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilAuth = new System.Windows.Forms.ImageList(this.components);
            this.membershipRole = new Uniframework.Common.WorkItems.Membership.MembershipRoleEdit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tlMembers)).BeginInit();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tlAuth)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(399, 287);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "确定(&O)";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(480, 287);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "取消";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Location = new System.Drawing.Point(13, 56);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage3;
            this.xtraTabControl1.Size = new System.Drawing.Size(542, 223);
            this.xtraTabControl1.TabIndex = 11;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage3,
            this.xtraTabPage1});
            // 
            // xtraTabPage3
            // 
            this.xtraTabPage3.Controls.Add(this.btnDelete);
            this.xtraTabPage3.Controls.Add(this.btnAdd);
            this.xtraTabPage3.Controls.Add(this.tlMembers);
            this.xtraTabPage3.Name = "xtraTabPage3";
            this.xtraTabPage3.Size = new System.Drawing.Size(533, 191);
            this.xtraTabPage3.Text = "所有成员";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(85, 163);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "删除(&R)";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(4, 163);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "添加(&D)...";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // tlMembers
            // 
            this.tlMembers.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colUserName});
            this.tlMembers.Location = new System.Drawing.Point(3, 3);
            this.tlMembers.Name = "tlMembers";
            this.tlMembers.OptionsBehavior.Editable = false;
            this.tlMembers.OptionsSelection.MultiSelect = true;
            this.tlMembers.OptionsView.ShowHorzLines = false;
            this.tlMembers.OptionsView.ShowIndicator = false;
            this.tlMembers.OptionsView.ShowRoot = false;
            this.tlMembers.OptionsView.ShowVertLines = false;
            this.tlMembers.Size = new System.Drawing.Size(527, 153);
            this.tlMembers.StateImageList = this.ilMembers;
            this.tlMembers.TabIndex = 0;
            // 
            // colUserName
            // 
            this.colUserName.Caption = "成员名称";
            this.colUserName.FieldName = "UserName";
            this.colUserName.Name = "colUserName";
            this.colUserName.Visible = true;
            this.colUserName.VisibleIndex = 0;
            // 
            // ilMembers
            // 
            this.ilMembers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilMembers.ImageStream")));
            this.ilMembers.TransparentColor = System.Drawing.Color.Transparent;
            this.ilMembers.Images.SetKeyName(0, "businessman.ico");
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.panelControl1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(533, 191);
            this.xtraTabPage1.Text = "权限管理";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.tlAuth);
            this.panelControl1.Location = new System.Drawing.Point(3, 3);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(527, 185);
            this.panelControl1.TabIndex = 0;
            // 
            // tlAuth
            // 
            this.tlAuth.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.tlAuth.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colNode,
            this.colId});
            this.tlAuth.ColumnsImageList = this.ilAuth;
            this.tlAuth.Dock = System.Windows.Forms.DockStyle.Left;
            this.tlAuth.Location = new System.Drawing.Point(2, 2);
            this.tlAuth.Name = "tlAuth";
            this.tlAuth.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.tlAuth.OptionsBehavior.Editable = false;
            this.tlAuth.OptionsView.ShowCheckBoxes = true;
            this.tlAuth.OptionsView.ShowHorzLines = false;
            this.tlAuth.OptionsView.ShowIndicator = false;
            this.tlAuth.OptionsView.ShowVertLines = false;
            this.tlAuth.SelectImageList = this.ilAuth;
            this.tlAuth.Size = new System.Drawing.Size(525, 181);
            this.tlAuth.TabIndex = 2;
            this.tlAuth.Tag = "/Shell/Foundation/Common/Authorization/Store/AuthTree/ContentMenu";
            this.tlAuth.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.tlAuth_BeforeCheckNode);
            this.tlAuth.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tlAuth_AfterCheckNode);
            // 
            // colNode
            // 
            this.colNode.Caption = "节点名称";
            this.colNode.FieldName = "Name";
            this.colNode.MinWidth = 35;
            this.colNode.Name = "colNode";
            this.colNode.SummaryFooter = DevExpress.XtraTreeList.SummaryItemType.Count;
            this.colNode.Visible = true;
            this.colNode.VisibleIndex = 0;
            // 
            // colId
            // 
            this.colId.Caption = "节点标识";
            this.colId.FieldName = "Id";
            this.colId.Name = "colId";
            // 
            // ilAuth
            // 
            this.ilAuth.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilAuth.ImageStream")));
            this.ilAuth.TransparentColor = System.Drawing.Color.Transparent;
            this.ilAuth.Images.SetKeyName(0, "folder_closed.ico");
            this.ilAuth.Images.SetKeyName(1, "folder.ico");
            this.ilAuth.Images.SetKeyName(2, "folder_gear.ico");
            this.ilAuth.Images.SetKeyName(3, "gear.ico");
            this.ilAuth.Images.SetKeyName(4, "gears.ico");
            // 
            // membershipRole
            // 
            this.membershipRole.Editable = false;
            this.membershipRole.Location = new System.Drawing.Point(10, 3);
            this.membershipRole.Name = "membershipRole";
            this.membershipRole.Role = "在此处输入唯一的角色名称";
            this.membershipRole.Size = new System.Drawing.Size(546, 53);
            this.membershipRole.TabIndex = 12;
            // 
            // MembershipRoleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.membershipRole);
            this.Controls.Add(this.xtraTabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "MembershipRoleView";
            this.Size = new System.Drawing.Size(566, 320);
            this.Load += new System.EventHandler(this.MembershipRoleView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tlMembers)).EndInit();
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tlAuth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage3;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraTreeList.TreeList tlMembers;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colUserName;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private MembershipRoleEdit membershipRole;
        private System.Windows.Forms.ImageList ilMembers;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraTreeList.TreeList tlAuth;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colNode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colId;
        private System.Windows.Forms.ImageList ilAuth;
    }
}
