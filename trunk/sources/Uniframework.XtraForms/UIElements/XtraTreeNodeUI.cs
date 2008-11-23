using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;
using System.Windows.Forms;

namespace Uniframework.XtraForms.UIElements
{
    public class XtraTreeListNode : TreeListNode
    {
        private Image image = null;
        private Image selectedImage = null;
        private string name = String.Empty;
        private string text = String.Empty;

        public XtraTreeListNode(int id, TreeListNodes owner)
            : base(id, owner)
        { }

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public Image SelectedImage
        {
            get { return selectedImage; }
            set { 
                selectedImage = value;
                XtraTreeList list = TreeList as XtraTreeList;
                if (list != null) {
                }
            }
        }
    }

    public class XtraTreeList : TreeList
    {
        private ImageList imageList;

        public XtraTreeList()
            : base()
        {
            imageList = new ImageList();
        }

        public ImageList ImageList
        {
            get { return imageList; }
        }
    }
}
