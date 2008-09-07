using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uniframework.UpgradeLaunch
{
    class Program
    {
        [STAThread]
        static void Main(string[] argv)
        {
            if (argv.Length != 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new LaunchProgress(argv[0]));
            }
            else
                Application.Exit();
        }
    }
}
