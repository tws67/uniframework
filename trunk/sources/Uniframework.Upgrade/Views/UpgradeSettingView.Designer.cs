namespace Uniframework.Upgrade.Views
{
    partial class UpgradeSettingView
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtUpgradeUrl = new DevExpress.XtraEditors.TextEdit();
            this.chkReciveMessage = new DevExpress.XtraEditors.CheckEdit();
            this.chkCheckUpgrade = new DevExpress.XtraEditors.CheckEdit();
            this.txtCheckInterval = new DevExpress.XtraEditors.SpinEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpgradeUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReciveMessage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckUpgrade.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCheckInterval.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(3, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(102, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "升级服务器地址(&A)";
            // 
            // txtUpgradeUrl
            // 
            this.txtUpgradeUrl.Location = new System.Drawing.Point(5, 23);
            this.txtUpgradeUrl.Name = "txtUpgradeUrl";
            this.txtUpgradeUrl.Size = new System.Drawing.Size(432, 21);
            this.txtUpgradeUrl.TabIndex = 1;
            // 
            // chkReciveMessage
            // 
            this.chkReciveMessage.Location = new System.Drawing.Point(3, 50);
            this.chkReciveMessage.Name = "chkReciveMessage";
            this.chkReciveMessage.Properties.Caption = "允许接收服务器端的更新消息(&U)";
            this.chkReciveMessage.Size = new System.Drawing.Size(261, 19);
            this.chkReciveMessage.TabIndex = 2;
            // 
            // chkCheckUpgrade
            // 
            this.chkCheckUpgrade.Location = new System.Drawing.Point(3, 75);
            this.chkCheckUpgrade.Name = "chkCheckUpgrade";
            this.chkCheckUpgrade.Properties.Caption = "自动检查系统更新(&C)";
            this.chkCheckUpgrade.Size = new System.Drawing.Size(142, 19);
            this.chkCheckUpgrade.TabIndex = 3;
            this.chkCheckUpgrade.CheckedChanged += new System.EventHandler(this.chkCheckUpgrade_CheckedChanged);
            // 
            // txtCheckInterval
            // 
            this.txtCheckInterval.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtCheckInterval.Location = new System.Drawing.Point(25, 100);
            this.txtCheckInterval.Name = "txtCheckInterval";
            this.txtCheckInterval.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.txtCheckInterval.Properties.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txtCheckInterval.Properties.IsFloatValue = false;
            this.txtCheckInterval.Properties.Mask.EditMask = "N00";
            this.txtCheckInterval.Size = new System.Drawing.Size(62, 21);
            this.txtCheckInterval.TabIndex = 4;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(96, 103);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(72, 14);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "分钟检查一次";
            // 
            // UpgradeSettingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtCheckInterval);
            this.Controls.Add(this.chkCheckUpgrade);
            this.Controls.Add(this.chkReciveMessage);
            this.Controls.Add(this.txtUpgradeUrl);
            this.Controls.Add(this.labelControl1);
            this.Name = "UpgradeSettingView";
            this.Size = new System.Drawing.Size(440, 270);
            this.Load += new System.EventHandler(this.UpgradeSettingView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtUpgradeUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkReciveMessage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckUpgrade.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCheckInterval.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtUpgradeUrl;
        private DevExpress.XtraEditors.CheckEdit chkReciveMessage;
        private DevExpress.XtraEditors.CheckEdit chkCheckUpgrade;
        private DevExpress.XtraEditors.SpinEdit txtCheckInterval;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}
