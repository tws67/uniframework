namespace Uniframework.Common.WorkItems.Membership
{
    partial class MembershipUserEditView
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
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            this.edtUser = new Uniframework.Common.WorkItems.Membership.MembershipUserEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.edtEmail = new DevExpress.XtraEditors.TextEdit();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.edtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // edtUser
            // 
            this.edtUser.Location = new System.Drawing.Point(3, 3);
            this.edtUser.Name = "edtUser";
            this.edtUser.Size = new System.Drawing.Size(386, 39);
            this.edtUser.TabIndex = 0;
            this.edtUser.UserName = "";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(56, 48);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(65, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "电子邮箱(&E)";
            // 
            // edtEmail
            // 
            this.edtEmail.Location = new System.Drawing.Point(56, 68);
            this.edtEmail.Name = "edtEmail";
            this.edtEmail.Size = new System.Drawing.Size(330, 21);
            this.edtEmail.TabIndex = 2;
            conditionValidationRule1.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.Contains;
            conditionValidationRule1.ErrorText = "请输入一个有效的电子邮箱地址";
            this.dxValidationProvider.SetValidationRule(this.edtEmail, conditionValidationRule1);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(230, 102);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(311, 102);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            // 
            // dxValidationProvider
            // 
            this.dxValidationProvider.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            // 
            // MembershipUserEditView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.edtEmail);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.edtUser);
            this.Name = "MembershipUserEditView";
            this.Size = new System.Drawing.Size(396, 135);
            this.Load += new System.EventHandler(this.MembershipUserEditView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MembershipUserEdit edtUser;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit edtEmail;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
    }
}
