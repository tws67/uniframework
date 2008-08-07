#region Licence...
//-----------------------------------------------------------------------------
// Date:	10/11/04	Time: 3:00p
// Module:	CSScriptLib.cs
// Classes:	CSScript
//			AppInfo
//
// This module contains the definition of the CSScript class. Which implements 
// compiling C# script engine (CSExecutor). Can be used for hosting C# script engine
// from any CLR application
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2004. All rights reserved.
//
// Redistribution and use of this code in source and binary forms,  without 
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
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Configuration;
using System.Collections.Specialized;
using csscript;

namespace CSScriptLibrary
{
	/// <summary>
	/// Delegate to handle output from script
	/// </summary>
	public delegate void PrintDelegate(string msg);
	/// <summary>
	/// Class which is implements CS-Script class library interface.
	/// </summary>
	public class CSScript
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public CSScript()
		{
			rethrow = false;
		}
		/// <summary>
		/// Force caught exceptions to be rethrown.
		/// </summary>
		static public bool Rethrow
		{
			get {return rethrow;}
			set {rethrow = value;}
		}
		/// <summary>
		/// Invokes CSExecutor (C# script engine)
		/// </summary>
		/// <param name="print">Print delegate to be used (if not null) to handle script engine output (eg. compilation errors).</param>
		/// <param name="args">Script arguments.</param>
		static public void Execute(CSScriptLibrary.PrintDelegate print, string[] args)
		{
			csscript.AppInfo.appName = new FileInfo(Application.ExecutablePath).Name;
			csscript.CSExecutor exec = new csscript.CSExecutor();
			exec.Rethrow = Rethrow;
			exec.Execute(args, new csscript.PrintDelegate(print != null ? print : new CSScriptLibrary.PrintDelegate(DefaultPrint)), null);
		}
		/// <summary>
		/// Invokes CSExecutor (C# script engine)
		/// </summary>
		/// <param name="print">Print delegate to be used (if not null) to handle script engine output (eg. compilation errors).</param>
		/// <param name="args">Script arguments.</param>
		/// <param name="rethrow">Flag, which indicated if script exceptions should be rethrowed by the script engine without any handling.</param>
		public void Execute(CSScriptLibrary.PrintDelegate print, string[] args, bool rethrow)
		{
			csscript.AppInfo.appName = new FileInfo(Application.ExecutablePath).Name;
			csscript.CSExecutor exec = new csscript.CSExecutor();
			exec.Rethrow = rethrow;
			exec.Execute(args, new csscript.PrintDelegate(print != null ? print : new CSScriptLibrary.PrintDelegate(DefaultPrint)), null);
		}
		/// <summary>
		/// Compiles script code into assembly with CSExecutor
		/// </summary>
		/// <param name="scriptText">The script code to be compiled.</param>
		/// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
		/// <param name="debugBuild">'true' if debug information should be included in assembly; otherwise, 'false'.</param>
		/// <returns>Compiled assembly file name.</returns>
		static public string CompileCode(string scriptText, string assemblyFile, bool debugBuild)
		{
			string tempFile = Path.GetTempFileName();
			try
			{
				using (StreamWriter sw = new StreamWriter(tempFile)) 
				{
					sw.Write(scriptText); 
				}
				return Compile(tempFile, assemblyFile, debugBuild);
			}
			finally
			{
				File.Delete(tempFile);
			}
		}
		/// <summary>
		/// Compiles script file into assembly with CSExecutor
		/// </summary>
		/// <param name="scriptFile">The name of script file to be compiled.</param>
		/// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
		/// <param name="debugBuild">'true' if debug information should be included in assembly; otherwise, 'false'.</param>
		/// <returns>Compiled assembly file name.</returns>
		static public string Compile(string scriptFile, string assemblyFile, bool debugBuild)
		{
			csscript.CSExecutor exec = new csscript.CSExecutor();
			exec.Rethrow = true;
			return exec.Compile(scriptFile, assemblyFile, debugBuild);
		}
		/// <summary>
		/// Compiles script file into assembly with CSExecutor
		/// </summary>
		/// <param name="scriptFile">The name of script file to be compiled.</param>
		/// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
		/// <param name="debugBuild">'true' if debug information should be included in assembly; otherwise, 'false'.</param>
		/// <param name="cssConfigFile">The name of CS-Script configuration file. If null the default config file will be used (appDir/css_config.xml).</param>
		/// <returns>Compiled assembly file name.</returns>
		static public string CompileWithConfig(string scriptFile, string assemblyFile, bool debugBuild, string cssConfigFile)
		{
			return CompileWithConfig(scriptFile, assemblyFile, debugBuild, cssConfigFile, null);
		}
		/// <summary>
		/// Compiles script file into assembly with CSExecutor
		/// </summary>
		/// <param name="scriptFile">The name of script file to be compiled.</param>
		/// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
		/// <param name="debugBuild">'true' if debug information should be included in assembly; otherwise, 'false'.</param>
		/// <param name="cssConfigFile">The name of CS-Script configuration file. If null the default config file will be used (appDir/css_config.xml).</param>
		/// <param name="compilerOptions">The string value to be passed directly to the language compiler.</param>
		/// <returns>Compiled assembly file name.</returns>
		static public string CompileWithConfig(string scriptFile, string assemblyFile, bool debugBuild, string cssConfigFile, string compilerOptions)
		{
			Settings settings = Settings.Load(cssConfigFile != null ? cssConfigFile : Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "css_config.xml"));
			
