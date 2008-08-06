using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Uniframework
{
    /// <summary>
    /// ��Ļ��긨����
    /// </summary>
    public class WaitCursor : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitCursor"/> class.
        /// </summary>
        /// <param name="showWait">if set to <c>true</c> [show wait].</param>
        public WaitCursor(bool showWait)
        {
            Cursor.Current = showWait ? Cursors.WaitCursor : Cursors.Default;
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide()
        {
            Cursor.Current = Cursors.Default;
        }

        #region IDisposable ��Ա

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Cursor.Current = Cursors.Default;
        }

        #endregion
    }
}
