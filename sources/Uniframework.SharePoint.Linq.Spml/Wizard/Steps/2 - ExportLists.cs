/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 * 
 * 0.2.2 - Introduction of entity wizard
 * 0.2.4 - Configurable support for SharePoint Object Model data provider
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// List export step of the entity generator wizard.
    /// </summary>
    public partial class ExportLists : UserControl, IWizardStep
    {
        #region Private members

        /// <summary>
        /// Wizard context.
        /// </summary>
        private WizardContext ctx;

        /// <summary>
        /// Indicates whether the wizard can go to the next step.
        /// </summary>
        private bool _next;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the list export step.
        /// </summary>
        /// <param name="context">Wizard context.</param>
        public ExportLists(WizardContext context)
        {
            this.ctx = context;

            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Title of the wizard step.
        /// </summary>
        public string Title
        {
            get { return "Define list entities"; }
        }

        /// <summary>
        /// Indicates whether the step allows to go to the next step currently. Will be true once the connection test has passed.
        /// </summary>
        public bool CanNext
        {
            get { return _next; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the state of the step has changed, e.g. when CanNext has changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Event raised when the step is performing work.
        /// </summary>
        public event EventHandler Working;

        /// <summary>
        /// Event raised when the step has completed its work.
        /// </summary>
        public event EventHandler WorkCompleted;

        /// <summary>
        /// Raises the StateChanged event.
        /// </summary>
        public void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raises the Working event.
        /// </summary>
        public void OnWorking()
        {
            if (Working != null)
                Working(this, new EventArgs());
        }

        /// <summary>
        /// Raises the WorkCompleted event.
        /// </summary>
        public void OnWorkCompleted()
        {
            if (WorkCompleted != null)
                WorkCompleted(this, new EventArgs());
        }

        #endregion

        #region Event handlers

        private void ExportLists_Load(object sender, EventArgs e)
        {
            //
            // Set split containers settings. Can't be moved to the InitializeComponent method due to a Windows Forms bug.
            //
            this.panel.Panel1MinSize = 100;
            this.panel.Panel2MinSize = 300;
            this.splitContainer2.Panel1MinSize = 100;
            this.splitContainer2.Panel2MinSize = 200;

            //
            // Get lists and set context name "best guess".
            //
            txtContext.Text = ctx.ResultContext.Name ?? ctx.ResultContext.Url.Host.Replace('.', ' ');
            PopulateLists();
        }

        private void lists_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoListSelect();
        }

        private void DoListSelect()
        {
            //
            // Make sure an item has been selected; single-select mode should be on.
            //
            if (lists.SelectedItems.Count == 1)
            {
                //
                // Get the selected item; it should be of type ListListViewItem.
                //
                ListListViewItem item = lists.SelectedItems[0] as ListListViewItem;
                Debug.Assert(item != null);

                //
                // If list definition isn't loaded yet, load it now.
                //
                if (!item.Loaded)
                {
                    //
                    // Will happen asynchronously. The same event will be raised again, but item.Loaded will be true.
                    //
                    LoadList(item, false);
                }
                else
                {
                    //
                    // Show the fields.
                    //
                    fields.Items.Clear();
                    fields.Items.AddRange(item.Fields.ToArray());
                    fields.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);

                    //
                    // Show properties for the list.
                    //
                    properties.SelectedObject = item.List;
                }
            }
        }

        private void lists_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //
            // Track selection of list mappings. Make sure the selected item is a ListListViewItem.
            //
            ListListViewItem item = e.Item as ListListViewItem;
            Debug.Assert(item != null);

            //
            // If checked, make sure the list definition is loaded and add it to the result context.
            //
            if (item.Checked)
            {
                if (!item.Loaded)
                    LoadList(item, true);
                else
                {
                    ctx.ResultContext.Lists.Add(item.List);

                    //
                    // Work available. Signal "can next" as true.
                    //
                    _next = true;
                    OnStateChanged();

                    //
                    // Checking an item should cause it to get selected.
                    //
                    item.Selected = true;
                    lists.Focus();
                }
            }
            //
            // If unchecked, remove it from the result context.
            //
            else if (ctx.ResultContext.Lists.Contains(item.List))
            {
                ctx.ResultContext.Lists.Remove(item.List);

                //
                // Work left to be done? If not signal "can next" as false.
                //
                if (ctx.ResultContext.Lists.Count == 0)
                {
                    _next = false;
                    OnStateChanged();
                }
            }
        }

        private void fields_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoFieldSelect();
        }

        private void DoFieldSelect()
        {
            //
            // Make sure an item has been selected; single-select mode should be on.
            //
            if (fields.SelectedItems.Count == 1)
            {
                //
                // Get the selected item; it should be of type FieldListViewItem.
                //
                FieldListViewItem field = fields.SelectedItems[0] as FieldListViewItem;
                Debug.Assert(field != null);

                //
                // Show properties for the field.
                //
                properties.SelectedObject = field.Field;
            }
        }

        private void fields_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            //
            // Track selection of field mappings. Make sure the selected item is a FieldListViewItem.
            //
            FieldListViewItem item = e.Item as FieldListViewItem;
            Debug.Assert(item != null);

            //
            // Set inclusion in export.
            //
            item.Field.Include = item.Checked;
            properties.Refresh();
        }

        private void fields_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //
            // Track selection of field mappings. Make sure the selected item is a FieldListViewItem.
            //
            FieldListViewItem item = fields.Items[e.Index] as FieldListViewItem;
            Debug.Assert(item != null);

            if (item.Field.IsPrimaryKey)
                e.NewValue = CheckState.Checked;
        }

        private void chkContext_CheckedChanged(object sender, EventArgs e)
        {
            //
            // Allow or disallow to set context name.
            //
            chkSom.Enabled = txtContext.Enabled = chkContext.Checked;
            if (!chkContext.Checked)
                ctx.ResultContext.Name = "";
            else
                ctx.ResultContext.Name = txtContext.Text;
        }

        private void txtContext_TextChanged(object sender, EventArgs e)
        {
            //
            // Set context name.
            //
            ctx.ResultContext.Name = txtContext.Text;
        }

        private void chkSom_CheckedChanged(object sender, EventArgs e)
        {
            //
            // Enable/disable support for SharePoint Object Model data provider.
            //
            ctx.ResultContext.EnableObjectModelProvider = chkSom.Checked;
        }

        private void lists_Enter(object sender, EventArgs e)
        {
            if (lists.SelectedItems.Count == 1)
            {
                ListListViewItem list = lists.SelectedItems[0] as ListListViewItem;
                Debug.Assert(list != null);

                properties.SelectedObject = list.List;
            }
        }

        private void fields_Enter(object sender, EventArgs e)
        {
            if (fields.SelectedItems.Count == 1)
            {
                FieldListViewItem field = fields.SelectedItems[0] as FieldListViewItem;
                Debug.Assert(field != null);

                properties.SelectedObject = field.Field;
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Populates the list box with SharePoint lists.
        /// </summary>
        private void PopulateLists()
        {
            lists.Items.Clear();
            foreach (List list in ctx.FullContext.Lists)
                lists.Items.Add(new ListListViewItem(list));
            lists.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private ListListViewItem itemUpdating;
        private bool checkListAfterLoad;

        /// <summary>
        /// Loads a list definition.
        /// </summary>
        /// <param name="item">List view item for the list mapping object to load the list definition for.</param>
        /// <param name="check">Indicates the list should be checked after loading or not.</param>
        private void LoadList(ListListViewItem item, bool check)
        {
            //
            // Load list in background.
            //
            OnWorking();
            itemUpdating = item;
            checkListAfterLoad = check;
            bgList.RunWorkerAsync(item);
        }

        private void bgList_DoWork(object sender, DoWorkEventArgs e)
        {
            //
            // Get worker invocation argument.
            //
            ListListViewItem item = e.Argument as ListListViewItem;
            Debug.Assert(item != null);

            //
            // Get the list object.
            //
            e.Result = Helpers.GetList(ctx.FullContext.Connection, item.List.Id.ToString());
        }

        private void bgList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    //
                    // Update the item with the result.
                    //
                    itemUpdating.List = (List)e.Result;
                    itemUpdating.Loaded = true;

                    //
                    // Create list items for the field mappings for the list. Check all of the known fields by default.
                    //
                    itemUpdating.Fields.Clear();
                    foreach (Field field in itemUpdating.List.GetKnownFields())
                    {
                        FieldListViewItem fi = new FieldListViewItem(field);
                        fi.Checked = field.Include = true;
                        itemUpdating.Fields.Add(fi);
                    }

                    //
                    // Self-select.
                    //
                    lists.SelectedItems.Clear();
                    itemUpdating.Selected = true;

                    //
                    // Self-check. Toggle check mode to make sure the item is selected.
                    //
                    if (checkListAfterLoad)
                    {
                        itemUpdating.Checked = false;
                        itemUpdating.Checked = true;
                    }
                }
                else
                {
                    MessageBoxOptions options = Wizard.IsRightToLeft(this) ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : 0;
                    MessageBox.Show("An error occurred while connecting to the server.\n\n" + e.Error.Message, this.ParentForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
                }
            }

            //
            // Signal background work completed.
            //
            OnWorkCompleted();
            lists.Focus();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Instructs the step to try to cancel a pending operation. This will try to stop a pending background connection operation, if any.
        /// </summary>
        public void Cancel()
        {
            if (bgList.IsBusy && !bgList.CancellationPending)
                bgList.CancelAsync();
        }

        #endregion
    }

    /// <summary>
    /// List view item for a SharePoint list mapping object.
    /// </summary>
    class ListListViewItem : ListViewItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new list view item for a SharePoint list mapping object.
        /// </summary>
        /// <param name="list">List mapping object.</param>
        public ListListViewItem(List list)
            : base(list.Name)
        {
            this.List = list;
            this.Loaded = false;
            this.Fields = new List<FieldListViewItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// List mapping object.
        /// </summary>
        public List List { get; set; }

        /// <summary>
        /// Indicates whether or not the list definition has been loaded already.
        /// </summary>
        public bool Loaded { get; set; }

        /// <summary>
        /// List view items for the SharePoint list's SharePoint field mapping objects.
        /// </summary>
        public List<FieldListViewItem> Fields { get; set; }

        #endregion
    }

    /// <summary>
    /// List view item for a SharePoint field mapping object.
    /// </summary>
    class FieldListViewItem : ListViewItem
    {
        #region Constructors

        /// <summary>
        /// Creates a new list view item for a SharePoint field mapping object.
        /// </summary>
        /// <param name="field">Field mapping object.</param>
        public FieldListViewItem(Field field)
            : base(field.DisplayName)
        {
            this.Field = field;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field mapping object.
        /// </summary>
        public Field Field { get; set; }

        #endregion
    }
}
