/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    /// <summary>
    /// Entity generator wizard.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
    public class CreateSpmlWizard : IWizard
    {
        #region Private members

        /// <summary>
        /// Name of the custom code generator tool.
        /// </summary>
        private string _tool;

        /// <summary>
        /// Indicates whether SPML generation succeeded; used to determine whether or not the file should be written.
        /// </summary>
        private bool _ok;

        #endregion

        #region Methods

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
            if (projectItem == null)
                throw new ArgumentNullException("projectItem");

            projectItem.Properties.Item("CustomTool").Value = _tool;
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _DTE dte = automationObject as _DTE;
            IDEWindow window = new IDEWindow(new IntPtr(dte.MainWindow.HWnd));

            if (runKind == WizardRunKind.AsNewItem)
            {
                _tool = replacementsDictionary["$CustomTool$"];

                string name = null;
                if (replacementsDictionary.ContainsKey("$rootname$"))
                {
                    name = replacementsDictionary["$rootname$"];
                    if (name.EndsWith(".spml", StringComparison.OrdinalIgnoreCase))
                        name = name.Substring(0, name.Length - ".spml".Length);
                }

                Context ctx = new Context();
                ctx.Name = name;
                ctx.Connection = new Connection();
                ctx.Connection.CustomAuthentication = false;
                ctx.Connection.User = replacementsDictionary.ContainsKey("$username$") ? replacementsDictionary["$username$"] : null;
                ctx.Connection.Domain = replacementsDictionary.ContainsKey("$userdomain$") ? replacementsDictionary["$userdomain$"] : null;

                Wizard start = new Wizard(ctx);
                DialogResult res = start.ShowDialog(window);
                _ok = res == DialogResult.OK;

                if (_ok)
                {
                    XmlDocument spml = new XmlDocument();
                    spml.InnerXml = start.WizardContext.ResultContext.ToSpml().OuterXml;

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    settings.OmitXmlDeclaration = true;
                    StringBuilder sb = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(sb, settings))
                        spml.WriteTo(writer);

                    replacementsDictionary.Add("$spml$", sb.ToString());
                    replacementsDictionary.Add("$version$", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                }
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return _ok;
        }

        #endregion
    }

    /// <summary>
    /// Helper class for the Visual Studio IDE window.
    /// </summary>
    internal class IDEWindow : IWin32Window
    {
        /// <summary>
        /// Handle to the IDE window.
        /// </summary>
        private IntPtr _handle;

        /// <summary>
        /// Creates a new Visual Studio IDE window wrapper.
        /// </summary>
        /// <param name="handle">Handle to the IDE window.</param>
        public IDEWindow(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// Gets the handle to the IDE window.
        /// </summary>
        public IntPtr Handle
        {
            get { return _handle; }
        }
    }
}
