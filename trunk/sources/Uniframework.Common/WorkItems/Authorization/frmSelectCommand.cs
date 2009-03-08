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
                            if (!categories.ContainsKey(ac.Category)) {
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
        public List<AuthorizationCommand> GetSelection()
        {
            List<AuthorizationCommand> list = new List<AuthorizationCommand>();
            foreach (TreeListNode node in tlCommands.Nodes) {
                if (node.CheckState == CheckState.Checked && node.Tag != null)
                    list.Add(node.Tag as AuthorizationCommand);
                if (node.Nodes.Count > 0)
                    GetSelection(list, node);
            }
            return list;
        }

        private void GetSelection(List<AuthorizationCommand> list, TreeListNode node)
        {
            foreach (TreeListNode child in node.Nodes) {
                if (child.CheckState == CheckState.Checked && child.Tag != null)
                    list.Add(child.Tag as AuthorizationCommand);
                if (child.Nodes.Count > 0)
                    GetSelection(list, child);
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
            if (String.IsNullOrEmpty(category) || category.Length == 0)
                category = Constants.DefaultCommandCategory;
            TreeListNode node = tlCommands.AppendNode(new object[] { category, "" }, -1, null);
            node.ImageIndex = 2;
            node.SelectImageIndex = 1;
            return node;
        }

        /// <summary>
        /// Handles the AfterCheckNode event of the tlCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraTreeList.NodeEventArgs"/> instance containing the event data.</param>
        private void tlCommands_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        /// <summary>
        /// Sets the checked parent nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="check">The check.</param>
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        /// <summary>
        /// Sets the checked child nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="check">The check.</param>
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        /// <summary>
        /// Handles the BeforeCheckNode event of the tlCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraTreeList.CheckNodeEventArgs"/> instance containing the event data.</param>
        private void tlCommands_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }
    }
}