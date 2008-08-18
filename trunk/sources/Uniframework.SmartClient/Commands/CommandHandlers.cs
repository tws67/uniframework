using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 框架通用的操作命令项
    /// </summary>
    public class CommandHandlers : Controller
    {
        /// <summary>
        /// Called when [command click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [CommandHandler(CommandHandlerNames.CMD_CLICKME)]
        public void OnCommandClick(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Command name " + ((Command)sender).Name);
            sb.AppendLine("You click me at " + DateTime.Now.ToLocalTime());
            MessageBox.Show(sb.ToString());
        }
    }
}
