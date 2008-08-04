using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// 屏幕光标辅助类
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

        #region IDisposable 成员

        public void Dispose()
        {
            Cursor.Current = Cursors.Default;
        }

        #endregion
    }
}
