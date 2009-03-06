using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI;
using Uniframework.Security;
using DevExpress.XtraTreeList.Nodes;

namespace Uniframework.Common.WorkItems.Authorization
{
    public partial class frmSelectCommand : DevExpress.XtraEditors.XtraForm
    {
        private Dictionary<string, TreeListNode> categories = new Dictionary<string, TreeListNode>();

        public frmSelectCommand()
        {
            InitializeComponent();
        }

        [ServiceDependency]
        public IAuthorizationCommandService CommandService
        {
            get;
            set;
        }

        /// <summary>
        /// Refreshes the command list.
        /// </summary>
        public void RefreshList()
        {
            using (WaitCursor cursor = new WaitCursor(true)) {
                tlCommands.BeginUpdate();
                categories.Clear();
                tlCommands.ClearNodes();
                
                // 添加所有的操作项
                try {
                    IList<AuthorizationCommand> lcs = CommandService.GetAll(); // 获取所有操作项
                    foreach (AuthorizationCommand ac in lcs) {
                        if (!AuthNode.Commands.Contains(ac))
                        {
                            if (!categories.ContainsKey(ac.Category))
                            {
                                categories.Add(ac.Category, AddCategory(ac.Category)); // 添加分类
                            }
                            TreeListNode parent = categories[ac.Category];
                            TreeListNode child = tlCommands.AppendNode(new object[] { ac.Name, ac.CommandUri }, parent, ac);
                            child.ImageIndex = 3;
                            child.SelectImageIndex = 3;
                        }
                    }
                }
                finally {
                    tlCommands.EndUpdate();
                }
            }
        }

        /// <summary>
        /// 当前选择的操作项
        /// </summary>
        /// <value>The selection.</value>
        public List<AuthorizationCommand> Selection
        {
            get {
                List<AuthorizationCommand> list = new List<AuthorizationCommand>();
                foreach (TreeListNode node in tlCommands.Nodes) {
                    if (node.Checked && node.Tag != null)
                        list.Add(node.Tag as AuthorizationCommand);
                }
                return list;
            }
        }

        public AuthorizationNode AuthNode
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the category node.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        private TreeListNode AddCategory(string category)
        {
            TreeListNode node = tlCommands.AppendNode(new object[] { category, "" }, -1, null);
            node.ImageIndex = 2;
            node.SelectImageIndex = 1;
            return node;
        }
    }
}