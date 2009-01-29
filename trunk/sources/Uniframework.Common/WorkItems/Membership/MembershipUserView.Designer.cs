namespace Uniframework.Common.WorkItems.Membership
{
    partial class MembershipUserView
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
            this.lcView = new DevExpress.XtraLayout.LayoutControl();
            this.txtEmail = new DevExpress.XtraEditors.TextEdit();
            this.txtConfirmPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.txtUserName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.lcView)).BeginInit();
            this.lcView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirmPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // lcView
            // 
            this.lcView.Controls.Add(this.txtEmail);
            this.lcView.Controls.Add(this.txtConfirmPassword);
            this.lcView.Controls.Add(this.txtPassword);
            this.lcView.Controls.Add(this.txtUserName);
            this.lcView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lcView.Location = new System.Drawing.Point(0, 0);
            this.lcView.Name = "lcView";
            this.lcView.Root = this.layoutControlGroup1;
            this.lcView.Size = new System.Drawing.Size(485, 303);
            this.lcView.TabIndex = 0;
            // 
            // txtEmail
            // 
            this.txtEmail.EnterMoveNextControl = true;
            this.dxErrorProvider.SetError(this.txtEmail, "请为用户指定使用的唯一邮箱地址");
            this.dxErrorProvider.SetErrorType(this.txtEmail, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
            this.txtEmail.Location = new System.Drawing.Point(88, 103);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Properties.NullText = "用户唯一的邮箱地址";
            this.txtEmail.Size = new System.Drawing.Size(391, 21);
            this.txtEmail.StyleController = this.lcView;
            this.txtEmail.TabIndex = 7;
            this.txtEmail.EditValueChanged += new System.EventHandler(this.txtEmail_EditValueChanged);
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.EnterMoveNextControl = true;
            this.dxErrorProvider.SetError(this.txtConfirmPassword, "您输入的密码前后不一致");
            this.dxErrorProvider.SetErrorType(this.txtConfirmPassword, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
            this.txtConfirmPassword.Location = new System.Drawing.Point(88, 71);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Properties.NullText = "再次确认密码";
            this.txtConfirmPassword.Size = new System.Drawing.Size(201, 21);
            this.txtConfirmPassword.StyleController = this.lcView;
            this.txtConfirmPassword.TabIndex = 6;
            // 
            // txtPassword
            // 
            this.txtPassword.EnterMoveNextControl = true;
            this.dxErrorProvider.SetError(this.txtPassword, "请为用户指定登录使用的密码");
            this.dxErrorProvider.SetErrorType(this.txtPassword, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
            this.txtPassword.Location = new System.Drawing.Point(88, 39);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.NullText = "用户登录使用的密码";
            this.txtPassword.Size = new System.Drawing.Size(201, 21);
            this.txtPassword.StyleController = this.lcView;
            this.txtPassword.TabIndex = 5;
            // 
            // txtUserName
            // 
            this.txtUserName.EnterMoveNextControl = true;
            this.dxErrorProvider.SetError(this.txtUserName, "必须为新建登录指定一个唯一的名称");
            this.dxErrorProvider.SetErrorType(this.txtUserName, DevExpress.XtraEditors.DXErrorProvider.ErrorType.Default);
            this.txtUserName.Location = new System.Drawing.Point(88, 7);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Properties.NullText = "用户登录名称";
            this.txtUserName.Size = new System.Drawing.Size(391, 21);
            this.txtUserName.StyleController = this.lcView;
            this.txtUserName.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem2,
            this.emptySpaceItem3,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(485, 303);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtUserName;
            this.layoutControlItem1.CustomizationFormText = "登录名称:";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(483, 32);
            this.layoutControlItem1.Text = "登录名称:";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(76, 20);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtPassword;
            this.layoutControlItem2.CustomizationFormText = "密码:";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 32);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(293, 32);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(293, 32);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(293, 32);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.Text = "密码:";
            this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(76, 20);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtConfirmPassword;
            this.layoutControlItem3.CustomizationFormText = "再次确认密码:";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 64);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(293, 32);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(293, 32);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(293, 32);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "再次确认密码:";
            this.layoutControlItem3.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(76, 20);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
            this.emptySpaceItem2.Location = new System.Drawing.Point(293, 32);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(190, 32);
            this.emptySpaceItem2.Text = "emptySpaceItem2";
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
            this.emptySpaceItem3.Location = new System.Drawing.Point(293, 64);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(190, 32);
            this.emptySpaceItem3.Text = "emptySpaceItem3";
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.txtEmail;
            this.layoutControlItem4.CustomizationFormText = "电子邮箱:";
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(483, 205);
            this.layoutControlItem4.Text = "电子邮箱:";
            this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(76, 20);
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // MembershipUserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lcView);
            this.Name = "MembershipUserView";
            this.Size = new System.Drawing.Size(485, 303);
            ((System.ComponentModel.ISupportInitialize)(this.lcView)).EndInit();
            this.lcView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirmPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl lcView;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtUserName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.TextEdit txtConfirmPassword;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraEditors.TextEdit txtEmail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
    }
}