			if (settings == null)
				throw new ApplicationException("The configuration file \""+cssConfigFile+"\" cannot be found");
			
			CSExecutor exec = new csscript.CSExecutor();
			exec.Rethrow = true;

			CSExecutor.options.altCompiler = settings.ExpandUseAlternativeCompiler();
			CSExecutor.options.compilerOptions = compilerOptions != null ? compilerOptions : "";
			CSExecutor.options.apartmentState = settings.DefaultApartmentState;
			CSExecutor.options.reportDetailedErrorInfo = settings.ReportDetailedErrorInfo;
			CSExecutor.options.cleanupShellCommand = settings.CleanupShellCommand;
			CSExecutor.options.doCleanupAfterNumberOfRuns = settings.DoCleanupAfterNumberOfRuns;
			
			CSExecutor.options.searchDirs = new string[]	
												{	
													Path.GetDirectoryName(scriptFile),
													settings == null ? "" : settings.ExpandExtraLibDirectory(),
													Environment.GetEnvironmentVariable("CSSCRIPT_DIR") == null ? "" : Environment.ExpandEnvironmentVariables(@"%CSSCRIPT_DIR%\lib"),
												}; 

			return exec.Compile(scriptFile, assemblyFile, debugBuild);
		}
		/// <summary>
		/// Compiles script code into assembly with CSExecutor and loads it in current AppDomain.
		/// </summary>
		/// <param name="scriptText">The script code to be compiled.</param>
		/// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
		/// <param name="debugBuild">'true' if debug information should be included in assembly; otherwise, 'false'.</param>
		/// <returns>Compiled assembly.</returns>
		static public Assembly LoadCode(string scriptText, string assemblyFile, bool debugBuild)
		{
			string tempFile = Path.GetTempFileName();
			try
			{
				using (StreamWriter sw = new StreamWriter(tempFile)) 
				{
					sw.Write(scriptText); 
				}
				return Load(tempFile, assemblyFile, debugBuild);
			}
			finally
			{
				File.Delete(tempFile);
			}
		}
		/// <summary>
		/// Compiles script file into assembly with CSExecutor and loads it in current AppDomain
		/// </summary>
		/// <param name="scriptFile">The name of script file to be compiled.</param>
		/// <param name="assemblyFile">The name of compiled assembly. If set to null a temnporary file name will be used.</param>
		/// <param name="debugBuild">'true' if debug information should be included in assembly; otherwise, 'false'.</param>
		/// <returns>Compiled assembly.</returns>
		static public Assembly Load(string scriptFile, string assemblyFile, bool debugBuild)
		{
			csscript.CSExecutor exec = new csscript.CSExecutor();
			exec.Rethrow = true;
			string outputFile = exec.Compile(scriptFile, assemblyFile, debugBuild);

			AssemblyName asmName = AssemblyName.GetAssemblyName(outputFile);
			return AppDomain.CurrentDomain.Load(asmName);
		}
		/// <summary>
		/// Default implementation of displaying application messages.
		/// </summary>
		static void DefaultPrint(string msg)
		{
			//do nothing
		}
		static bool rethrow;
	}
}

