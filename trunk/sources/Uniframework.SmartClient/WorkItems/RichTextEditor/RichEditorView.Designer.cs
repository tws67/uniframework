namespace Uniframework.SmartClient
{
    partial class RichEditorView
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
            this.RichTextEdit = new DevExpress.XtraRichTextEdit.XtraRichTextEdit();
            this.SuspendLayout();
            // 
            // RichTextEdit
            // 
            this.RichTextEdit.ActiveViewType = DevExpress.XtraRichTextEdit.RichEditViewType.Draft;
            this.RichTextEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RichTextEdit.Location = new System.Drawing.Point(0, 0);
            this.RichTextEdit.Name = "RichTextEdit";
            this.RichTextEdit.Size = new System.Drawing.Size(748, 418);
            this.RichTextEdit.TabIndex = 0;
            // 
            // RichEditorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RichTextEdit);
            this.Name = "RichEditorView";
            this.Size = new System.Drawing.Size(748, 418);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraRichTextEdit.XtraRichTextEdit RichTextEdit;
    }
}
