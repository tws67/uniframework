namespace Uniframework.Upgrade
{
    partial class UpgradeProcess
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
            this.picWizard = new System.Windows.Forms.PictureBox();
            this.labCurrent = new DevExpress.XtraEditors.LabelControl();
            this.pbCurrent = new DevExpress.XtraEditors.ProgressBarControl();
            this.labTotal = new DevExpress.XtraEditors.LabelControl();
            this.pbTotal = new DevExpress.XtraEditors.ProgressBarControl();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.picWizard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCurrent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTotal.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // picWizard
            // 
            this.picWizard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picWizard.Image = global::Uniframework.Upgrade.Properties.Resources.LiveUpgrade;
            this.picWizard.Location = new System.Drawing.Point(12, 12);
            this.picWizard.Name = "picWizard";
            this.picWizard.Size = new System.Drawing.Size(71, 129);
            this.picWizard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWizard.TabIndex = 7;
            this.picWizard.TabStop = false;
            // 
            // labCurrent
            // 
            this.labCurrent.Location = new System.Drawing.Point(96, 30);
            this.labCurrent.Name = "labCurrent";
            this.labCurrent.Size = new System.Drawing.Size(52, 14);
            this.labCurrent.TabIndex = 8;
            this.labCurrent.Text = "正在下载:";
            // 
            // pbCurrent
            // 
            this.pbCurrent.Location = new System.Drawing.Point(96, 50);
            this.pbCurrent.Name = "pbCurrent";
            this.pbCurrent.Properties.Step = 1;
            this.pbCurrent.Size = new System.Drawing.Size(361, 16);
            this.pbCurrent.TabIndex = 9;
            // 
            // labTotal
            // 
            this.labTotal.Location = new System.Drawing.Point(96, 72);
            this.labTotal.Name = "labTotal";
            this.labTotal.Size = new System.Drawing.Size(36, 14);
            this.labTotal.TabIndex = 10;
            this.labTotal.Text = "总进度";
            // 
            // pbTotal
            // 
            this.pbTotal.Location = new System.Drawing.Point(96, 92);
            this.pbTotal.Name = "pbTotal";
            this.pbTotal.Properties.Step = 1;
            this.pbTotal.Size = new System.Drawing.Size(361, 16);
            this.pbTotal.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(86, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(371, 2);
            this.label1.TabIndex = 13;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(382, 155);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "取消";
            // 
            // UpgradeProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 190);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbTotal);
            this.Controls.Add(this.labTotal);
            this.Controls.Add(this.pbCurrent);
            this.Controls.Add(this.labCurrent);
            this.Controls.Add(this.picWizard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpgradeProcess";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统更新";
            ((System.ComponentModel.ISupportInitialize)(this.picWizard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCurrent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTotal.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picWizard;
        private DevExpress.XtraEditors.LabelControl labCurrent;
        private DevExpress.XtraEditors.ProgressBarControl pbCurrent;
        private DevExpress.XtraEditors.LabelControl labTotal;
        private DevExpress.XtraEditors.ProgressBarControl pbTotal;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}