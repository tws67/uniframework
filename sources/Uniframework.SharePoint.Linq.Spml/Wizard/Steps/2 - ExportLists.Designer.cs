namespace Uniframework.SharePoint.Linq.Spml
{
    partial class ExportLists
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportLists));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.fields = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label4 = new System.Windows.Forms.Label();
            this.properties = new System.Windows.Forms.PropertyGrid();
            this.panel = new System.Windows.Forms.SplitContainer();
            this.lists = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.txtContext = new System.Windows.Forms.TextBox();
            this.chkContext = new System.Windows.Forms.CheckBox();
            this.bgList = new System.ComponentModel.BackgroundWorker();
            this.chkSom = new System.Windows.Forms.CheckBox();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel.Panel1.SuspendLayout();
            this.panel.Panel2.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.fields);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.properties);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // fields
            // 
            resources.ApplyResources(this.fields, "fields");
            this.fields.CheckBoxes = true;
            this.fields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.fields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.fields.MultiSelect = false;
            this.fields.Name = "fields";
            this.fields.ShowGroups = false;
            this.fields.UseCompatibleStateImageBehavior = false;
            this.fields.View = System.Windows.Forms.View.Details;
            this.fields.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.fields_ItemChecked);
            this.fields.SelectedIndexChanged += new System.EventHandler(this.fields_SelectedIndexChanged);
            this.fields.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.fields_ItemCheck);
            this.fields.Enter += new System.EventHandler(this.fields_Enter);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // properties
            // 
            resources.ApplyResources(this.properties, "properties");
            this.properties.Name = "properties";
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            // 
            // panel.Panel1
            // 
            this.panel.Panel1.Controls.Add(this.lists);
            this.panel.Panel1.Controls.Add(this.label2);
            // 
            // panel.Panel2
            // 
            this.panel.Panel2.Controls.Add(this.splitContainer2);
            // 
            // lists
            // 
            resources.ApplyResources(this.lists, "lists");
            this.lists.CheckBoxes = true;
            this.lists.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lists.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lists.MultiSelect = false;
            this.lists.Name = "lists";
            this.lists.ShowGroups = false;
            this.lists.UseCompatibleStateImageBehavior = false;
            this.lists.View = System.Windows.Forms.View.Details;
            this.lists.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lists_ItemChecked);
            this.lists.SelectedIndexChanged += new System.EventHandler(this.lists_SelectedIndexChanged);
            this.lists.Enter += new System.EventHandler(this.lists_Enter);
            // 
            // txtContext
            // 
            resources.ApplyResources(this.txtContext, "txtContext");
            this.txtContext.Name = "txtContext";
            this.txtContext.TextChanged += new System.EventHandler(this.txtContext_TextChanged);
            // 
            // chkContext
            // 
            resources.ApplyResources(this.chkContext, "chkContext");
            this.chkContext.Checked = true;
            this.chkContext.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkContext.Name = "chkContext";
            this.chkContext.UseVisualStyleBackColor = true;
            this.chkContext.CheckedChanged += new System.EventHandler(this.chkContext_CheckedChanged);
            // 
            // bgList
            // 
            this.bgList.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgList_DoWork);
            this.bgList.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgList_RunWorkerCompleted);
            // 
            // chkSom
            // 
            resources.ApplyResources(this.chkSom, "chkSom");
            this.chkSom.Name = "chkSom";
            this.chkSom.UseVisualStyleBackColor = true;
            this.chkSom.CheckedChanged += new System.EventHandler(this.chkSom_CheckedChanged);
            // 
            // ExportLists
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkSom);
            this.Controls.Add(this.chkContext);
            this.Controls.Add(this.txtContext);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.label1);
            this.Name = "ExportLists";
            this.Load += new System.EventHandler(this.ExportLists_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.panel.Panel1.ResumeLayout(false);
            this.panel.Panel1.PerformLayout();
            this.panel.Panel2.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PropertyGrid properties;
        private System.Windows.Forms.SplitContainer panel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView lists;
        private System.Windows.Forms.ListView fields;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtContext;
        private System.Windows.Forms.CheckBox chkContext;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker bgList;
        private System.Windows.Forms.CheckBox chkSom;

    }
}
