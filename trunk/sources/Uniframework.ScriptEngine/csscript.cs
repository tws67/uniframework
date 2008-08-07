#region Licence...
//-----------------------------------------------------------------------------
// Date:	17/10/04	Time: 2:33p
// Module:	csscript.cs
// Classes:	CSExecutor
//			ExecuteOptions
//
// This module contains the definition of the CSExecutor class. Which implements 
// compiling C# code and executing 'Main' method of compiled assembly
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2004. All rights reserved.
//
// Redistribution and use of this code in source and binary forms, without 
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright notice, 
//	this list of conditions and the following disclaimer. 
// 2. Neither the name of an author nor the names of the contributors may be used 
//	to endorse or promote products derived from this software without specific 
//	prior written permission.
// 3. This code may be used in compiled form in any way you desire. This
//	  file may be redistributed unmodified by any means PROVIDING it is 
//	not sold for profit without the authors written consent, and 
//	providing that this notice and the authors name is included. 
//
// Redistribution and use of this code in source and binary forms, with modification, 
// are permitted provided that all above conditions are met and software is not used 
// or sold for profit.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR 
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF 
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//	Caution: Bugs are expected!
//----------------------------------------------
#endregion 

using System;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Security.Policy;
using System.Collections;
using System.Threading;
using System.Text;
using System.Globalization;
using CSScriptLibrary;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.XPath;
using System.Drawing.Design;
using System.Design;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace csscript
{
	#region Settings...
	/// <summary>
	/// Settings is an class that holds CS-Script application settings.
	/// </summary>
	public class Settings
	{	
		/// <summary>
		/// Command to be executed to perform custom cleanup. 
		/// If this value is empty automatic cleanup of all 
		/// temporary files will occurs after the script execution.
		/// This imp lays that the script has to be executed in the 
		/// separate AppDomain and some performance penalty will be incurred.
		/// 
		/// Setting this value to the command for custom cleanup application 
		/// (e.g. csc.exe cleanTemp.cs) will force the script engine to execute 
		/// script in the 'current' AppDomain what will improve performance.
		/// </summary>
		[Category("CustomCleanup")]
		public string CleanupShellCommand
		{
			get { return cleanupShellCommand; }
			set { cleanupShellCommand = value; }
		}
		/// <summary>
		/// Returns value of the CleanupShellCommand (with expanding environment variables). 
		/// </summary>
		/// <returns>shell command string</returns>
		public string ExpandCleanupShellCommand() {return Environment.ExpandEnvironmentVariables(cleanupShellCommand);} 
		private string cleanupShellCommand = "";
		
		/// <summary>
		/// This value indicates frequency of the custom cleanup
		/// operation. It has affect only if CleanupShellCommand is not empty.
		/// </summary>
		[Category("CustomCleanup")]
		public uint DoCleanupAfterNumberOfRuns
		{
			get { return doCleanupAfterNumberOfRuns; }
			set { doCleanupAfterNumberOfRuns = value; }
		}
		private uint doCleanupAfterNumberOfRuns = 30;
		
		/// <summary>
		/// Location of alternative code provider assembly. If set it forces script engine to use an alternative code compiler. 
		/// </summary>
		[Category("Extensibility"), Editor(typeof(System.Windows.Forms.Design.FileNameEditor),typeof(UITypeEditor))] 
		public string UseAlternativeCompiler 
		{
			get { return useAlternativeCompiler; }
			set { useAlternativeCompiler = value; }
		}
		/// <summary>
		/// Returns value of the UseAlternativeCompiler (with expanding environment variables). 
		/// </summary>
		/// <returns>Path string</returns>
		public string ExpandUseAlternativeCompiler() {return Environment.ExpandEnvironmentVariables(useAlternativeCompiler);} 
		private string useAlternativeCompiler = "";

		/// <summary>
		/// DefaultApartmentState is an ApartmemntState, which will be used 
		/// at run-time if none specified in the code with COM threading model attributes.
		/// </summary>
		[Category("RuntimeSettings")]
		public ApartmentState DefaultApartmentState
		{
			get { return defaultApartmentState; }
			set { defaultApartmentState = value; }
		}
		private ApartmentState defaultApartmentState = ApartmentState.STA;

		/// <summary>
		/// Default command-line arguments. For example if "/dbg" is specified all scripts will be compiled in debug mode
		/// regardless if the user specified "/dbg" when a particular script is launched.
		/// </summary>
		[Category("RuntimeSettings")]
		public string DefaultArguments
		{
			get { return defaultArguments; }
			set { defaultArguments = value; }
		}
		private string defaultArguments = "/c";
		/// <summary>
		/// The value, which indicates if auto-generated files (if any) should should be hidden in the temporary directory.
		/// </summary>
		[Category("RuntimeSettings")]
		public HideOptions HideAutoGeneratedFiles
		{
			get { return hideOptions; }
			set { hideOptions = value; }
		}
		private HideOptions hideOptions = HideOptions.HideMostFiles;
		/// <summary>
		/// Enum for possible hide auto-generated files sceenarious
		/// Note: when HideAll is used it is responsibility of the pre/post script to implement actual hiding. 
		/// </summary>
		public enum HideOptions
		{
			/// <summary>
			/// Do not hide auto-generated files.
			/// </summary>
			DoNotHide,
			/// <summary>
			/// Hide the most of the auto-generated (cache and "imported") files.
			/// </summary>
			HideMostFiles,
			/// <summary>
			/// Hide all auto-generated files including the files generated by pre/post scripts.
			/// </summary>
			HideAll
		}
		/// <summary>
		/// Bolean flag that indicates how much error detais to be reported should error occure.
		/// false - Top level exception will be reported
		/// true - Whole exception stack will be reported
		/// </summary>
		[Category("RuntimeSettings")]
		public bool ReportDetailedErrorInfo
		{
			get { return reportDetailedErrorInfo; }
			set { reportDetailedErrorInfo = value; }
		}
		private bool reportDetailedErrorInfo = false;
		/// <summary>
		/// Bolean flag that indicates if compiler warnings should be included in script compilation output.
		/// false - warnings will be displayed
		/// true - warnings will not be displayed
		/// </summary>
		[Category("RuntimeSettings")]
		public bool HideCompilerWarnings
		{
			get { return hideCompilerWarnings; }
			set { hideCompilerWarnings = value; }
		}
		private bool hideCompilerWarnings = false;
		/// <summary>
		/// Directory that user can specify to indicate location of the scripts to be imported/executed 
		/// without specifying the script full path. %CSSCRIPT_DIR%\lib is a hard coded system directory for importable 
		/// scripts. ExtraImportDirectory is a user-defined one.
		/// </summary>
		[Category("Extensibility"), Editor(typeof(System.Windows.Forms.Design.FolderNameEditor),typeof(UITypeEditor))]
		public string ExtraLibDirectory
		{
			get { return extraLibDirectory; }
			set { extraLibDirectory = value; }
		}
		/// <summary>
		/// Returns value of the ExtraLibDirectory (with expanding environment variables). 
		/// </summary>
		/// <returns>Path string</returns>
		public string ExpandExtraLibDirectory() {return Environment.ExpandEnvironmentVariables(extraLibDirectory);} 
		private string extraLibDirectory = "";
		/// <summary>
		/// Saves CS-Script application settings to a file (.dat).
		/// </summary>
		/// <param name="fileName">File name of the .dat file</param>
		public void Save(string fileName)
		{
			//It is very tempting to use XmlSerializer but it adds 200 ms to the 
			//application startup time. Whereas current startup delay for cscscript.exe is hust a 100 ms.
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml("<CSSConfig/>");
				doc.DocumentElement.AppendChild(doc.CreateElement("defaultArguments")).AppendChild(doc.CreateTextNode(defaultArguments));
				doc.DocumentElement.AppendChild(doc.CreateElement("defaultApartmentState")).AppendChild(doc.CreateTextNode(defaultApartmentState.ToString()));
				doc.DocumentElement.AppendChild(doc.CreateElement("reportDetailedErrorInfo")).AppendChild(doc.CreateTextNode(reportDetailedErrorInfo.ToString()));
				doc.DocumentElement.AppendChild(doc.CreateElement("useAlternativeCompiler")).AppendChild(doc.CreateTextNode(useAlternativeCompiler));
				doc.DocumentElement.AppendChild(doc.CreateElement("extraLibDirectory")).AppendChild(doc.CreateTextNode(extraLibDirectory));
				doc.DocumentElement.AppendChild(doc.CreateElement("cleanupShellCommand")).AppendChild(doc.CreateTextNode(cleanupShellCommand));
				doc.DocumentElement.AppendChild(doc.CreateElement("doCleanupAfterNumberOfRuns")).AppendChild(doc.CreateTextNode(doCleanupAfterNumberOfRuns.ToString()));
				doc.DocumentElement.AppendChild(doc.CreateElement("hideOptions")).AppendChild(doc.CreateTextNode(hideOptions.ToString()));
				doc.DocumentElement.AppendChild(doc.CreateElement("hideCompilerWarnings")).AppendChild(doc.CreateTextNode(hideCompilerWarnings.ToString()));
				doc.Save(fileName);
			}
			catch{}
		}
		/// <summary>
		/// Loads CS-Script application settings from a file. Default settings object is returned if it cannot be loaded from the file.
		/// </summary>
		/// <param name="fileName">File name of the XML file</param>
		/// <returns>Setting object deserialized from the XML file</returns>
		public static Settings Load(string fileName)
		{
			return Load(fileName, true);
		}
		/// <summary>
		/// Loads CS-Script application settings from a file.
		/// </summary>
		/// <param name="fileName">File name of the XML file</param>
		/// <param name="createAlways">Create and return default settings object if it cannot be loaded from the file.</param>
		/// <returns>Setting object deserialized from the XML file</returns>
		public static Settings Load(string fileName, bool createAlways)
		{
			Settings settings = new Settings();
			if (File.Exists(fileName))
			{
				try
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(fileName);
					XmlNode data = doc.FirstChild;
					settings.defaultArguments = data.SelectSingleNode("defaultArguments").InnerText;
					settings.defaultApartmentState = (ApartmentState)Enum.Parse(typeof(ApartmentState), data.SelectSingleNode("defaultApartmentState").InnerText, false);
					settings.reportDetailedErrorInfo = data.SelectSingleNode("reportDetailedErrorInfo").InnerText.ToLower() == "true";
					settings.UseAlternativeCompiler = data.SelectSingleNode("useAlternativeCompiler").InnerText;
					settings.extraLibDirectory = data.SelectSingleNode("extraLibDirectory").InnerText;
					settings.cleanupShellCommand = data.SelectSingleNode("cleanupShellCommand").InnerText;
					settings.doCleanupAfterNumberOfRuns = uint.Parse(data.SelectSingleNode("doCleanupAfterNumberOfRuns").InnerText);
					settings.hideOptions = (HideOptions)Enum.Parse(typeof(HideOptions), data.SelectSingleNode("hideOptions").InnerText, true);
					settings.hideCompilerWarnings = data.SelectSingleNode("hideCompilerWarnings").InnerText.ToLower() == "true";
				}
				catch
				{
					if (!createAlways)
						settings = null;
				}
			}
			return settings;
		}
	}

	#endregion
	/// <summary>
	/// CSExecutor is an class that implements execution of *.cs files.
	/// </summary>
	class CSExecutor
	{
		#region Public interface...
		/// <summary>
		/// Force caught exceptions to be rethrown.
		/// </summary>
		public bool Rethrow
		{
			get {return rethrow;}
			set {rethrow = value;}
		}
		/// <summary>
		/// Parses application (script engine) arguments.
		/// </summary>
		/// <param name="args">Arguments</param>
		/// <returns>Index of the first script argument</returns>
		internal int ParseAppArgs(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("/"))
				{
					if (args[i] == "/nl")
					{
						options.noLogo = true;
					}
					else if (args[i] == "/c" && (!options.supressExecution))
					{
						options.useCompiled = true;
					}
					else if (args[i] == "/sconfig")
					{
						options.useScriptConfig = true;
					}
					else if (args[i].StartsWith("/noconfig"))
					{
						options.noConfig = true;
						if (args[i].StartsWith("/noconfig:"))
							options.altConfig = args[i].Substring("/noconfig:".Length);
					}
					else if (args[i].StartsWith("/ca"))
					{
						options.useCompiled = true; 
						options.forceCompile = true;
						options.supressExecution = true;
					}
					else if (args[i].StartsWith("/co:"))
					{
						options.compilerOptions = args[i].Substring("/co:".Length);
					}
					else if (args[i].StartsWith("/cd"))
					{
						options.supressExecution = true;
						options.DLLExtension = true;
					}
					else if (args[i] == "/dbg" || args[i] == "/d")
					{
						options.DBG = true;
					}
					else if (args[i] == "/l")
					{
						options.local = true;
					}
					else if (args[i].StartsWith("/r:"))
					{
						string[] assemblies = args[i].Remove(0, 3).Split(":".ToCharArray());
						options.refAssemblies = assemblies;
					}
					else if (args[i].StartsWith("/e") && !options.buildExecutable)
					{
						options.buildExecutable = true;
						options.supressExecution = true;
						options.buildWinExecutable = args[i].StartsWith("/ew");
					}
					else if (args[0] == "/?" || args[0] == "-?")
					{
						ShowHelp();
						options.processFile = false;
						break;
					}
					else if (args[0] == "/s")
					{
						ShowSample();
						options.processFile = false;
						break;
					}
				}
				else
					return i;
			}
			return args.Length;
		}
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		public void Execute(string[] args, PrintDelegate printDelg, string primaryScript)
		{
			try
			{
				print = printDelg != null ? printDelg : new PrintDelegate(VoidPrint);

				if (args.Length > 0) 
				{
					#region Parse command-line arguments...
					
					//Here we need to separate application arguments from script ones.
					//Script engine arguments are always followed by script arguments
					//[appArgs][scriptFile][scriptArgs][//x]
					ArrayList appArgs = new ArrayList();

					int firstScriptArg = ParseAppArgs(args); 
					if (args.Length <= firstScriptArg)
					{
						Environment.ExitCode = 1;
						return; //no script, no script arguments
					}

					//read persistent settings from configuration file
					Settings settings = null;
				
					if (options.noConfig)
					{
						if (options.altConfig != "")
							settings = Settings.Load(Path.GetFullPath(options.altConfig));
					}
					else
						settings = Settings.Load(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "css_config.xml"));

					if (settings != null)
					{
						options.hideTemp = settings.HideAutoGeneratedFiles;
						options.altCompiler = settings.ExpandUseAlternativeCompiler();
						options.apartmentState = settings.DefaultApartmentState;
						options.reportDetailedErrorInfo = settings.ReportDetailedErrorInfo;
						options.cleanupShellCommand = settings.ExpandCleanupShellCommand();
						options.doCleanupAfterNumberOfRuns = settings.DoCleanupAfterNumberOfRuns;
						options.hideCompilerWarnings = settings.HideCompilerWarnings;

						//process default command-line arguments
						string[] defaultCmdArgs = settings.DefaultArguments.Split(" ".ToCharArray());
						int firstDefaultScriptArg = ParseAppArgs(defaultCmdArgs);
						if (firstDefaultScriptArg != defaultCmdArgs.Length)
						{
							options.scriptFileName = defaultCmdArgs[firstDefaultScriptArg];
							for (int i = firstDefaultScriptArg+1; i < defaultCmdArgs.Length; i++)
								if (defaultCmdArgs[i].Trim().Length != 0) 
									appArgs.Add(defaultCmdArgs[i]);
						}	
					}

					//process original command-line arguments
					if (options.scriptFileName == "")
					{
						options.scriptFileName = args[firstScriptArg];	
						firstScriptArg++;
					}

					for (int i = firstScriptArg; i < args.Length; i++)
					{
						if (args[i].Trim().Length != 0)
						{
							if (i == args.Length - 1 && string.Compare(args[args.Length - 1], "//x", true, CultureInfo.InvariantCulture) == 0)
							{
								options.startDebugger = true;
								options.DBG = true;
							}
							else
								appArgs.Add(args[i]);
						}
					}
					scriptArgs = (string[])appArgs.ToArray(typeof(string));

					//searchDirs[0] is the script file directory. Set it only after 
					//the script file resolved because it can be:
					//	CurrentDir 
					//	dir defined by the absolute/ralative script file path 
					//	%CSSCRIPT_DIR%
					//	ExtraLibDirectory
					//  CacheDir
					options.searchDirs = new string[]{	"",
														Environment.ExpandEnvironmentVariables(@"%CSSCRIPT_DIR%\lib"),
														settings == null ? "" : settings.ExpandExtraLibDirectory(),
														""};
					
					options.scriptFileName = FileParser.ResolveFile(options.scriptFileName, options.searchDirs);
					

					if (primaryScript != null)
						options.scriptFileNamePrimary = primaryScript;
					else
						options.scriptFileNamePrimary = options.scriptFileName;

					if (CSExecutor.ScriptCacheDir == "")
						CSExecutor.SetScriptCacheDir(options.scriptFileName); 

					options.searchDirs[0] = Path.GetDirectoryName(Path.GetFullPath(options.scriptFileName)); 
					if (settings != null && settings.HideAutoGeneratedFiles != Settings.HideOptions.DoNotHide)
						options.searchDirs[3] = CSExecutor.ScriptCacheDir; 
					
					CSharpParser.CmdScriptInfo[] cmdScripts = new CSharpParser.CmdScriptInfo[0];
					//analyse ThreadingModel to use it whith execution thread 
					if (File.Exists(options.scriptFileName))
					{
						//do quick parsing for pre/post scripts, ThreadingModel and embedded script arguments
						CSharpParser parser = new CSharpParser(options.scriptFileName, true); 
						
						if (parser.ThreadingModel != ApartmentState.Unknown)
							options.apartmentState = parser.ThreadingModel;
							
						cmdScripts = parser.CmdScripts;
						
						if (primaryScript == null)//this is a primary script
						{
							int firstEmbeddedScriptArg = ParseAppArgs(parser.Args);
							if (firstEmbeddedScriptArg != -1)
							{
								for (int i = firstEmbeddedScriptArg; i < parser.Args.Length; i++ )
									appArgs.Add(parser.Args[i]);
							}
							scriptArgs = (string[])appArgs.ToArray(typeof(string));
						}
					}
					#endregion

					ExecuteOptions originalOptions = (ExecuteOptions)options.Clone(); //preserve master script options
					string originalCurrDir = Environment.CurrentDirectory;

					//run prescripts		
					//Note: during the secondary script execution static options will be modified (this is required for 
					//browsing in CSSEnvironment with reflection). So reset it back with originalOptions after the execution is completed
					foreach (CSharpParser.CmdScriptInfo info in cmdScripts)
						if (info.preScript)
						{
							Environment.CurrentDirectory = originalCurrDir;
							info.args[1] = FileParser.ResolveFile(info.args[1], originalOptions.searchDirs);

							CSExecutor exec = new CSExecutor(info.abortOnError, originalOptions);

							if (originalOptions.DBG)
							{
								ArrayList newArgs = new ArrayList();
								newArgs.AddRange(info.args);
								newArgs.Insert(0, "/dbg");
								info.args = (string[])newArgs.ToArray(typeof(string));
							}
							if (info.abortOnError)
								exec.Execute(info.args, printDelg, originalOptions.scriptFileName);
							else
								exec.Execute(info.args, null, originalOptions.scriptFileName);
						}

					options = originalOptions;
					ExecuteOptions.options = originalOptions; //update static members as well
					Environment.CurrentDirectory = originalCurrDir;

					//Run main script
					//We need to start the execution in a new thread as it is the only way 
					//to set desired ApartmentState under .NET 2.0
					Thread newThread = new Thread(new ThreadStart(this.ExecuteImpl));
					newThread.ApartmentState = options.apartmentState;
					newThread.Start();
					newThread.Join();
					if (lastException != null)
						throw new Exception("Script "+options.scriptFileName+" cannot be executed.", lastException);
					
					//run postscripts		
					foreach (CSharpParser.CmdScriptInfo info in cmdScripts)
						if (!info.preScript)
						{
							Environment.CurrentDirectory = originalCurrDir;
							info.args[1] = FileParser.ResolveFile(info.args[1], originalOptions.searchDirs);

							CSExecutor exec = new CSExecutor(info.abortOnError, originalOptions);

							if (originalOptions.DBG)
							{
								ArrayList newArgs = new ArrayList();
								newArgs.AddRange(info.args);
								newArgs.Insert(0, "/dbg");
								info.args = (string[])newArgs.ToArray(typeof(string));
							}
							if (info.abortOnError)
							{
								exec.Rethrow = true;
								exec.Execute(info.args, printDelg, originalOptions.scriptFileName);
							}
							else
								exec.Execute(info.args, null, originalOptions.scriptFileName);
						}
				}
				else 
				{
					ShowHelp();
				}
			}
			catch (Exception e) 
			{
				if (rethrow)
				{
					throw;
				}
				else
				{
					Environment.ExitCode = 1;
					if (options.reportDetailedErrorInfo)
						print(e.ToString());
					else	
						print(e.Message); //Mono friendly
				}
			}
		}

		/// <summary>
		/// Returns custom application config file.
		/// </summary>
		internal string GetCustomAppConfig(string[] args)
		{
			try
			{
				if (args.Length > 0)
				{
					int firstScriptArg = ParseAppArgs(args);
					if (args.Length > firstScriptArg) 
					{
						Settings settings = null;
						if (options.noConfig)	
						{
							if (options.altConfig != "")
								settings = Settings.Load(Path.GetFullPath(options.altConfig)); //read persistent settings from configuration file
						}
						else
							settings = Settings.Load(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "css_config.xml"));

						if (!options.useScriptConfig && settings.DefaultArguments.IndexOf("/sconfig") == -1)
							return "";

						string script = args[firstScriptArg];
						string[] searchDirs = new string[]{	Environment.ExpandEnvironmentVariables(@"%CSSCRIPT_DIR%\lib"),
														settings == null ? "" : settings.ExpandExtraLibDirectory()};

						script = FileParser.ResolveFile(script, searchDirs);
						if (File.Exists(script))
							return script + ".config";
					}
				}
			}
			catch
			{
				//ignore the exception because it will be raised (again) and handeled by the Execute method
			}
			return "";
		}
		/// <summary>
		/// Dummy 'print' to suppress displaying application messages.
		/// </summary>
		static void VoidPrint(string msg)
		{
		}

		/// <summary>
		/// This method implements compiling and execution of the script.  
		/// </summary>
		public Exception lastException;
		/// <summary>
		/// This method implements compiling and execution of the script.  
		/// </summary>
		private void ExecuteImpl()
		{
			try
			{
				if (options.processFile)
				{
					if (options.local)
						Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(options.scriptFileName));

					if (!options.noLogo)
					{
						Console.WriteLine(AppInfo.appLogo);
					}
					
					//compile
					string assemblyFileName = options.useCompiled ? GetAvailableAssembly(options.scriptFileName) : null;

					if (options.useCompiled && options.useSmartCaching)
					{
						if (assemblyFileName != null)
						{
							MetaDataItems depInfo = new MetaDataItems();
							
							if (depInfo.ReadFileStamp(assemblyFileName))
							{
								string dependencyFile = "";
								foreach (MetaDataItems.MetaDataItem item in depInfo.items)
								{
									if (item.assembly)
										dependencyFile = Path.Combine(Path.GetDirectoryName(options.scriptFileName), item.file); //assembly should be in the same directory with the script
									else
										dependencyFile = FileParser.ResolveFile(item.file, options.searchDirs, false);

									FileInfo info = new FileInfo(dependencyFile);
									if (!info.Exists || info.LastWriteTimeUtc != item.date)
									{
										assemblyFileName = null;
										break;
									}
								}
							}
							else
								assemblyFileName = null;
						}
					}

					if (options.forceCompile && assemblyFileName != null)
					{
						File.Delete(assemblyFileName);
						assemblyFileName = null;
					}


					if (options.buildExecutable || !options.useCompiled || (options.useCompiled && assemblyFileName == null) || options.forceCompile)
					{
						try
						{
							assemblyFileName = Compile(options.scriptFileName);
						}
						catch 
						{
							print("Error: Specified file could not be compiled.\n");
							throw;
						}
					}

					//execute
					if (!options.supressExecution) 
					{
						try 
						{
							if (options.startDebugger)
							{
								System.Diagnostics.Debugger.Launch();
								if (System.Diagnostics.Debugger.IsAttached)
									System.Diagnostics.Debugger.Break();
							}	
							if (options.useCompiled || options.cleanupShellCommand != "")
							{
								//despite the name of the class the execution will be in the current domain
								RemoteExecutor executor = new RemoteExecutor(options.searchDirs); 
								executor.ExecuteAssembly(assemblyFileName, scriptArgs);
							}
							else 
							{
								ExecuteAssembly(assemblyFileName);
							}
						}
						catch 
						{
							print("Error: Specified file could not be executed.\n");
							throw;
						}	

						//cleanup
						if (File.Exists(assemblyFileName) && !options.useCompiled && options.cleanupShellCommand == "") 
						{
							try
							{
								File.Delete(assemblyFileName);
							}
							catch { }
						}

						if (options.cleanupShellCommand != "")
						{
							string counterFile = Path.Combine(GetScriptTempDir(), "counter.txt");
							int prevRuns = 0;
							try
							{
								using (StreamReader sr = new StreamReader(counterFile))
								{
									prevRuns = int.Parse(sr.ReadToEnd());
								}
							}
							catch { }
				
							if (prevRuns > options.doCleanupAfterNumberOfRuns)
						   	{
								prevRuns = 1;
								string[] cmd = options.ExtractShellCommand(options.cleanupShellCommand);
								if (cmd.Length > 1)
									Process.Start(cmd[0], cmd[1]);
								else
									Process.Start(cmd[0]);
							}
							else
								prevRuns++;

							try
							{
								using (StreamWriter sw = new StreamWriter(counterFile))
									sw.Write(prevRuns);
							}
							catch { }
						}
					}
				}
			}
			catch (Exception e) 
			{
				if (rethrow)
				{
					lastException = e;
				}
				else
				{
					Environment.ExitCode = 1;
					if (options.reportDetailedErrorInfo)
						print(e.ToString());
					else	
						print(e.Message); //Mono friendly
				}
			}
		}
		
		/// <summary>
		/// Compiles C# script file into assembly.
		/// </summary>
		public string Compile(string scriptFile, string assemblyFile, bool debugBuild) 
		{
			if (assemblyFile != null)
				options.forceOutputAssembly = assemblyFile;
			else
				options.forceOutputAssembly = PathGetTempFileName();
			if (debugBuild)
				options.DBG = true;
			return Compile(scriptFile);
		}
		
		/// <summary>
		/// More relaible version of the Path.GetTempFileName(). 
		/// It is required because it was some reports about non unique names returned by Path.GetTempFileName()
		/// when running in multi-threaded environment.
		/// </summary>
		/// <returns>Temporary file name.</returns>
		string PathGetTempFileName()
		{
			return Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
		}
		#endregion 

		#region Class nested declarations...
		/// <summary>
		/// Application specific settings
		/// </summary>
		internal class ExecuteOptions : ICloneable
		{
			public static ExecuteOptions options;
			public ExecuteOptions()
			{
				options = this;
			}
			public object Clone()
			{
				ExecuteOptions clone = new ExecuteOptions();
				clone.processFile = this.processFile;
				clone.scriptFileName = this.scriptFileName;
				clone.noLogo = this.noLogo;
				clone.useCompiled = this.useCompiled;
				clone.useSmartCaching = this.useSmartCaching;
				clone.DLLExtension = this.DLLExtension;
				clone.forceCompile = this.forceCompile;
				clone.supressExecution = this.supressExecution;
				clone.DBG = this.DBG;
				clone.startDebugger = this.startDebugger;
				clone.local = this.local;
				clone.buildExecutable = this.buildExecutable;
				clone.refAssemblies = (string[])new ArrayList(this.refAssemblies).ToArray(typeof(string));
				clone.searchDirs = (string[])new ArrayList(this.searchDirs).ToArray(typeof(string));
				clone.buildWinExecutable = this.buildWinExecutable;
				clone.altCompiler = this.altCompiler;
				clone.compilerOptions = this.compilerOptions;
				clone.reportDetailedErrorInfo = this.reportDetailedErrorInfo;
				clone.hideCompilerWarnings = this.hideCompilerWarnings;
				clone.apartmentState = this.apartmentState;
				clone.forceOutputAssembly = this.forceOutputAssembly;
				clone.cleanupShellCommand = this.cleanupShellCommand;
				clone.noConfig = this.noConfig;
				clone.altConfig = this.altConfig;
				clone.hideTemp = this.hideTemp;
				clone.useScriptConfig = this.useScriptConfig;
				clone.scriptFileNamePrimary = this.scriptFileNamePrimary;
				clone.doCleanupAfterNumberOfRuns = this.doCleanupAfterNumberOfRuns;
				return clone; 
			}
			public object Derive()
			{
				ExecuteOptions clone = new ExecuteOptions();
				clone.processFile = this.processFile;
				//clone.scriptFileName = this.scriptFileName;
				//clone.noLogo = this.noLogo;
				//clone.useCompiled = this.useCompiled;
				clone.useSmartCaching = this.useSmartCaching;
				//clone.DLLExtension = this.DLLExtension;
				//clone.forceCompile = this.forceCompile;
				clone.supressExecution = this.supressExecution;
				clone.DBG = this.DBG;
				clone.local = this.local;
				clone.buildExecutable = this.buildExecutable;
				clone.refAssemblies = (string[])new ArrayList(this.refAssemblies).ToArray(typeof(string));
				clone.searchDirs = (string[])new ArrayList(this.searchDirs).ToArray(typeof(string));
				clone.buildWinExecutable = this.buildWinExecutable;
				clone.altCompiler = this.altCompiler;
				clone.compilerOptions = this.compilerOptions;
				clone.reportDetailedErrorInfo = this.reportDetailedErrorInfo;
				clone.hideCompilerWarnings = this.hideCompilerWarnings;
				clone.apartmentState = this.apartmentState;
				clone.forceOutputAssembly = this.forceOutputAssembly;
				clone.cleanupShellCommand = this.cleanupShellCommand;
				clone.noConfig = this.noConfig;
				clone.altConfig = this.altConfig;
				clone.hideTemp = this.hideTemp;
				clone.scriptFileNamePrimary = this.scriptFileNamePrimary;
				clone.doCleanupAfterNumberOfRuns = this.doCleanupAfterNumberOfRuns;
				return clone; 
			}
			public bool processFile = true;
			public string scriptFileName = "";
			public string scriptFileNamePrimary = null;
			public bool noLogo = false;
			public bool useCompiled = false;
			public bool useScriptConfig = false;
			public bool useSmartCaching = true; //hardcoded true but can be set from config file in the future
			public bool DLLExtension = false;
			public bool forceCompile = false;
			public bool supressExecution = false;
			public bool DBG = false;
			public bool startDebugger = false;
			public bool local = false;
			public bool buildExecutable = false;
			public string[] refAssemblies = new string[0];
			public string[] searchDirs =  new string[0];
			public bool buildWinExecutable = false;
			public string altCompiler = "";
			public bool reportDetailedErrorInfo = false;
			public bool hideCompilerWarnings = false;
			public ApartmentState apartmentState = ApartmentState.STA;
			public string forceOutputAssembly = "";
			public string cleanupShellCommand = "";
			public bool noConfig = false;
			public string compilerOptions = "";
			public string altConfig = "";
			public Settings.HideOptions hideTemp = Settings.HideOptions.HideMostFiles;
			public uint doCleanupAfterNumberOfRuns = 20;
			public string[] ExtractShellCommand(string command)
			{
				int pos = command.IndexOf("\"");
				string endToken = "\"";
				if (pos == -1 || pos != 0) //no quatation marks 
						endToken = " ";

				pos = command.IndexOf(endToken, pos+1);
				if (pos == -1)
						return new string[] { command };
				else
					return new string[] { command.Substring(0, pos).Replace("\"", ""),  command.Substring(pos+1).Trim()};
			}
		}
		#endregion 

		#region Class data...
		/// <summary>
		/// C# Script arguments array (sub array of application arguments array).
		/// </summary>
		string[] scriptArgs;
		/// <summary>
		/// Callback to print application messages to appropriate output.
		/// </summary>
		static PrintDelegate print;
		/// <summary>
		/// Container for paresed command line parguments
		/// </summary>
		static internal ExecuteOptions options;
		/// <summary>
		/// Flag to force to rethrow critical exceptions
		/// </summary>
		bool rethrow;
		#endregion 

		#region Class methods...
		/// <summary>
		/// Constructor
		/// </summary>
		public CSExecutor()
		{
			rethrow = false;
			options = new ExecuteOptions();
		}
		/// <summary>
		/// Constructor
		/// </summary>
		public CSExecutor(bool rethrow, ExecuteOptions optionsBase)
		{
			this.rethrow = rethrow;
			options = new ExecuteOptions();
			//force to read all relative options data from the config file
			options.noConfig = optionsBase.noConfig;
			options.altConfig = optionsBase.altConfig;
		}
		/// <summary>
		/// Checks/returns if compiled C# script file (ScriptName + "c") available and valid.
		/// </summary>
		private string GetAvailableAssembly(string scripFileName) 
		{
			string retval = null;
			string asmFileName = options.hideTemp != Settings.HideOptions.DoNotHide ? Path.Combine( CSExecutor.ScriptCacheDir, Path.GetFileName(scripFileName) + "c") : scripFileName + "c";
			if (File.Exists(asmFileName) && File.Exists(scripFileName))
			{
				FileInfo scriptFile = new FileInfo(scripFileName);
				FileInfo asmFile = new FileInfo(asmFileName);
				
				if( asmFile.LastWriteTime == scriptFile.LastWriteTime && 
					asmFile.LastWriteTimeUtc == scriptFile.LastWriteTimeUtc)
				{
					retval = asmFileName;
				}
			}
			return retval;
		}
		/// <summary>
		/// Compiles C# script file.
		/// </summary>
		private string Compile(string scriptFileName) 
		{
			bool generateExe = options.buildExecutable;
			string scriptDir = Path.GetDirectoryName(scriptFileName);
			string assemblyFileName = "";
			string tempDir = Path.GetDirectoryName(GetScriptTempFile());
			
			//options may be uninitialized in case we are compileng from CSScriptLibrary
			if (options.searchDirs.Length == 0)
				options.searchDirs = new string[]{scriptDir};

			//parse source file in order to find all referenced assemblies
			//ASSUMPTION: assembly name is the same as namespace + ".dll"
			//if script doesn't follow this assumption user will need to 
			//specify assemblies explicitly  

			ScriptParser parser = new ScriptParser(scriptFileName, options.searchDirs);

			ICodeCompiler compiler;
			if (options.altCompiler == "")
			{
				compiler = (new CSharpCodeProvider()).CreateCompiler();
			}
			else
			{
				try
				{
					Assembly asm = Assembly.LoadFrom(Path.IsPathRooted(options.altCompiler) ? options.altCompiler : Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), options.altCompiler));
					Type[] types = asm.GetModules()[0].FindTypes(Module.FilterTypeName, "CSSCodeProvider");
					MethodInfo method = types[0].GetMethod("CreateCompiler");
					compiler = (ICodeCompiler)method.Invoke(null, new object[]{scriptFileName});
				}
				catch (Exception ex)
				{
					throw new Exception("Cannot use alternative compiler", ex); 
				}
			}
		
			CompilerParameters compileParams = new CompilerParameters();
		 	
			if (options.DBG)
				compileParams.CompilerOptions = "/d:DEBUG /d:TRACE ";

			if (options.compilerOptions != string.Empty)
				compileParams.CompilerOptions += options.compilerOptions;

			compileParams.IncludeDebugInformation = options.DBG;
			compileParams.GenerateExecutable = generateExe;
			compileParams.GenerateInMemory = !generateExe;
			
			ArrayList refAssemblies = new ArrayList();

			//add assemblies were referenced from command line
			foreach (string asmName in options.refAssemblies)
				compileParams.ReferencedAssemblies.AddRange(AssemblyResolver.FindAssembly(asmName, options.searchDirs));
		
			AssemblyResolver.ignoreFileName = Path.GetFileNameWithoutExtension(scriptFileName) + ".dll";
			
			//add local and global assemblies (if found) that have the same assembly name as a namespace
			foreach (string nmSpace in parser.ReferencedNamespaces) 
				compileParams.ReferencedAssemblies.AddRange(AssemblyResolver.FindAssembly(nmSpace, options.searchDirs));
			
			//add assemblies referenced from code
			foreach (string asmName in parser.ReferencedAssemblies)
				if (asmName.StartsWith("\"") && asmName.EndsWith("\"")) 
					compileParams.ReferencedAssemblies.Add(asmName.Replace("\"", "")); //not-searchable assemblies
				else
					compileParams.ReferencedAssemblies.AddRange(AssemblyResolver.FindAssembly(asmName, options.searchDirs));
			
			//add resources referenced from code
			foreach (string resFile in parser.ReferencedResources)
			{
				string file = null;
				foreach (string dir in options.searchDirs)
				{
					file = Path.IsPathRooted(resFile) ? Path.GetFullPath(resFile) : Path.Combine(dir, resFile);
					if (File.Exists(file))
						break;
				}

				if (file == null)
					file = resFile;
				
				compileParams.CompilerOptions += "\"/res:" + file + "\" "; //eg. /res:C:\\Scripting.Form1.resources";
			}
				

			if (options.forceOutputAssembly != "")
			{
				assemblyFileName = options.forceOutputAssembly;
			}
			else
			{
				if (generateExe)
					assemblyFileName = Path.Combine(scriptDir, Path.GetFileNameWithoutExtension(scriptFileName) + ".exe");
				else if (options.useCompiled || options.DLLExtension)
				{
					if (options.DLLExtension)
						assemblyFileName = Path.Combine(scriptDir, Path.GetFileNameWithoutExtension(scriptFileName) + ".dll");
					else if (options.hideTemp != Settings.HideOptions.DoNotHide)
						assemblyFileName = Path.Combine(CSExecutor.ScriptCacheDir,  Path.GetFileName(scriptFileName) + "c");
					else
						assemblyFileName = scriptFileName + "c";
				}
				else
				{
					string tempFile = Path.GetTempFileName();
					if (File.Exists(tempFile))
						File.Delete(tempFile);
					assemblyFileName = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(tempFile)+".dll");
				}
			}
			
			if (generateExe && options.buildWinExecutable)
				compileParams.CompilerOptions += "/target:winexe ";

			if (File.Exists(assemblyFileName))
				File.Delete(assemblyFileName);

			compileParams.OutputAssembly = assemblyFileName;

			CompilerResults results;
			if (generateExe)
			{
				results = compiler.CompileAssemblyFromFileBatch(compileParams, parser.FilesToCompile);
			}
			else
			{
				
				string originalExtension = Path.GetExtension(compileParams.OutputAssembly);
				if (originalExtension != ".dll")
				{
					//Despite the usage of .dll file name is not required for MS C# compiler we need to do this because 
					//some compilers (Mono, VB) accept only dll or exe file extensions.
					compileParams.OutputAssembly = Path.ChangeExtension(compileParams.OutputAssembly, ".dll");

					if (File.Exists(compileParams.OutputAssembly))
						File.Delete(compileParams.OutputAssembly);
					results = compiler.CompileAssemblyFromFileBatch(compileParams, parser.FilesToCompile);

					if (File.Exists(compileParams.OutputAssembly))
					{
						while (true)
						{
							//There were reports of MS C# compiler (csc.exe) notr releasing OutputAssembly file
							//after compilation finished. Thus wait a little...
							try
							{
								using (File.Open(compileParams.OutputAssembly, FileMode.Open, FileAccess.Read, FileShare.None))
									break;
							}
							catch
							{
								Thread.Sleep(100);
							}
						}
						File.Move(compileParams.OutputAssembly, Path.ChangeExtension(compileParams.OutputAssembly, originalExtension));
					}
				}
				else
				{
					if (File.Exists(compileParams.OutputAssembly))
						File.Delete(compileParams.OutputAssembly);
					results = compiler.CompileAssemblyFromFileBatch(compileParams, parser.FilesToCompile);
				}
			}

			if (results.Errors.Count != 0) 
			{
				StringBuilder compileErr = new StringBuilder();
				foreach (CompilerError err in results.Errors) 
				{
					if (err.IsWarning && options.hideCompilerWarnings)
						continue;
					
					compileErr.Append(err.ToString());
					compileErr.Append(Environment.NewLine);
					compileErr.Append(Environment.NewLine);
				}
				throw new Exception(compileErr.ToString());				
			}
			else
			{
				if (!options.DBG) //.pdb and imported files might be needed for the debugger
				{
					parser.DeleteImportedFiles();
					string pdbFile = Path.Combine(Path.GetDirectoryName(assemblyFileName), Path.GetFileNameWithoutExtension(assemblyFileName)+".pdb");	
					if (File.Exists(pdbFile))
						File.Delete(pdbFile);
				}

				if (options.useCompiled)
				{
					if (options.useSmartCaching)
					{
						MetaDataItems depInfo = new MetaDataItems();

						//save referenced local assemblies info
						foreach(string assembly in compileParams.ReferencedAssemblies)
						{
							foreach(string dir in options.searchDirs)
								if (string.Compare(dir, Path.GetDirectoryName(assembly), true) == 0)
								{
									FileInfo assemblyInfo = new FileInfo(assembly);
									depInfo.AddItem(Path.GetFileName(assembly), assemblyInfo.LastWriteTimeUtc, true);
									break;
								}
						}
						//save imported scripts info
						foreach(string script in parser.ImportedFiles)
						{
							FileInfo scriptInfo = new FileInfo(script);
							
							bool local = false;
							foreach (string dir in options.searchDirs)
								if ((local = (string.Compare(dir, Path.GetDirectoryName(scriptInfo.FullName), true) == 0)))									break;
							
							if (local)
								depInfo.AddItem(Path.GetFileName(script), scriptInfo.LastWriteTimeUtc, false);
							else 
								depInfo.AddItem(script, scriptInfo.LastWriteTimeUtc, false);

						}

						depInfo.StampFile(assemblyFileName);		
					}

					FileInfo scriptFile = new FileInfo(scriptFileName);
					FileInfo asmFile = new FileInfo(assemblyFileName);
					
					if (scriptFile!= null && asmFile != null)
					{
						//Console.WriteLine("Update LastWriteTime"+assemblyFileName); //temp os
						asmFile.LastWriteTime = scriptFile.LastWriteTime; 
						asmFile.LastWriteTimeUtc = scriptFile.LastWriteTimeUtc;
					}
				}
			}
			return assemblyFileName;
		}
		
		/// <summary>
		/// Executes compiled C# script file.
		/// Invokes static method 'Main' from the assembly.
		/// </summary>
		private void ExecuteAssembly(string assemblyFile) 
		{
			//execute assembly in a different domain to make it possible to unload assembly before clean up
			AssemblyExecutor executor = new AssemblyExecutor(assemblyFile, "AsmExecution");
			executor.Execute(scriptArgs);
		} 

		/// <summary>
		/// Returns the name of the temporary file in the CSSCRIPT subfolder of Path.GetTempPath().
		/// </summary>
		/// <returns>Temporary file name.</returns>
		static public string GetScriptTempFile() 
		{
			string file = Path.GetTempFileName();
			if (File.Exists(file))
				File.Delete(file);
			return Path.Combine(GetScriptTempDir(), Path.GetFileName(file));
		}
		/// <summary>
		/// Returns the name of the temporary folder in the CSSCRIPT subfolder of Path.GetTempPath().
		/// </summary>
		/// <returns>Temporary directoiry name.</returns>
		static public string GetScriptTempDir()
		{
			string dir = Path.Combine(Path.GetTempPath(), "CSSCRIPT");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			return dir;
		} 
		
		///<summary>
		/// Contains the name of the temporary cache folder in the CSSCRIPT subfolder of Path.GetTempPath(). The cache folder is specifig for every script file.
		/// </summary>
		static public string ScriptCacheDir
		{
			get
			{
				return cacheDir;
			}
		}
		/// <summary>
		/// Generates the name of the temporary cache folder in the CSSCRIPT subfolder of Path.GetTempPath(). The cache folder is specifig for every script file.
		/// </summary>
		/// <param name="scriptFile">script file</param>
		static public void SetScriptCacheDir(string scriptFile)
		{
			string dir = Path.Combine(GetScriptTempDir(), @"Cache\"+Path.GetFullPath(scriptFile).ToLower().GetHashCode().ToString());
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
				using (StreamWriter sw = new StreamWriter(Path.Combine(dir, "css_info.txt")))
					sw.Write(Environment.Version.ToString()+"\n"+Path.GetDirectoryName(Path.GetFullPath(scriptFile))+"\n");
			}
			//Console.WriteLine("SetScriptCacheDir: "+scriptFile + " : " + dir); //temp os
			cacheDir = dir;
		}
		private static string cacheDir = "";

		/// <summary>
		/// Prints Help info.
		/// </summary>
		private void ShowHelp()
		{
			StringBuilder msgBuilder = new StringBuilder();
			msgBuilder.Append(AppInfo.appLogo);
			msgBuilder.Append("\nUsage: "+AppInfo.appName+" <switch 1> <switch 2> <file> [params] [//x]\n");
			msgBuilder.Append("\n");
			msgBuilder.Append("<switch 1>\n");
			msgBuilder.Append(" /?    - 获取帮助信息\n");
            msgBuilder.Append(" /e    - 将脚本编译为控制台程序\n");
            msgBuilder.Append(" /ew   - 将脚本编译为Windows窗口程序\n");
            msgBuilder.Append(" /c    - 使用预编译文件以提升脚本执行的性能\n");
			msgBuilder.Append(" /ca   - 将脚本文件编译为预编译文件(.csc)但不执行它脚本程序\n");
            msgBuilder.Append(" /cd   - 将脚本编译为程序集(.dll)但不执行\n\n");
			msgBuilder.Append(" /co:<options>\n");
			msgBuilder.Append("       - 直接传递编译器参数给脚本语言编译程序\n");
			msgBuilder.Append("         例如 : /co:/d:TRACE pass /d:TRACE option to C# compiler \n\n");
            msgBuilder.Append(" /s    - 生成脚本框架到指定文件，(例如 : " + AppInfo.appName + " /s > Example.cs)\n");
			msgBuilder.Append("\n");
			msgBuilder.Append("<switch 2>\n");
			if (AppInfo.appParamsHelp != "")
				msgBuilder.Append(" /" + AppInfo.appParamsHelp);	//application specific usage info
			msgBuilder.Append(" /dbg | /d\n");
            msgBuilder.Append("       - 强制编译器添加调试信息\n");
            msgBuilder.Append(" /l    - 将脚本所在文件夹设置为当前路径\n");
            msgBuilder.Append(" /noconfig[:<file>]\n       - 不使用默认配置文件或为其指定一个配置文件\n");
            msgBuilder.Append("         例如 : " + AppInfo.appName + " /noconfig Example.cs\n");
            msgBuilder.Append("                " + AppInfo.appName + " /noconfig:c:\\cs-script\\css_VB.dat Example.vb)\n");
			msgBuilder.Append(" /sconfig\n       - 在编译/执行时为脚本程序指定一个配置文件 (例如 : Script.cs.config)\n");
            //msgBuilder.Append("This option might be usefull fo running scripts, which usually cannot be executed without configuration file (eg. WCF, remoting).\n\n");
			msgBuilder.Append(" /r:<assembly 1>:<assembly N>\n");
            msgBuilder.Append("       - 当脚本引擎不能解析某命名空间的情况时显式引用程序集\n");
			msgBuilder.Append("         例如 : " + AppInfo.appName + " /r:myLib.dll myScript.cs\n");
			msgBuilder.Append("\n");
            msgBuilder.Append("file   - 将执行的脚本文件\n");
            msgBuilder.Append("params - 为将执行的脚本文件指定的参数信息\n");
            msgBuilder.Append(" //x   - 执行脚本时启动调试器\n");
			msgBuilder.Append("\n");
			if (AppInfo.appConsole) // a temporary hack to prevent showing a huge message box when not in console mode
			{
                //msgBuilder.Append("\n");
                //msgBuilder.Append("****************************************\n");
                //msgBuilder.Append("脚本语法\n");
                //msgBuilder.Append("****************************************\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("脚本引擎预编译器指令：\n\n"); // Engine directives:
				msgBuilder.Append("--------------------\n\n");
				msgBuilder.Append("//css_import <file>[, preserve_main][, rename_namespace(<oldName>, <newName>)];\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("Aliase          - //css_imp\n");
				msgBuilder.Append("    为了让C/C++程序员使用方便也可以使用另外的两个指令来引入命名空间， //css_include 和 //css_inc. 它们的作用与 //css_import <file> 相同。\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("file            - 在编译时需要引用的脚本文件\n"); // name of a script file to be imported at compile-time.
				msgBuilder.Append("<preserve_main> - 指示预处理器不重命名脚本的静态入口函数 : 'static Main'\n");
                msgBuilder.Append("oldName         - 在预编译时需要重命名的命名空间\n"); // name of a namespace to be renamed during importing
                msgBuilder.Append("newName         - 在预编译时为需要重命名的命名空间起的新名字\n"); // new name of a namespace to be renamed during importing
				msgBuilder.Append("\n");
                msgBuilder.Append("    这条指令主要用于在编译时需要将某脚本注入到另外的脚本文件中时，从而可以执行其它脚本中的代码，此命令可以多次使用。\n"); // This directive is used to inject one script into another at compile time. Thus code from one script can be exercised in another one.
				//msgBuilder.Append("'Rename' clause can appear in the directive multiple times.\n");	
                msgBuilder.Append("\n--------------------\n\n");
				msgBuilder.Append("//css_args arg0[,arg1]..[,argN];\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("    嵌入的脚本参数指令，此处除\"/noconfig\"参数外都可以使用\n"); // Embedded script arguments. The both script and engine arguments are allowed except \"/noconfig\" engine command switch.
                msgBuilder.Append("    例如 : //css_args /dbg; 这条指令将总是强制脚本引擎总是执行在调试模式下\n"); // This directive will always force script engine to execute the script in debug mode.
                msgBuilder.Append("\n--------------------\n\n");
				msgBuilder.Append("//css_reference <file>;\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("Aliase          - //css_ref\n"); 
				msgBuilder.Append("\n");
                msgBuilder.Append("file            - 指定在运行时需要加载的程序集\n"); // name of the assembly file to be loaded at run-time.
				msgBuilder.Append("\n");
                msgBuilder.Append("    这条指令主要用于需要在运行时引用其它程序集的情况，程序集必须在全局程序集缓存(GAC)、与脚本相同的文件夹或\"Script Library\"文件夹中，详情请参考脚本引擎配置文件。\n"); // This directive is used to reference assemblies required at run time.
                //msgBuilder.Append("The assembly must be in GAC, the same folder with the script file or in the 'Script Library' folders (see 'CS-Script settings').\n");
                msgBuilder.Append("\n--------------------\n\n");
				msgBuilder.Append("//css_resource <file>;\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("Aliase          - //css_res\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("file            - 指定脚本文件需要用到的资源文件\n"); // name of the resource file to be used with the script.
				msgBuilder.Append("\n");
                msgBuilder.Append("    这条指令主要用于为脚本文件引用相关的图片、字符等资源文件。\n"); // This directive is used to reference resourrce file for script.
				msgBuilder.Append("    例如 : //css_res Scripting.Form1.resources;\n");
                msgBuilder.Append("\n--------------------\n\n");
				msgBuilder.Append("//css_prescript file([arg0][,arg1]..[,argN])[ignore];\n");
				msgBuilder.Append("//css_postscript file([arg0][,arg1]..[,argN])[ignore];\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("Aliase          - //css_pre and //css_post\n");
				msgBuilder.Append("\n");
				msgBuilder.Append("file            - 脚本文件(脚本引擎的扩充选项)\n");
				msgBuilder.Append("arg0..N         - 为脚本文件指定一个或多个字符的参数\n");
                msgBuilder.Append("ignore          - 忽略主脚本文件中出现的错误而继续执行\n"); // continue execution of the main script in case of error
				msgBuilder.Append("\n");
                msgBuilder.Append("    这两第指令主要用于执行附加的\"前处理\"或\"后处理\"脚本文件，如果其参数指定为 \"$this\" 将在执行时替换为主要脚本文件名。\n"); // These directives are used to execute secondary pre- and post-action scripts.
                //msgBuilder.Append("If $this is specified as arg0..N it will be replaced at execution time with the main script full name.\n");
                msgBuilder.Append("\n--------------------\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("    以上所有的预编译指令都必须独自占一行以免影响 CLI 正常编译，同时这些指令必须放在任何命名空间或类的定义之前。\n"); // Any directive has to be written as a single line in order to have no impact on compiling by CLI compliant compiler.
                //msgBuilder.Append("It also must be placed before any namespace or class declaration.\n");
				msgBuilder.Append("\n");
                msgBuilder.Append("--------------------\n\n");
				msgBuilder.Append("示例 : \n");
				msgBuilder.Append("\n");
				msgBuilder.Append(" using System;\n");
				msgBuilder.Append(" //css_prescript com(WScript.Shell, swshell.dll);\n");
				msgBuilder.Append(" //css_import tick, rename_namespace(CSScript, TickScript);\n");
				msgBuilder.Append(" //css_reference teechart.lite.dll;\n");
				msgBuilder.Append(" \n");
				msgBuilder.Append(" namespace CSScript\n");
				msgBuilder.Append(" {\n");
				msgBuilder.Append("   class TickImporter\n");
				msgBuilder.Append("   {\n");
				msgBuilder.Append("      static public void Main(string[] args)\n");
				msgBuilder.Append("      {\n");
				msgBuilder.Append("         TickScript.Ticker.i_Main(args);\n");
				msgBuilder.Append("      }\n");
				msgBuilder.Append("   }\n");
				msgBuilder.Append(" }\n");
				msgBuilder.Append("\n");
			}

			print(msgBuilder.ToString());
		} 
		/// <summary>
		/// Show sample C# scrip file.
		/// </summary>
		private void ShowSample()
		{
			StringBuilder msgBuilder = new StringBuilder();
			msgBuilder.Append("using System;\r\n");
			msgBuilder.Append("using System.Windows.Forms;\r\n");
			msgBuilder.Append("\r\n");
			msgBuilder.Append("class Script\r\n");
			msgBuilder.Append("{\r\n");
			msgBuilder.Append("	[STAThread]\r\n");
			msgBuilder.Append("	static public void Main(string[] args)\r\n");
			msgBuilder.Append("	{\r\n");
			msgBuilder.Append("		MessageBox.Show(\"Just a test!\");\r\n");
			msgBuilder.Append("\r\n");
			msgBuilder.Append("		for (int i = 0; i < args.Length; i++)\r\n");
			msgBuilder.Append("		{\r\n");
			msgBuilder.Append("			Console.WriteLine(args[i]);\r\n");
			msgBuilder.Append("		}\r\n");
			msgBuilder.Append("	}\r\n");
			msgBuilder.Append("}\r\n");
			print(msgBuilder.ToString());
		}
		#endregion
	}

	#region MetaDataItems...
	/// <summary>
	/// The MetaDataItems class contains information about script dependencies (referenced local 
	/// assemblies and imported scripts) and copmpiler options. This information is required when scripts are executed 
	/// in 'cached' mode (/c switch). On the base of this information the script engine will 
	/// compile new version of .csc assembly if any of it's dependencies is changed. This is 
	/// required even for referenced local assemblies as it is possible that they are a strongly 
	/// named assemblies (recompiling is required for any compiled client of the assembly in this 
	/// case). 
	/// 
	/// The perfect place to store the dependencies info (custom meta data) is the assembly 
	/// resources. However if we do so such assemblies have to be loaded in order to read their 
	/// resources. It is not acceptable as after loading assembly cannot be unloaded. Also when 
	/// using assembly loading performance is compromised significantly.
	/// 
	/// That is why custom meta data is just physically appended to the file. This is a valid 
	/// approach because such assembly is not to be distributed anywhere but to stay always 
	/// on the PC and play the role of the temporary data for the script engine.
	/// 
	/// Note: .dll compiled and linked in normal way without any custom meta data attached. 
	/// </summary>
	class MetaDataItems
	{
		public class MetaDataItem
		{
			public MetaDataItem(string file, DateTime date, bool assembly)
			{
				this.file = file;
				this.date = date;
				this.assembly = assembly;
			}
			public string file;
			public DateTime date;
			public bool assembly;
		}

		public ArrayList items = new ArrayList();
		public void AddItem(string file, DateTime date, bool assembly)
		{
			this.items.Add(new MetaDataItem(file, date, assembly));
		}
		public bool StampFile(string file)
		{
			try
			{
				using (FileStream fs = new FileStream(file, FileMode.Open))
				{
					fs.Seek(0,SeekOrigin.End);
					using (BinaryWriter w = new BinaryWriter(fs))
					{
						char[] data = this.ToString().ToCharArray();
						w.Write(data);
						w.Write((Int32)data.Length);
						w.Write((Int32)(CSExecutor.options.DBG ? 1 : 0));
						w.Write((Int32)Environment.Version.ToString().GetHashCode());
						w.Write((Int32)stampID);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
			return true;
		}
		public bool ReadFileStamp(string file)
		{
			try
			{
				using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader r = new BinaryReader(fs))
					{
						fs.Seek(-intSize, SeekOrigin.End);
						int stamp = r.ReadInt32();
						if (stamp == stampID)
						{
							fs.Seek(-(intSize*2), SeekOrigin.End);	
							if (r.ReadInt32() != Environment.Version.ToString().GetHashCode())
							{
								//Console.WriteLine("Environment.Version");
								return false;
							}

							fs.Seek(-(intSize*3), SeekOrigin.End);
							if (r.ReadInt32() != (CSExecutor.options.DBG ? 1 : 0))
							{
								//Console.WriteLine("CSExecutor.options.DBG");
								return false;
							}

							fs.Seek(-(intSize*4), SeekOrigin.End);	
							int dataSize = r.ReadInt32();

							if (dataSize != 0)
							{
								fs.Seek(-(intSize*4+dataSize), SeekOrigin.End);	
								return this.Parse(new string(r.ReadChars(dataSize)));
							}
							else
								return true;
						}
						return false;
					}
				}
			}
			catch 
			{
			}
			return false;
		}
		private new string ToString()
		{
			StringBuilder bs = new StringBuilder();
			foreach (MetaDataItem fileInfo in items)
			{
				bs.Append(fileInfo.file);
				bs.Append(";");
				bs.Append(fileInfo.date.ToFileTimeUtc().ToString());
				bs.Append(";");
				bs.Append(fileInfo.assembly ? "Y" : "N");
				bs.Append("|");
			}
			return bs.ToString();
		}
		private bool Parse(string data)
		{
			foreach (string itemData in data.Split("|".ToCharArray()))
			{
				if (itemData.Length > 0)
				{
					string[] parts = itemData.Split(";".ToCharArray());
					if (parts.Length == 3)
					{
						this.items.Add(new MetaDataItem(parts[0], DateTime.FromFileTimeUtc(long.Parse(parts[1])), parts[2] == "Y"));
					}
					else
						return false;
				}
			}
			return true;
		}
		private int stampID = Assembly.GetExecutingAssembly().FullName.Split(",".ToCharArray())[1].GetHashCode();
		private int intSize = Marshal.SizeOf((Int32)0);
	}
	#endregion
}			
