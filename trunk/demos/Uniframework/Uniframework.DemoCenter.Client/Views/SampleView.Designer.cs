namespace Uniframework.DemoCenter.Client.Views
{
    partial class SampleView
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
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btnHello = new DevExpress.XtraEditors.SimpleButton();
            this.btnHelloOffline = new DevExpress.XtraEditors.SimpleButton();
            this.btnHelloCache = new DevExpress.XtraEditors.SimpleButton();
            this.labSamleEvent = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupControl1
            // 
            this.groupControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl1.Controls.Add(this.txtName);
            this.groupControl1.Controls.Add(this.labelControl2);
            this.groupControl1.Controls.Add(this.labSamleEvent);
            this.groupControl1.Controls.Add(this.btnHelloCache);
            this.groupControl1.Controls.Add(this.btnHelloOffline);
            this.groupControl1.Controls.Add(this.btnHello);
            this.groupControl1.Location = new System.Drawing.Point(3, 3);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(663, 112);
            this.groupControl1.TabIndex = 0;
            this.groupControl1.Text = "远程方法与事件演示";
            // 
            // btnHello
            // 
            this.btnHello.Location = new System.Drawing.Point(5, 24);
            this.btnHello.Name = "btnHello";
            this.btnHello.Size = new System.Drawing.Size(134, 23);
            this.btnHello.TabIndex = 0;
            this.btnHello.Text = "Say &hello          ";
            this.btnHello.Click += new System.EventHandler(this.btnHello_Click);
            // 
            // btnHelloOffline
            // 
            this.btnHelloOffline.Location = new System.Drawing.Point(5, 53);
            this.btnHelloOffline.Name = "btnHelloOffline";
            this.btnHelloOffline.Size = new System.Drawing.Size(134, 23);
            this.btnHelloOffline.TabIndex = 1;
            this.btnHelloOffline.Text = "Say hello &offline";
            this.btnHelloOffline.Click += new System.EventHandler(this.btnHelloOffline_Click);
            // 
            // btnHelloCache
            // 
            this.btnHelloCache.Location = new System.Drawing.Point(5, 82);
            this.btnHelloCache.Name = "btnHelloCache";
            this.btnHelloCache.Size = new System.Drawing.Size(134, 23);
            this.btnHelloCache.TabIndex = 2;
            this.btnHelloCache.Text = "Say hello &cache";
            this.btnHelloCache.Click += new System.EventHandler(this.btnHelloCache_Click);
            // 
            // labSamleEvent
            // 
            this.labSamleEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labSamleEvent.Location = new System.Drawing.Point(468, 29);
            this.labSamleEvent.Name = "labSamleEvent";
            this.labSamleEvent.Size = new System.Drawing.Size(59, 14);
            this.labSamleEvent.TabIndex = 3;
            this.labSamleEvent.Text = "Active me!";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(195, 29);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(64, 14);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "&Your name:";
            // 
            // txtName
            // 
            this.txtName.EditValue = "Uniframework !!! ";
            this.txtName.Location = new System.Drawing.Point(265, 26);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(160, 21);
            this.txtName.TabIndex = 5;
            // 
            // groupControl2
            // 
            this.groupControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl2.Location = new System.Drawing.Point(3, 121);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(663, 112);
            this.groupControl2.TabIndex = 1;
            this.groupControl2.Text = "服务端异常捕获演示";
            // 
            // SampleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Name = "SampleView";
            this.Size = new System.Drawing.Size(669, 485);
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton btnHello;
        private DevExpress.XtraEditors.SimpleButton btnHelloOffline;
        private DevExpress.XtraEditors.SimpleButton btnHelloCache;
        private DevExpress.XtraEditors.LabelControl labSamleEvent;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.GroupControl groupControl2;
    }
}
