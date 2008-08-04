using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// ��Ļ��긨����
    /// </summary>
    public class WaitCursor : IDisposable
    {
        public WaitCursor(bool showWait)
        {
            Cursor.Current = showWait ? Cursors.WaitCursor : Cursors.Default;
        }

        public void Show()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        public void Hide()
        {
            Cursor.Current = Cursors.Default;
        }

        #region IDisposable ��Ա

        public void Dispose()
        {
            Cursor.Current = Cursors.Default;
        }

        #endregion
    }
}
