namespace Uniframework.SmartClient
{
    partial class TaskbarView
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
            this.taskbarWorkspace = new Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace();
            ((System.ComponentModel.ISupportInitialize)(this.taskbarWorkspace)).BeginInit();
            this.SuspendLayout();
            // 
            // taskbarWorkspace
            // 
            this.taskbarWorkspace.ActiveGroup = null;
            this.taskbarWorkspace.ContentButtonHint = null;
            this.taskbarWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskbarWorkspace.Location = new System.Drawing.Point(0, 0);
            this.taskbarWorkspace.Name = "taskbarWorkspace";
            this.taskbarWorkspace.OptionsNavPane.ExpandedWidth = 140;
            this.taskbarWorkspace.Size = new System.Drawing.Size(169, 420);
            this.taskbarWorkspace.TabIndex = 0;
            this.taskbarWorkspace.View = new DevExpress.XtraNavBar.ViewInfo.SkinExplorerBarViewInfoRegistrator();
            // 
            // TaskbarView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.taskbarWorkspace);
            this.Name = "TaskbarView";
            this.Size = new System.Drawing.Size(169, 420);
            this.Load += new System.EventHandler(this.TaskbarView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.taskbarWorkspace)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Uniframework.XtraForms.Workspaces.XtraNavBarWorkspace taskbarWorkspace;

    }
}
