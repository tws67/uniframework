#region Licence...
//-----------------------------------------------------------------------------
// Date:	17/10/04	Time: 2:33p 
// Module:	AssemblyResolver.cs
// Classes:	AssemblyResolver
//
// This module contains the definition of the AssemblyResolver class. Which implements 
// some mothods for simplified Assembly navigation
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
using System.Reflection;
using System.Collections;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using csscript;

namespace CSScriptLibrary
{
	/// <summary>
	/// Class for resolving assembly name to assembly file
	/// </summary>
	public class AssemblyResolver
	{
		#region Class public data...
		/// <summary>
		/// File to be excluded from assembly search
		/// </summary>
		static public string ignoreFileName = "";
		#endregion
		#region Class public methods...
		/// <summary>
		/// Resolves assembly name to assembly file. Loads assembly file to the current AppDomain.
		/// </summary>
		/// <param name="assemblyName">The name of assembly</param>
		/// <param name="dir">The name of directory where local assemblies are expected to be</param>
		/// <returns>loaded assembly</returns>
		static public Assembly ResolveAssembly(string assemblyName, string dir)
		{
			if (Directory.Exists(dir))
			{
				//try file with name AssemblyDisplayName + .dll 
				string[] asmFileNameTokens = assemblyName.Split(", ".ToCharArray(), 5);
			
				string asmFile = Path.Combine(dir, asmFileNameTokens[0])+ ".dll";
				if (ignoreFileName != Path.GetFileName(asmFile) && File.Exists(asmFile))
				{
					try
					{
						AssemblyName asmName = AssemblyName.GetAssemblyName(asmFile);
						if (asmName != null && asmName.FullName == assemblyName)
							return Assembly.LoadFrom(asmFile);
						else if (assemblyName.IndexOf(",") == -1 && asmName.FullName.StartsWith(assemblyName)) //short name requested 
							return Assembly.LoadFrom(asmFile);
					}
					catch{}
				}

				foreach (string file in Directory.GetFiles(dir, asmFileNameTokens[0]+".*"))
				{
					try
					{
						AssemblyName asmName = AssemblyName.GetAssemblyName(file);
						if (asmName != null && asmName.FullName == assemblyName)
							return Assembly.LoadFrom(file);
						else if (assemblyName.IndexOf(",") == -1 && asmName.FullName.StartsWith(assemblyName)) //short name requested
							return Assembly.LoadFrom(asmFile);
					}
					catch{}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Resolves namespace/assembly(file) name into array of assembly locations (local and GAC ones).
		/// </summary>
		/// <param name="name">'namespace'/assembly(file) name</param>
		/// <param name="searchDirs">Assembly seartch directories</param>
		/// <returns>collection of assembly file names wher namespace is impelemented</returns>
		static public string[] FindAssembly(string name, string[] searchDirs)
		{
			ArrayList retval = new ArrayList();
			
			foreach (string dir in searchDirs)
			{
				foreach(string asmLocation in FindLocalAssembly(name, dir))	//local assemblies allternative locations
					retval.Add(asmLocation);

				if (retval.Count != 0)
					break;
			}

			if (retval.Count == 0)
			{	
				foreach(string asmGACLocation in FindGlobalAssembly(name))
				{
					retval.Add(asmGACLocation);
				}
			}
			return (string[])retval.ToArray(typeof(string));
		}
		/// <summary>
		/// Resolves namespace into array of local assembly locations.
		/// (Currently it returns only one assembly location but in future 
		/// it can be extended to collect all assemblies with the same namespace)
		/// </summary>
		/// <param name="name">namespace/assembly name</param>
		/// <param name="dir">directory</param>
		/// <returns>collection of assembly file names wher namespace is impelemented</returns>
		static public string[] FindLocalAssembly(string name, string dir) 
		{
			//We are returning and array because name may represent assembly name or namespace 
			//and as such can consist of more than one assembly file (multiple assembly file is not supported at this stage).
			if (Directory.Exists(dir))
			{	
				string asmFile = Path.Combine(dir, name);
				if(asmFile != Path.GetFileName(asmFile) && File.Exists(asmFile))
					return new string[]{asmFile};

				asmFile += ".dll"; //just in case if user did not specify the extension
				if(ignoreFileName != Path.GetFileName(asmFile) && File.Exists(asmFile))
					return new string[]{asmFile};
			}
			return new string[0];
		}

		/// <summary>
		/// Resolves namespace into array of global assembly (GAC) locations.
		/// </summary>
		/// <param name="namespaceStr">'namespace' name</param>
		/// <returns>collection of assembly file names wher namespace is impelemented</returns>
		static public string[] FindGlobalAssembly(String namespaceStr) 
		{
			ArrayList retval = new ArrayList();
			try
			{ 
				AssemblyEnum asmEnum = new AssemblyEnum(namespaceStr);
				String asmName;
				while ((asmName = asmEnum.GetNextAssembly()) != null)
				{
					string asmLocation = AssemblyCache.QueryAssemblyInfo(asmName);
					retval.Add(asmLocation);
				}
			}
			catch
			{
				//If exception is thrown it is very likely it is because where fusion.dll does not exist/unavailable/broken.
				//We might be running under the MONO run-time. 
			}
			
			if (retval.Count == 0 && namespaceStr.ToLower().EndsWith(".dll"))
				retval.Add(namespaceStr); //in case of if the namespaceStr is a dll name

			return (string[])retval.ToArray(typeof(string));
		}
		#endregion
		/// <summary>
		/// Search for namespace into local assembly file.
		/// </summary>
		static private bool IsNamespaceDefinedInAssembly(string asmFileName, string namespaceStr) 
		{
			if (File.Exists(asmFileName))
			{
				try
				{
					Assembly assembly = Assembly.LoadFrom(asmFileName);
					if (assembly != null)	
					{
						foreach (Module m in assembly.GetModules())
						{
							foreach (Type t in m.GetTypes())
							{
								if (namespaceStr == t.Namespace)
								{
									return true;
								}
							}
						}	
					}
				}
				catch {}
			}
			return false;
		}
	}
}
