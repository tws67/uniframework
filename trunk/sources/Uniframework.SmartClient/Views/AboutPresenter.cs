using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.Configuration;
using Microsoft.Practices.CompositeUI.Services;

namespace Uniframework.SmartClient.Views
{
    public class AboutPresenter : Presenter<frmAbout>
    {
        private IModuleEnumerator moduleEnumerator;

        [ServiceDependency]
        public IModuleEnumerator ModuleEnumerator
        {
            set { moduleEnumerator = value; }
            get { return moduleEnumerator; }
        }

        public string NetFrameworkVersion
        {
            get { return Environment.Version.ToString(); }
        }

        public string Copyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(
                  Assembly.GetEntryAssembly(), typeof(AssemblyCopyrightAttribute))).Copyright;
            }
        }

        public override void OnViewReady()
        {
            base.OnViewReady();

            foreach (IModuleInfo info in ModuleEnumerator.EnumerateModules())
            {
                AssemblyName assemblyName = Assembly.ReflectionOnlyLoadFrom(info.AssemblyFile).GetName();
                View.AddModule(assemblyName.Name, assemblyName.Version.ToString());
            }
        }

        public void CopyInfo()
        {
            StringBuilder str = new StringBuilder();

            str.Append(Application.ProductName);
            str.Append("\t");
            str.AppendLine(Application.ProductVersion);
            str.AppendLine();

            str.Append(".NET Framework");
            str.Append("\t");
            str.AppendLine(Assembly.GetExecutingAssembly().ImageRuntimeVersion);
            str.AppendLine();

            foreach (IModuleInfo info in moduleEnumerator.EnumerateModules())
            {
                AssemblyName assemblyName = Assembly.ReflectionOnlyLoadFrom(info.AssemblyFile).GetName();
                str.Append(info.AssemblyFile);
                str.Append("\t");
                str.Append(assemblyName.Version.ToString());
                str.AppendLine();
            }

            Clipboard.SetText(str.ToString(), TextDataFormat.UnicodeText);
        }

        public void SystemInfo()
        {
            try { System.Diagnostics.Process.Start("MSInfo32.exe"); }
            catch { }
        }

        public void Close()
        {
            CloseView();
        }
    }
}
