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
 * 0.2.3 - Introduction of Choice class
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Form to provide editing support for a series of Choices.
    /// </summary>
    public partial class ChoiceEditorForm : Form
    {
        #region Private members

        /// <summary>
        /// Choice collection to edit.
        /// </summary>
        private List<Choice> _choices;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Choice collection editing form.
        /// </summary>
        /// <param name="choices">Choice collection to edit.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public ChoiceEditorForm(List<Choice> choices)
        {
            _choices = choices;

            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the choice collection to edit.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<Choice> Choices { get { return _choices; } }

        #endregion

        #region Event handlers

        private void ChoiceEditorForm_Load(object sender, EventArgs e)
        {
            choices.Items.AddRange(_choices.ToArray());
        }

        private void choices_SelectedIndexChanged(object sender, EventArgs e)
        {
            properties.SelectedObject = choices.SelectedItem;
        }

        #endregion
    }
}
