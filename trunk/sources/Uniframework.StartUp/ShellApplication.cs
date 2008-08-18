using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Uniframework.SmartClient;

namespace Uniframework.StartUp
{
    public class ShellApplication : UniframeworkApplication<ControlledWorkItem<RootWorkItemController>, ShellForm>
    {
    }
}
