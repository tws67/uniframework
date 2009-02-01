namespace Uniframework.Common.WorkItems.Membership
{
    partial class MembershipUserChoiseView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MembershipUserChoiseView));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.tlUsers = new DevExpress.XtraTreeList.TreeList();
            this.colUserName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.ilUsers = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tlUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(64, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "成员列表(&L)";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(82, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(472, 2);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(394, 288);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(479, 288);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            // 
            // tlUsers
            // 
            this.tlUsers.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colUserName});
            this.tlUsers.Location = new System.Drawing.Point(12, 32);
            this.tlUsers.Name = "tlUsers";
            this.tlUsers.OptionsBehavior.Editable = false;
            this.tlUsers.OptionsSelection.MultiSelect = true;
            this.tlUsers.OptionsView.ShowHorzLines = false;
            this.tlUsers.OptionsView.ShowIndicator = false;
            this.tlUsers.OptionsView.ShowRoot = false;
            this.tlUsers.OptionsView.ShowVertLines = false;
            this.tlUsers.Size = new System.Drawing.Size(542, 244);
            this.tlUsers.StateImageList = this.ilUsers;
            this.tlUsers.TabIndex = 5;
            // 
            // colUserName
            // 
            this.colUserName.Caption = "成员名称";
            this.colUserName.FieldName = "UserName";
            this.colUserName.Name = "colUserName";
            this.colUserName.Visible = true;
            this.colUserName.VisibleIndex = 0;
            // 
            // ilUsers
            // 
            this.ilUsers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilUsers.ImageStream")));
            this.ilUsers.TransparentColor = System.Drawing.Color.Transparent;
            this.ilUsers.Images.SetKeyName(0, "businessman.ico");
            // 
            // MembershipUserChoiseView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlUsers);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelControl1);
            this.Name = "MembershipUserChoiseView";
            this.Size = new System.Drawing.Size(566, 320);
            this.Enter += new System.EventHandler(this.MembershipUserChoiseView_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.tlUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraTreeList.TreeList tlUsers;
        private System.Windows.Forms.ImageList ilUsers;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colUserName;
    }
}
