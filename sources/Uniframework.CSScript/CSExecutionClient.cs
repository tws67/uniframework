using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;

namespace csscript
{
    delegate void PrintDelegate(string msg);

    /// <summary>
    /// Wrapper class that runs CSExecutor within console application context.
    /// </summary>
    class CSExecutionClient
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            SetEnvironmentVariable("CSScriptRuntime", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            CSExecutor exec = new CSExecutor();

            if (AppDomain.CurrentDomain.FriendlyName != "ExecutionDomain") // AppDomain.IsDefaultAppDomain is more appropriate but it is not available in .NET 1.1
            {
                string configFile = exec.GetCustomAppConfig(args);
                if (configFile != "")
                {
                    AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                    setup.ConfigurationFile = configFile;

                    AppDomain appDomain = AppDomain.CreateDomain("ExecutionDomain", null, setup);
                    appDomain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location, null, args);
                    return;
                }
            }
            AppInfo.appName = new FileInfo(Application.ExecutablePath).Name;
            exec.Execute(args, new PrintDelegate(Print), null);
        }
        /// <summary>
        /// Implementation of displaying application messages.
        /// </summary>
        static void Print(string msg)
        {
            Console.WriteLine(msg);
        }
    }
    /// <summary>
    /// Repository for application specific data
    /// </summary>
    class AppInfo
    {
        public static string appName = ".NET Common Language Script Engine, based on C# script.";
        public static bool appConsole = true;
        public static string appLogo
        {
            get { return ".NET Common Language Script Engine, based on C# script. Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ".\nCopyright (C) 1998-2007 Sjteksoft.\n"; }
        }
        public static string appLogoShort
        {
            get { return ".NET Common Language Script Engine, based on C# script. Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ".\n"; }
        }
        public static string appParams = "[/nl]:";
        public static string appParamsHelp = "nl   - 执行期间不显示脚本引擎的版本信息\n";
    }
}
