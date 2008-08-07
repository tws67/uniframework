#region Licence...
//-----------------------------------------------------------------------------
// Date:	31/01/05	Time: 17:15p
// Module:	fileparser.cs
// Classes:	ParsingParams
//			ScriptInfo
//			ScriptParser
//			FileParser
//			FileParserComparer
//
// This module contains the definition of the classes which implement 
// parsing script code. The result of such processing is a collections of the names 
// of the namespacs and assemblies used by the script code.
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2004-2007. All rights reserved.
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
using System.Text;
using System.Collections;
using System.Diagnostics;
using csscript;

namespace CSScriptLibrary
{
	/// <summary>
	/// ParsingParams is an class that holds parsing parameters (parameters that controls how file is to be parsed). 
	/// At this moment they are namespace renaming rules only.
	/// </summary>
	class ParsingParams 
	{
		#region Public interface...
		public ParsingParams()
		{
			renameNamespaceMap = new ArrayList();
		}
		public string[][] RenameNamespaceMap
		{
			get {return (string[][])renameNamespaceMap.ToArray(typeof(string[])); }
		}
		public void AddRenameNamespaceMap(string[][] names)
		{
			renameNamespaceMap.AddRange(names);
		}
		/// <summary>
		/// Compare() is to be used to help with implementation of IComparer for sorting operations.
		/// </summary>
		public static int Compare(ParsingParams xPrams, ParsingParams yPrams)
		{
			if (xPrams == null && yPrams == null)
				return 0;

			int retval =  xPrams == null ? -1 : (yPrams == null ? 1 : 0); 
			
			if (retval == 0)
			{
				string[][] xNames = xPrams.RenameNamespaceMap;
				string[][] yNames = yPrams.RenameNamespaceMap;
				retval = Comparer.Default.Compare(xNames.Length, yNames.Length);
				if (retval == 0)
				{
					for (int i = 0; i < xNames.Length && retval == 0; i++)
					{
						retval = Comparer.Default.Compare(xNames[i].Length, yNames[i].Length);
						if (retval == 0)
						{
							for (int j = 0; j < xNames[i].Length; j++)
							{
								retval = Comparer.Default.Compare(xNames[i][j], yNames[i][j]);
							}
						}
					}
				}
			}
			return retval;
		}

		public bool preserveMain = false;
		#endregion
		private ArrayList renameNamespaceMap;
	}

