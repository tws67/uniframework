/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 * 
 * 0.1.0 - Alpha
 * 0.1.1 - Added Visual Basic language support and -language command line switch
 *         Bug fix for DateTime? and Nullable(Of DateTime)
 * 0.1.3 - Bug fix for FillInChoice attribute presence check (not required)
 *         Bug fix for (Multi)Choice fields containing choices starting with a digit
 * 0.2.0 - Generates entity types deriving from SharePointEntityType
 *         Read-only property generation for read-only fields
 *         Support for Counter field type for primary key values
 *         Infer file name for entity class from list's name, not the command-line parameter
 *         Restructuring of class files in project
 *         Hosting model with events
 *         Support for Lookup fields with recursive entity type generation
 * 0.2.1 - Use of EntityGenerator back-end
 *         Refactoring of argument parsing to Args class.
 * 0.2.2 - New entity model
 */

#region Namespace imports

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.SPMetal
{
    class Program
    {
        /// <summary>
        /// Entry point for SpMetal.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        static void Main(string[] args)
        {
            //
            // Unhandled exception signaling.
            //
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Error);

            //
            // Title including assembly version number.
            //
            Console.WriteLine("Bart De Smet SpMetal SharePoint List Definition Export version {0}", Assembly.GetEntryAssembly().GetName().Version);
            Console.WriteLine("Copyright (C) Sjteksoft 2008. All rights reserved.\n");

            //
            // Parse arguments.
            //
            Args a = Args.Parse(args);

            //
            // Invalid arguments: display help information and exit.
            //
            if (a == null)
            {
                ShowSyntaxInfo();
                return;
            }

            //
            // Entity generator.
            //
            Generator gen = new Generator(
                                new EntityGeneratorArgs()
                                {
                                    RunMode = a.RunMode,
                                    Connection = new Connection()
                                                 {
                                                     CustomAuthentication = a.User != null,
                                                     Url = a.Url,
                                                     User = a.User,
                                                     Password = a.Password,
                                                     Domain = a.Domain
                                                 },
                                    Namespace = a.Namespace,
                                    Language = (a.Language == "VB" ? Language.VB : Language.CSharp)
                                }
                            );

            //
            // Pluralization setting.
            //
            List.AutoPluralize = a.Pluralize;

            //
            // Get SPML first.
            //
            XmlDocument spml = new XmlDocument();

            //
            // Check run mode: online or offline.
            //
            if ((a.RunMode & RunModes.Online) == RunModes.Online)
            {
                try
                {
                    spml.InnerXml = gen.GenerateSpml(a.Context, a.EnableSom, a.List).InnerXml;
                }
                catch (EntityGeneratorException ex)
                {
                    Console.WriteLine("Failed to generate SPML. " + ex.Message);
                    return;
                }
            }
            else if ((a.RunMode & RunModes.Offline) == RunModes.Offline)
            {
                try
                {
                    spml.InnerXml = File.ReadAllText(a.In);
                }
                catch (XmlException ex)
                {
                    Console.WriteLine("Invalid SPML file. " + ex.Message);
                    return;
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Failed to read input file. " + ex.Message);
                    return;
                }
            }
            else
                Debug.Assert(false);

            //
            // Just save the SPML.
            //
            if ((a.RunMode & RunModes.Export) == RunModes.Export)
            {
                //
                // Write to output file.
                //
                try
                {
                    using (FileStream fs = File.Open(a.Xml, FileMode.Create, FileAccess.Write))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.Encoding = Encoding.UTF8;
                        using (XmlWriter writer = XmlWriter.Create(fs, settings))
                        {
                            spml.WriteTo(writer);
                            Console.WriteLine("Output written to {0}.", a.Xml);
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine("Failed to write ouput. " + ex.Message);
                    return;
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Failed to write ouput. " + ex.Message);
                    return;
                }
            }
            //
            // Create and save entity code.
            //
            else if ((a.RunMode & RunModes.CodeGen) == RunModes.CodeGen)
            {
                //
                // Generate code in the appropriate language.
                //
                CodeCompileUnit compileUnit;
                try
                {
                    bool som;
                    compileUnit = gen.GenerateCode(spml, out som);
                }
                catch (EntityGeneratorException ex)
                {
                    Console.WriteLine(ex.Message);
                    if (ex.Data.Contains("messages"))
                    {
                        Console.WriteLine("\nSchema validation messages:");
                        foreach (var s in (List<ValidationEventArgs>)ex.Data["messages"])
                            Console.WriteLine("- [{3}] {0} ({1},{2})", s.Message, s.Exception.LineNumber, s.Exception.LinePosition, s.Severity.ToString().ToUpperInvariant());
                    }
                    return;
                }
                CodeDomProvider cdp = CodeDomProvider.CreateProvider(a.Language);
                StringBuilder code = new StringBuilder();
                TextWriter tw = new StringWriter(code, CultureInfo.InvariantCulture);
                cdp.GenerateCodeFromCompileUnit(compileUnit, tw, null);

                //
                // Infer file name from list name if not specified on the command-line (v0.2.0.0).
                //
                if (a.Code == null || a.Code.Length == 0)
                {
                    string file;

                    if (!string.IsNullOrEmpty(a.Context))
                        file = a.Context + "SharePointDataContext";
                    else
                    {
                        XmlAttribute ctx = spml["SharePointDataContext"].Attributes["Name"];
                        if (ctx != null && !string.IsNullOrEmpty(ctx.Value))
                            file = ctx.Value + "SharePointDataContext";
                        else if (!string.IsNullOrEmpty(a.List))
                            file = a.List; //TODO: non-GUID file name
                        else
                            file = Path.GetRandomFileName();
                    }

                    file += (a.Language == "CS" ? ".cs" : ".vb");
                    foreach (char c in Path.GetInvalidFileNameChars())
                        file = file.Replace(c.ToString(), "");
                    a.Code = file;
                }

                //
                // Write to output file.
                //
                try
                {
                    using (StreamWriter sw = File.CreateText(a.Code))
                    {
                        sw.WriteLine(code.ToString());
                        Console.WriteLine("Output written to {0}.", a.Code);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine("Failed to write ouput. " + ex.Message);
                    return;
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Failed to write ouput. " + ex.Message);
                    return;
                }
            }
            else
                Debug.Assert(false);
        }

        /// <summary>
        /// Catch all for unexpected application-level exceptions.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event arguments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void Error(object sender, UnhandledExceptionEventArgs e)
        {
            //
            // Create message.
            //
            Exception ex = e.ExceptionObject as Exception;
            string message = null;
            if (ex != null)
                message = String.Format(CultureInfo.InvariantCulture, "{0}: {1}\r\n{2}", ex.GetType().FullName, ex.Message, ex.StackTrace);

            //
            // Print error message.
            //
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Fatal error occurred. Please contact the LINQ to SharePoint team.\n");
            Console.ResetColor();

            //
            // Log error report.
            //
            if (message != null)
            {
                string report = Path.GetTempFileName();
                try
                {
                    using (StreamWriter sw = new StreamWriter(report))
                    {
                        sw.WriteLine("Error report for LINQ to SharePoint SpMetal version {0}\r\n", Assembly.GetEntryAssembly().GetName().Version);
                        sw.WriteLine("Generated on {0}", DateTime.Now);
                        sw.WriteLine("Running {0}", Environment.OSVersion.VersionString);
                        sw.WriteLine();
                        sw.WriteLine(message);
                        sw.WriteLine();
                        sw.WriteLine("End of report");
                    }
                    Console.WriteLine("An error report was written to:\n{0}", report);

#if DEBUG
                    Process.Start("notepad.exe", report);
#endif
                }
                catch { } //Failure to write error report can be ignored.
            }

            Environment.Exit(-1);
        }

        /// <summary>
        /// Shows syntax information on the console, in response to a -? or -help call or when invalid arguments have been supplied.
        /// </summary>
        private static void ShowSyntaxInfo()
        {
            Console.WriteLine("No inputs specified\n");

            string file = Assembly.GetEntryAssembly().GetName().Name + ".exe";
            Console.WriteLine("Usage: {0} [options]", file);
            Console.WriteLine();

            Console.WriteLine("Syntax:");
            Console.WriteLine("  {0} [{{online}}|{{offline}}]", file);
            Console.WriteLine("  {online}  := -url:<url> -list:<list> [{context}] {connect} {option}");
            Console.WriteLine("  {offline} := -in:<file> {codegen}");
            Console.WriteLine("  {context} := -context:<context> [-enablesom]");
            Console.WriteLine("  {connect} := [-user:<user> -password:<password> [-domain:<domain>]]");
            Console.WriteLine("  {option}  := [{codegen}|{export}]");
            Console.WriteLine("  {codegen} := [-code:<file>] [-language:<lang>] [-namespace:<ns>] [-pluralize]");
            Console.WriteLine("  {export}  := [-xml:<file>]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -url:<url>            URL to the root of the SharePoint site");
            Console.WriteLine("  -user:<user>          User name for connection to SharePoint site");
            Console.WriteLine("  -password:<password>  Password for connection to SharePoint site");
            Console.WriteLine("  -domain:<domain>      Domain for connection to SharePoint site");
            Console.WriteLine("  -list:<list>          Name of the list to export (* = all lists)");
            Console.WriteLine("  -context:<context>    Name of the context to create");
            Console.WriteLine("  -in:<file>            Input file with SPML for code generation");
            Console.WriteLine("  -xml:<file>           Output file for SPML generation");
            Console.WriteLine("  -code:<file>          Output file for code generation (= -out:<file>)");
            Console.WriteLine("  -language:<lang>      Code language used for output: VB or CS (default)");
            Console.WriteLine("  -namespace:<ns>       Namespace to put generated code in");
            Console.WriteLine("  -pluralize            Auto-(de)pluralize list entity names");
            Console.WriteLine("  -enablesom            Adds support for the SharePoint Object Model provider");
            Console.WriteLine();
            Console.WriteLine("Samples:");
            Console.WriteLine("To export one list to code (online codegen).");
            Console.WriteLine("  {0} -url:http://wss -list:Products -language:CS -code:Products.cs", file);
            Console.WriteLine();
            Console.WriteLine("To generate an SPML mapping definition for all lists (online export).");
            Console.WriteLine("  {0} -url:http://wss3demo -list:* -xml:Northwind.dbml", file);
            Console.WriteLine();
            Console.WriteLine("To generate code for an SPML mapping definition (offline codegen).");
            Console.WriteLine("  {0} -in:Northwind.spml -language:CS -code:Northwind.cs", file);
        }
    }
}