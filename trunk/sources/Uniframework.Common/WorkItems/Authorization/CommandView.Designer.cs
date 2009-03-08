namespace Uniframework.Common.WorkItems.Authorization
{
    partial class CommandView
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
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule1 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule conditionValidationRule2 = new DevExpress.XtraEditors.DXErrorProvider.ConditionValidationRule();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.edtName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.edtCategory = new DevExpress.XtraEditors.MRUEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.edtCommandUri = new DevExpress.XtraEditors.TextEdit();
            this.image = new DevExpress.XtraEditors.PanelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.edtImage = new DevExpress.XtraEditors.ButtonEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labPreview = new DevExpress.XtraEditors.LabelControl();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCategory.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCommandUri.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtImage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Uniframework.Common.Properties.Resources.gear;
            this.pictureBox1.Location = new System.Drawing.Point(10, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 32);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(54, 17);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 14);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "命令名称(&N)";
            // 
            // edtName
            // 
            this.edtName.Location = new System.Drawing.Point(126, 14);
            this.edtName.Name = "edtName";
            this.edtName.Size = new System.Drawing.Size(422, 21);
            this.edtName.TabIndex = 2;
            conditionValidationRule1.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule1.ErrorText = "命令名称不能为空，例如“打印（&P）...”";
            this.dxValidationProvider.SetValidationRule(this.edtName, conditionValidationRule1);
            this.edtName.Leave += new System.EventHandler(this.edtName_Leave);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(79, 44);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(41, 14);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "分组(&C)";
            // 
            // edtCategory
            // 
            this.edtCategory.Location = new System.Drawing.Point(126, 41);
            this.edtCategory.Name = "edtCategory";
            this.edtCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.edtCategory.Size = new System.Drawing.Size(189, 21);
            this.edtCategory.TabIndex = 4;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(55, 71);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(65, 14);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "命令操作(&P)";
            // 
            // edtCommandUri
            // 
            this.edtCommandUri.Location = new System.Drawing.Point(126, 68);
            this.edtCommandUri.Name = "edtCommandUri";
            this.edtCommandUri.Size = new System.Drawing.Size(422, 21);
            this.edtCommandUri.TabIndex = 6;
            conditionValidationRule2.ConditionOperator = DevExpress.XtraEditors.DXErrorProvider.ConditionOperator.IsNotBlank;
            conditionValidationRule2.ErrorText = "命令操作不能为空，例如\"/Shell/Foundation/File/Print\"";
            this.dxValidationProvider.SetValidationRule(this.edtCommandUri, conditionValidationRule2);
            // 
            // image
            // 
            this.image.Location = new System.Drawing.Point(126, 143);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(33, 32);
            this.image.TabIndex = 7;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(79, 98);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(38, 14);
            this.labelControl4.TabIndex = 8;
            this.labelControl4.Text = "图标(&I)";
            // 
            // edtImage
            // 
            this.edtImage.Location = new System.Drawing.Point(126, 95);
            this.edtImage.Name = "edtImage";
            this.edtImage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.edtImage.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.edtImage_ButtonClick);
            this.edtImage.Size = new System.Drawing.Size(422, 21);
            this.edtImage.TabIndex = 9;
            this.edtImage.Leave += new System.EventHandler(this.edtImage_Leave);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(126, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(422, 2);
            this.label1.TabIndex = 10;
            this.label1.Text = "label1";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(48, 122);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(72, 14);
            this.labelControl5.TabIndex = 11;
            this.labelControl5.Text = "命令图标预览";
            // 
            // labPreview
            // 
            this.labPreview.Location = new System.Drawing.Point(165, 152);
            this.labPreview.Name = "labPreview";
            this.labPreview.Size = new System.Drawing.Size(72, 14);
            this.labPreview.TabIndex = 12;
            this.labPreview.Text = "命令图标预览";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(392, 290);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(473, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "取消";
            // 
            // dxValidationProvider
            // 
            this.dxValidationProvider.ValidationMode = DevExpress.XtraEditors.DXErrorProvider.ValidationMode.Auto;
            // 
            // CommandView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labPreview);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.edtImage);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.image);
            this.Controls.Add(this.edtCommandUri);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.edtCategory);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.edtName);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "CommandView";
            this.Size = new System.Drawing.Size(560, 320);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCategory.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtCommandUri.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtImage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit edtName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.MRUEdit edtCategory;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit edtCommandUri;
        private DevExpress.XtraEditors.PanelControl image;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.ButtonEdit edtImage;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labPreview;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
    }
}