	/// <summary>
	/// Class which is a placeholder for general information of the script file
	/// </summary>
	class ScriptInfo
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="info">ImportInfo object containing the information how the script file should be parsed.</param>
		public ScriptInfo(CSharpParser.ImportInfo info)
		{
			this.fileName = info.file;
			parseParams.AddRenameNamespaceMap(info.renaming);
			parseParams.preserveMain = info.preserveMain ;
		}  
		public ParsingParams parseParams = new ParsingParams();
		public string fileName;
	}

	/// <summary>
	/// Class that implements parsing the single C# script file
	/// </summary>
	class FileParser 
	{
		public System.Threading.ApartmentState ThreadingModel
		{
			get
			{
				if (this.parser == null)
					return System.Threading.ApartmentState.Unknown;
				else
					return this.parser.ThreadingModel;
			}
		}

		public FileParser()
		{
		}
		public FileParser(string fileName, ParsingParams prams, bool process, bool imported, string[] searchDirs)
		{
			this.imported = imported;
			this.prams = prams;
			this.searchDirs = searchDirs;
			this.fileName = ResolveFile(fileName, searchDirs);
			if (process)
				ProcessFile();
		}

		public string fileNameImported = ""; 
		public ParsingParams prams = null;
		
		public string FileToCompile
		{
			get {return imported ? fileNameImported : fileName;}
		}
		public string[] SearchDirs
		{
			get {return searchDirs; }
		}
		public bool Imported
		{
			get {return imported; }
		}

		public string[] ReferencedNamespaces
		{
			get { return parser.RefNamespaces; }
		}
		
		public string[] ReferencedAssemblies
		{
			get {return parser.RefAssemblies;}
		}
		
		public string[] ReferencedResources
		{
			get {return parser.ResFiles;}
		}

		public ScriptInfo[] ReferencedScripts
		{
			get {return (ScriptInfo[])referencedScripts.ToArray(typeof(ScriptInfo)); }
		}
		
		public void ProcessFile()
		{
			referencedAssemblies.Clear();   
			referencedScripts.Clear();	  
			referencedNamespaces.Clear();   
			referencedResources.Clear();

			using (StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding(0))) 
			{
				this.parser = new CSharpParser(sr.ReadToEnd());
			}

			foreach (CSharpParser.ImportInfo info in parser.Imports)
		   	{
				referencedScripts.Add(new ScriptInfo(info)); 
			}
			referencedAssemblies.AddRange(parser.RefAssemblies);
			referencedNamespaces.AddRange(parser.RefNamespaces);
			referencedResources.AddRange(parser.ResFiles);

			if (imported)
			{
				if (prams != null)
				{
					parser.DoRenaming(prams.RenameNamespaceMap, prams.preserveMain);
				}
				if (parser.ModifiedCode == "")
				{
					fileNameImported = fileName; //importing does not require any modification of the original code 
				}
				else
				{
					fileNameImported = Path.Combine(CSExecutor.ScriptCacheDir, string.Format("i_{0}_{1}{2}", Path.GetFileNameWithoutExtension(fileName), Path.GetDirectoryName(fileName).GetHashCode(), Path.GetExtension(fileName)));
					if (!Directory.Exists(Path.GetDirectoryName(fileNameImported)))
						Directory.CreateDirectory(Path.GetDirectoryName(fileNameImported));
					if (File.Exists(fileNameImported))
					{
						File.SetAttributes(fileNameImported, FileAttributes.Normal);
						File.Delete(fileNameImported);
					}

					using (StreamWriter scriptWriter = new StreamWriter(fileNameImported, false, Encoding.GetEncoding(0))) 
					{
						//scriptWriter.Write(ComposeHeader(fileNameImported)); //using a big header at start is overkill (it also shifts line numbers so they do not match with the original script file) 
						//but maight be required in future
						scriptWriter.WriteLine(parser.ModifiedCode);
						scriptWriter.WriteLine("///////////////////////////////////////////");
						scriptWriter.WriteLine("// Compiler-generated file - DO NOT EDIT!");
						scriptWriter.WriteLine("///////////////////////////////////////////");
					}
					File.SetAttributes(fileNameImported, FileAttributes.ReadOnly);
				}
			}
		}

		private ArrayList referencedScripts = new ArrayList();
		private ArrayList referencedNamespaces = new ArrayList();
		private ArrayList referencedAssemblies = new ArrayList();
		private ArrayList referencedResources = new ArrayList();
		

		private string[] searchDirs;
		private bool imported = false;
			
		/// <summary>
		/// Searches for script file by given script name. Calls ResolveFile(string fileName, string[] extraDirs, bool throwOnError) 
		/// with throwOnError flag set to true.
		/// </summary>
		public static string ResolveFile(string fileName, string[] extraDirs)
		{
			return ResolveFile(fileName, extraDirs, true);
		}
		/// <summary>
		/// Searches for script file by given script name. Search order:
		/// 1. Current directory
		/// 2. extraDirs (usually %CSSCRIPT_DIR%\Lib and ExtraLibDirectory)
		/// 3. PATH
		/// Also fixes file name if user did not provide extension for script file (assuming .cs extension)
		/// </summary>
		public static string ResolveFile(string file, string[] extraDirs, bool throwOnError)
		{
			string fileName = file;
			//current directory
			if (Path.GetExtension(fileName) == "")
				fileName += ".cs";

			if (File.Exists(fileName))
			{
				return Path.GetFullPath(fileName);
			}
			
			//arbitrary directories
			if (extraDirs != null)
			{
				foreach (string extraDir in extraDirs)
				{
					string dir = extraDir;
					if (File.Exists(Path.Combine(dir, fileName)))
					{
						return Path.GetFullPath(Path.Combine(dir, fileName));
					}
				}
			}

			//PATH
			string[] pathDirs = Environment.GetEnvironmentVariable("PATH").Replace("\"", "").Split(';');
			foreach(string pathDir in pathDirs)
			{
				string dir = pathDir;
				if (File.Exists(Path.Combine(dir, fileName)))
				{
					return Path.GetFullPath(Path.Combine(dir, fileName));
				}
			}
			
			if (throwOnError)
				throw new FileNotFoundException(string.Format("Could not find file \"{0}\"", fileName));

			return "";
		}
		static public string headerTemplate =
		  		@"/*" + Environment.NewLine +
		   		@" Created by {0}" +
		   		@" Original location: {1}" + Environment.NewLine +
		   		@" C# source equivalent of {2}" + Environment.NewLine +
		   		@" compiler-generated file created {3} - DO NOT EDIT!" + Environment.NewLine +
		   		@"*/" + Environment.NewLine;

		public string ComposeHeader(string path)
		{
				return string.Format(headerTemplate, csscript.AppInfo.appLogoShort, path, fileName, DateTime.Now);
		}

		public string fileName = "";
		public CSharpParser parser;
	}

	/// <summary>
	/// Class that implements parsing the single C# Script file
	/// </summary>
	/// <summary>
	/// Implementation of the IComparer for sorting operations of collections of FileParser instances
	/// </summary>
	class FileParserComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			if (x == null && y == null)
				return 0;

			int retval = x == null ? -1 : (y == null ? 1 : 0); 

			if (retval == 0)
			{
				FileParser xParser = (FileParser)x;
				FileParser yParser = (FileParser)y;
				retval = string.Compare(xParser.fileName, yParser.fileName, true);
				if (retval == 0)
				{
					retval = ParsingParams.Compare(xParser.prams, yParser.prams);
				}
			}

			return retval;
		}
	}

	/// <summary>
	/// Class that manages parsing the main and all imported (if any) C# Script files
	/// </summary>
	public class ScriptParser
	{
		/// <summary>
		/// ApartmentState of a script during the execution (default: ApartmentState.Unknown)
		/// </summary>
		public System.Threading.ApartmentState apartmentState = System.Threading.ApartmentState.Unknown; 
		/// <summary>
		/// Colection of the files to be compiled (including dependand scripts)
		/// </summary>
		public string[] FilesToCompile
		{
			get 
			{
				ArrayList retval = new ArrayList();
				foreach(FileParser file in fileParsers)
					retval.Add(file.FileToCompile);
				return (string[])retval.ToArray(typeof(string)); 
			}
		}
		/// <summary>
		/// Colection of the imported files (dependand scripts)
		/// </summary>
		public string[] ImportedFiles
		{
			get 
			{
				ArrayList retval = new ArrayList();
				foreach(FileParser file in fileParsers)
				{
					if (file.Imported)
						retval.Add(file.fileName);
				}
				return (string[])retval.ToArray(typeof(string)); 
			}
		}
		/// <summary>
		/// Collection of resource files referenced from code
		/// </summary>
		public string[] ReferencedResources
		{
			get {return (string[])referencedResources.ToArray(typeof(string)); }
		}
		/// <summary>
		/// Collection of namespaces referenced from code (including those referenced in dependand scripts)
		/// </summary>
		public string[] ReferencedNamespaces
		{
			get {return (string[])referencedNamespaces.ToArray(typeof(string)); }
		}
		/// <summary>
		/// Collection of referenced asesemblies. All assemblies are referenced either from command-line, code or resolved from referenced namespaces.
		/// </summary>
		public string[] ReferencedAssemblies
		{
			get {return (string[])referencedAssemblies.ToArray(typeof(string)); }
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fileName">Script file name</param>
		public ScriptParser(string fileName)
		{
			Init(fileName, null);
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fileName">Script file name</param>
		/// <param name="searchDirs">Extra ScriptLibrary directory(ies) </param>
		public ScriptParser(string fileName, string[] searchDirs)
		{
			if (CSExecutor.ScriptCacheDir == "") //ust in case if ScriptParser is used outside of the script engine
				CSExecutor.SetScriptCacheDir(fileName);
			Init(fileName, searchDirs);
		}

		/// <summary>
		/// Initialization of ScriptParser instance
		/// </summary>
		/// <param name="fileName">Script file name</param>
		/// <param name="searchDirs">Extra ScriptLibrary directory(ies) </param>
		private void Init(string fileName, string[] searchDirs)
		{
			referencedNamespaces = new ArrayList(); 
			referencedAssemblies = new ArrayList();
			referencedResources = new ArrayList();
			
			//process main file
			FileParser mainFile = new FileParser(fileName, null, true, false, searchDirs);
			this.apartmentState = mainFile.ThreadingModel;

			foreach (string namespaceName in mainFile.ReferencedNamespaces)
				PushNamespace(namespaceName);

			foreach (string asmName in mainFile.ReferencedAssemblies)
				PushAssembly(asmName);
			
			foreach (string resFile in mainFile.ReferencedResources)
				PushResource(resFile);

			ArrayList dirs = new ArrayList();
			dirs.Add(Path.GetDirectoryName(mainFile.fileName));//note: mainFile.fileName is warrantied to be a full name but fileName is not
			if (searchDirs != null)
				dirs.AddRange(searchDirs);
			this.searchDirs = (string[])dirs.ToArray(typeof(string)); 

			//process impported files if any
			foreach (ScriptInfo fileInfo in mainFile.ReferencedScripts)
				ProcessFile(fileInfo);

			//Main script file shall always be the first. Add it now as previously array was sorted a few times
			this.fileParsers.Insert(0, mainFile); 
		}
		
		private void ProcessFile(ScriptInfo fileInfo)
		{
			FileParserComparer fileComparer = new FileParserComparer();
			
			FileParser importedFile = new FileParser(fileInfo.fileName, fileInfo.parseParams, false, true, this.searchDirs); //do not parse it yet (the third param is false)
			if (fileParsers.BinarySearch(importedFile, fileComparer) < 0)
			{
				importedFile.ProcessFile(); //parse now namespaces, ref. assemblies and scripts; also it will do namespace renaming

				this.fileParsers.Add(importedFile);
				this.fileParsers.Sort(fileComparer);

				foreach (string namespaceName in importedFile.ReferencedNamespaces)
					PushNamespace(namespaceName);

				foreach (string asmName in importedFile.ReferencedAssemblies)
					PushAssembly(asmName);

				foreach(ScriptInfo scriptFile in importedFile.ReferencedScripts)
					ProcessFile(scriptFile);
			}
		}

		private ArrayList fileParsers = new ArrayList();
	
		/// <summary>
		/// Saves all imported scripts int temporary location. 
		/// </summary>
		/// <returns>Collection of the saved imported scrips file names</returns>
		public string[] SaveImportedScripts()
		{
			string workingDir = Path.GetDirectoryName(((FileParser)fileParsers[0]).fileName);
			ArrayList retval = new ArrayList();

			foreach(FileParser file in fileParsers)
			{
				if (file.Imported)
				{
					if (file.fileNameImported != file.fileName) //script file was copied
					{
					//	string scriptFile = file.fileNameImported; 
					//	string newFileName =  Path.Combine(workingDir, Path.GetFileName(scriptFile));
					//	if (File.Exists(newFileName))
					//		File.SetAttributes(newFileName, FileAttributes.Normal);
					//	File.Copy(scriptFile, newFileName, true);
					//	retval.Add(newFileName);
						retval.Add(file.fileNameImported);
					}
					else
						retval.Add(file.fileName);
				}
			}
			return (string[])retval.ToArray(typeof(string));
		}
		/// <summary>
		/// Deletes imported scripts as a cleanup operation
		/// </summary>
		public void DeleteImportedFiles()
		{
			foreach(FileParser file in fileParsers)
			{
				if (file.Imported && file.fileNameImported != file.fileName) //the file was copied
				{
					try
					{
						File.SetAttributes(file.FileToCompile, FileAttributes.Normal);
						File.Delete(file.FileToCompile);
					}
					catch{}
				}
			}
		}
				
		private ArrayList referencedNamespaces;
		private ArrayList referencedResources;
		private ArrayList referencedAssemblies;
		private string[] searchDirs;

		private void PushNamespace(string nameSpace)
		{
			if (referencedNamespaces.Count > 1)
				referencedNamespaces.Sort();

			if (referencedNamespaces.BinarySearch(nameSpace) < 0)
				referencedNamespaces.Add(nameSpace);
		}
		private void PushAssembly(string asmName)
		{
			string entrtyName = asmName.ToLower();
			if (referencedAssemblies.Count > 1)
				referencedAssemblies.Sort();

			if (referencedAssemblies.BinarySearch(entrtyName) < 0)
				referencedAssemblies.Add(entrtyName);
		}
		private void PushResource(string resName)
		{
			string entrtyName = resName.ToLower();
			if (referencedResources.Count > 1)
				referencedResources.Sort();

			if (referencedResources.BinarySearch(entrtyName) < 0)
				referencedResources.Add(entrtyName);
		}
	}		
}