namespace csscript
{
	/* 
	 This code is copied from the article "Custom app.config" By Brian ONeil 
	 http://www.codeproject.com/csharp/customconfig.asp
	 It is just been reformated but the algorithm is not changed. 
	 */
	
	
	/// <summary>
	/// This class implements access to the app.config file  associated with the script file (eg. hello.cs.config). 
	/// Unfortunately System.Configuration.ConfigurationSettings.AppSettings cannot be used to access [script].config file.
	/// This is because ConfigurationSettings.AppSettings is biound to the [application].config file,  which is in fact shared by all scripts as they share the same applicatipn (eg. cscs.exe).
	/// The CSSEnvironment allows access the [script].config file in the similar manner as the [application].config.
	/// </summary>
	public class CSSEnvironment
	{
		/// <summary>
		/// The full name of the [script].config file containg the script configuration data.
		/// </summary>
		static public string ConfigFile
		{
			get
			{
				if (forceConfigFile == null && ScriptFile != null)
					return ScriptFile + ".config";
				else
					return forceConfigFile;
			}
			set
			{
				forceConfigFile = value;
			}
		}
		static string forceConfigFile = null;
		/// <summary>
		/// The full name of the CS-Script engine custom config file (eg. %CSSCRIPT_DIR%\css_config.dat).  
		/// </summary>
		public static Settings Settings
		{
			get
			{	
				//System.Diagnostics.Debug.Assert(false);
				if (ScriptFile != null) //we are running the script engine
				{
					string configFile = FindExecuteOptionsField(Assembly.GetExecutingAssembly(), "altConfig");
					if (configFile == null)
						configFile = FindExecuteOptionsField(Assembly.GetEntryAssembly(), "altConfig");

					if (configFile == null || configFile == "")
						return Settings.Load(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "css_config.xml"));
					else
						return Settings.Load(Path.GetFullPath(configFile), false);
				}
				return null;
			}
		}

