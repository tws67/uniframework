using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraGrid;

namespace Uniframework.SmartClient
{
    public interface IXtraGridView
    {

        GridControl Grid { get; }
    }
}
