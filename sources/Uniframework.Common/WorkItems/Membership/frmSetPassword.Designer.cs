namespace Uniframework.Common.WorkItems.Membership
{
    partial class frmSetPassword
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
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule compareAgainstControlValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.CompareAgainstControlValidationRule();
            this.edtPassword = new DevExpress.XtraEditors.TextEdit();
            this.edtUser = new Uniframework.Common.WorkItems.Membership.MembershipUserEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.edtConfigrationPassword = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtConfigrationPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // edtPassword
            // 
            this.edtPassword.EnterMoveNextControl = true;
            this.edtPassword.Location = new System.Drawing.Point(150, 76);
            this.edtPassword.Name = "edtPassword";
            this.edtPassword.Properties.PasswordChar = '●';
            this.edtPassword.Size = new System.Drawing.Size(264, 21);
            this.edtPassword.TabIndex = 0;
            conditionValidationRule1.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule1.ErrorText = "密码不能为空";
            this.dxValidationProvider.SetValidationRule(this.edtPassword, conditionValidationRule1);
            // 
            // edtUser
            // 
            this.edtUser.Location = new System.Drawing.Point(12, 12);
            this.edtUser.Name = "edtUser";
            this.edtUser.Size = new System.Drawing.Size(405, 39);
            this.edtUser.TabIndex = 0;
            this.edtUser.TabStop = false;
            this.edtUser.UserName = "";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(11, 52);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(72, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "帐户的新密码";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(90, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 2);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Uniframework.Common.Properties.Resources.keys;
            this.pictureBox1.Location = new System.Drawing.Point(15, 84);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(37, 35);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(67, 79);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(53, 14);
            this.labelControl3.TabIndex = 7;
            this.labelControl3.Text = "新密码(&P)";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(67, 103);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(77, 14);
            this.labelControl4.TabIndex = 9;
            this.labelControl4.Text = "确认新密码(&C)";
            // 
            // edtConfigrationPassword
            // 
            this.edtConfigrationPassword.EnterMoveNextControl = true;
            this.edtConfigrationPassword.Location = new System.Drawing.Point(150, 103);
            this.edtConfigrationPassword.Name = "edtConfigrationPassword";
            this.edtConfigrationPassword.Properties.PasswordChar = '●';
            this.edtConfigrationPassword.Size = new System.Drawing.Size(264, 21);
            this.edtConfigrationPassword.TabIndex = 1;
            compareAgainstControlValidationRule1.CompareControlOperator = DevExpress.XtraEditors.DXErrorProvider.CompareControlOperator.Equals;
            compareAgainstControlValidationRule1.Control = this.edtPassword;
            compareAgainstControlValidationRule1.ErrorText = "前后输入的密码不一致";
            this.dxValidationProvider.SetValidationRule(this.edtConfigrationPassword, compareAgainstControlValidationRule1);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(12, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(401, 2);
            this.label2.TabIndex = 10;
            this.label2.Text = "label2";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(12, 156);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(0, 14);
            this.labelControl5.TabIndex = 11;
            // 
            // labelControl2
            // 
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.labelControl2.Location = new System.Drawing.Point(67, 136);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(346, 28);
            this.labelControl2.TabIndex = 12;
            this.labelControl2.Text = "设置帐户密码后将导致原始密码不可用，按“确定”键后您所做的修改将马上生效！";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(257, 187);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(338, 187);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            // 
            // dxValidationProvider
            // 
            this.dxValidationProvider.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            // 
            // frmSetPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 220);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.edtConfigrationPassword);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.edtPassword);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.edtUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetPassword";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置密码";
            ((System.ComponentModel.ISupportInitialize)(this.edtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtConfigrationPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MembershipUserEdit edtUser;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit edtPassword;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit edtConfigrationPassword;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
    }
}