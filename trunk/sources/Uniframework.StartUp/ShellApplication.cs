using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniframework.SmartClient;
using Microsoft.Practices.CompositeUI;
using System.Windows.Forms;

namespace Uniframework.StartUp
{
    public class ShellApplication<TWorkItem, TShell> : UniframeworkApplication<TWorkItem, TShell>
        where TWorkItem : WorkItem, new()
        where TShell : Form
    {
    }
}