		/// <summary>
		/// The directory where CS-Script engine keeps autogenerated files (subject of HideAutoGeneratedFiles setting).
		/// </summary>
		public static string CacheDirectory
		{
			get
			{
				if (ScriptFile != null) //we are running the script engine
				{
					//Console.WriteLine("<"+Path.GetFullPath(scriptFile).ToLower());
					return Path.Combine(CSExecutor.GetScriptTempDir(), @"Cache\"+Path.GetFullPath(ScriptFile).ToLower().GetHashCode().ToString());
				}
				else 
					return null;
			}
		}
		/// <summary>
		/// Generates the name of the cache directory for the specified script file.
		/// </summary>
		/// <param name="file">Script file name.</param>
		/// <returns>Cache directory name.</returns>
		public static string GetCacheDirectory(string file)
		{
			return Path.Combine(CSExecutor.GetScriptTempDir(), @"Cache\"+Path.GetFullPath(file).ToLower().GetHashCode().ToString());
		}
		/// <summary>
		/// The full name of the script file being executed.  
		/// </summary>
		public static string ScriptFile
		{
			get
			{
				scriptFile = FindExecuteOptionsField(Assembly.GetExecutingAssembly(), "scriptFileName");
				if (scriptFile == null)
					scriptFile = FindExecuteOptionsField(Assembly.GetEntryAssembly(), "scriptFileName");
				return scriptFile;
			}
		}
		static private string scriptFile = null;

		/// <summary>
		/// The full name of the primary script file being executed. Usually it is the sam file as ScriptFile. 
		/// However these fields are different if analysed analised from the pre/post-script.
		/// </summary>
		public static string PrimaryScriptFile
		{
			get
			{
				if (scriptFileNamePrimary == null)
				{
					scriptFileNamePrimary = FindExecuteOptionsField(Assembly.GetExecutingAssembly(), "scriptFileNamePrimary");
					if (scriptFileNamePrimary == null || scriptFileNamePrimary == "")
						scriptFileNamePrimary = FindExecuteOptionsField(Assembly.GetEntryAssembly(), "scriptFileNamePrimary");
				}
				return scriptFileNamePrimary;
			}
		}
		static private string scriptFileNamePrimary = null;

		static private string FindExecuteOptionsField(Assembly asm, string field)
		{
			Type t = asm.GetModules()[0].GetType("csscript.CSExecutor+ExecuteOptions");
			if (t != null)
			{
				foreach (FieldInfo fi in t.GetFields(BindingFlags.Static | BindingFlags.Public))
				{
					if (fi.Name == "options")
					{
						//need to use reflection as we might be running either cscs.exe or the script host application
						//thus there is no warranty which assembly contains correct "options" object
						object otionsObject = fi.GetValue(null);
						if (otionsObject != null)
						{
							object scriptFileObject = otionsObject.GetType().GetField(field).GetValue(otionsObject);
							if (scriptFileObject != null)
								return scriptFileObject.ToString();
						}
						break;
					}
				}
			}
			return null;
		}
		private CSSEnvironment()
		{
		}
		
		/// <summary>
		/// Returns the ConfigurationSection object for the passed configuration section name and path. 
		/// </summary>
		/// <param name="sectionName">Config file section name.</param>
		/// <returns>Instance of the ConfigurationSection.</returns>
		public static object GetConfig(string sectionName)
		{
			if (ConfigFile == null)
				throw new ApplicationException("The config file is not known.\nThe possible reasons is that you are not running the C# script.\nYou may work around the problem by setting the CSSEnvironment.ConfigFile property manually before accessing the config file.");
			XmlDocument xmlDoc = new XmlDocument();

			xmlDoc.Load(ConfigFile);

			if(sectionName == "appSettings")
				return GetAppSettingsFileHandler(sectionName, GetHandler(sectionName, xmlDoc), xmlDoc);
			else
				return GetHandler(sectionName, xmlDoc).Create(null, null, xmlDoc.SelectSingleNode("//" + sectionName));
		}
		/// <summary>
		/// Gets the AppSettingsSection object configuration section that applies to this Configuration object. 
		/// </summary>
		public static NameValueCollection AppSettings
		{
			get
			{
				NameValueCollection retval = (NameValueCollection)GetConfig("appSettings");
				if (retval == null)
					return new NameValueCollection();
				else
					return retval;
			}
		}
		private static IConfigurationSectionHandler GetHandler(string sectionName, XmlDocument xmlDoc)
		{
			if(sectionName == "appSettings")
				return new NameValueSectionHandler();

			string[] sections = sectionName.Split('/');
			XmlNode node = null;
			
			//see if we have a section group that we have to go through
			if(sections.Length > 1)
				node = xmlDoc.SelectSingleNode("/configuration/configSections/sectionGroup[@name='" + sections[0] + "']/section[@name='" + sections[1] + "']");
			else
				node = xmlDoc.SelectSingleNode("/configuration/configSections/section[@name='" + sections[0] + "']");

			string typeName = node.Attributes["type", ""].Value;

			if(typeName == null || typeName.Length == 0)
				return null;

			Type handlerType = Type.GetType(typeName);
			return (IConfigurationSectionHandler)Activator.CreateInstance(handlerType);
		}

		private static object GetAppSettingsFileHandler(string sectionName, IConfigurationSectionHandler parentHandler, XmlDocument xmlDoc)
		{
			object handler = null;
			XmlNode node = xmlDoc.SelectSingleNode("//" + sectionName);
			XmlAttribute att = (XmlAttribute)node.Attributes.RemoveNamedItem("file");

			if(att == null || att.Value == null || att.Value.Length == 0)
			{
				return parentHandler.Create(null, null, node);
			}
			else
			{
				string fileName = att.Value;
				string dir = Path.GetDirectoryName(fileName);
				string fullName = Path.Combine(dir, fileName);
				XmlDocument xmlDoc2 = new XmlDocument();
				xmlDoc2.Load(fullName);

				object parent = parentHandler.Create(null, null, node);
				IConfigurationSectionHandler h = new NameValueSectionHandler();
				handler = h.Create(parent, null, xmlDoc2.DocumentElement);
			}

			return handler;
		}
	}
	delegate void PrintDelegate(string msg);
	/// <summary>
	/// Repository for application specific data
	/// </summary>
	class AppInfo
	{
		public static string appName = "CSScriptLibrary";
		public static bool appConsole = false;
		public static string appLogo
		{
			get { return "C# Script execution engine. Version "+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()+".\nCopyright (C) 2004 Oleg Shilo.\n";}
		}
		public static string appLogoShort
		{
			get { return "C# Script execution engine. Version "+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()+".\n";}
		}
		public static string appParams = "[/nl]:";
		public static string appParamsHelp = "nl	-	No logo mode: No banner will be shown at execution time.\n";
	}
}
