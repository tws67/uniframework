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
 */

#region Namespace imports

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// Helper class to draw a line.
    /// </summary>
    public partial class Line : Control
    {
        /// <summary>
        /// Creates a new line control.
        /// </summary>
        public Line()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.White, 0, 0, this.Width, 0);
            base.OnPaint(e);
        }
    }
}
