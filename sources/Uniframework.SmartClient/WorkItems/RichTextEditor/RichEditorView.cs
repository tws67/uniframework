using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;

namespace Uniframework.SmartClient
{
    public partial class RichEditorView : DevExpress.XtraEditors.XtraUserControl
    {
        public event EventHandler ModifiedChanged;
        public event EventHandler Disposed;

        public RichEditorView()
        {
            InitializeComponent();
        }

        public bool Modified
        {
            get;
            set;
        }

        /// <summary>
        /// Opens the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Open(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate)) {
                RichTextEdit.Text = fs.ToString();
                fs.Close();
            }
        }

        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            using (FileStream fs = new FileStream(filename, FileMode.CreateNew)) {
                byte[] bytes = Encoding.UTF8.GetBytes(RichTextEdit.Text);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
            }
        }
    }
}
