using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    public interface IDynamicHelpView
    {
        void UpdateHelpUrl(Uri url);
        void HomeUrlChanged();
    }
}
