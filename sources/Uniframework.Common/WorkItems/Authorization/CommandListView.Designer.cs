namespace Uniframework.Common.WorkItems.Authorization
{
    partial class CommandListView
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
            this.dataGrid = new DevExpress.XtraGrid.GridControl();
            this.lvCommands = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCategory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCommandUri = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bsCommands = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvCommands)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCommands)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.DataSource = this.bsCommands;
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.MainView = this.lvCommands;
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.Size = new System.Drawing.Size(760, 467);
            this.dataGrid.TabIndex = 0;
            this.dataGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.lvCommands});
            // 
            // lvCommands
            // 
            this.lvCommands.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCategory,
            this.colName,
            this.colCommandUri,
            this.colImage});
            this.lvCommands.GridControl = this.dataGrid;
            this.lvCommands.Name = "lvCommands";
            this.lvCommands.OptionsBehavior.Editable = false;
            this.lvCommands.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colCategory, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // colCategory
            // 
            this.colCategory.Caption = "分组";
            this.colCategory.FieldName = "Category";
            this.colCategory.Name = "colCategory";
            this.colCategory.Visible = true;
            this.colCategory.VisibleIndex = 0;
            // 
            // colName
            // 
            this.colName.Caption = "名称";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 1;
            // 
            // colCommandUri
            // 
            this.colCommandUri.Caption = "操作命令";
            this.colCommandUri.FieldName = "CommandUri";
            this.colCommandUri.Name = "colCommandUri";
            this.colCommandUri.Visible = true;
            this.colCommandUri.VisibleIndex = 2;
            // 
            // colImage
            // 
            this.colImage.Caption = "图标文件";
            this.colImage.FieldName = "Image";
            this.colImage.Name = "colImage";
            this.colImage.Visible = true;
            this.colImage.VisibleIndex = 3;
            this.colImage.Width = 117;
            // 
            // bsCommands
            // 
            this.bsCommands.CurrentChanged += new System.EventHandler(this.bsCommands_CurrentChanged);
            // 
            // CommandListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGrid);
            this.Name = "CommandListView";
            this.Size = new System.Drawing.Size(760, 467);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lvCommands)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCommands)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl dataGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView lvCommands;
        private DevExpress.XtraGrid.Columns.GridColumn colCategory;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colCommandUri;
        private DevExpress.XtraGrid.Columns.GridColumn colImage;
        private System.Windows.Forms.BindingSource bsCommands;



    }
}
