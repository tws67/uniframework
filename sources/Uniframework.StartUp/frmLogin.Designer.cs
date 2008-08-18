namespace Uniframework.StartUp
{
    partial class frmLogin
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.bkWorker = new System.ComponentModel.BackgroundWorker();
            this.pnlClient = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblServer = new System.Windows.Forms.Label();
            this.linkClose = new System.Windows.Forms.LinkLabel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.IsBalloon = true;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // bkWorker
            // 
            this.bkWorker.WorkerReportsProgress = true;
            // 
            // pnlClient
            // 
            this.pnlClient.BackgroundImage = global::Uniframework.StartUp.Properties.Resources.Middle;
            this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlClient.Location = new System.Drawing.Point(0, 60);
            this.pnlClient.Name = "pnlClient";
            this.pnlClient.Size = new System.Drawing.Size(552, 170);
            this.pnlClient.TabIndex = 3;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackgroundImage = global::Uniframework.StartUp.Properties.Resources.Bottom;
            this.pnlBottom.Controls.Add(this.lblServer);
            this.pnlBottom.Controls.Add(this.linkClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 230);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(552, 60);
            this.pnlBottom.TabIndex = 2;
            // 
            // lblServer
            // 
            this.lblServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblServer.BackColor = System.Drawing.Color.Transparent;
            this.lblServer.Font = new System.Drawing.Font("SimSun", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblServer.ForeColor = System.Drawing.Color.White;
            this.lblServer.Location = new System.Drawing.Point(0, 43);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(547, 12);
            this.lblServer.TabIndex = 5;
            this.lblServer.Text = "Server";
            this.lblServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblServer.Visible = false;
            // 
            // linkClose
            // 
            this.linkClose.BackColor = System.Drawing.Color.Transparent;
            this.linkClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkClose.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkClose.ForeColor = System.Drawing.Color.White;
            this.linkClose.Image = global::Uniframework.StartUp.Properties.Resources.exit;
            this.linkClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkClose.LinkArea = new System.Windows.Forms.LinkArea(0, 8);
            this.linkClose.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkClose.LinkColor = System.Drawing.Color.White;
            this.linkClose.Location = new System.Drawing.Point(12, 17);
            this.linkClose.Name = "linkClose";
            this.linkClose.Size = new System.Drawing.Size(88, 26);
            this.linkClose.TabIndex = 3;
            this.linkClose.TabStop = true;
            this.linkClose.Text = "È¡ÏûµÇÂ¼";
            this.linkClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkClose.UseCompatibleTextRendering = true;
            // 
            // pnlTop
            // 
            this.pnlTop.BackgroundImage = global::Uniframework.StartUp.Properties.Resources.Top;
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(552, 60);
            this.pnlTop.TabIndex = 1;
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 290);
            this.Controls.Add(this.pnlClient);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmLogin";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmLogin";
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.ComponentModel.BackgroundWorker bkWorker;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.LinkLabel linkClose;
        private System.Windows.Forms.Panel pnlClient;
    }
}