#region Licence...
//-----------------------------------------------------------------------------
// Date:	10/9/05	Time: 3:00p
// Module:	AsmHelper.cs
// Classes:	AsmHelper
//
// This module contains the definition of the AsmHelper class. Which implements 
// dynamic assembly loading/unloading and invoking methods from loaded assembly.
//
// Written by Oleg Shilo (oshilo@gmail.com)
// Copyright (c) 2005-2007. All rights reserved.
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
using System.Reflection;
using System.Globalization;

using csscript;

namespace CSScriptLibrary
{
	/// <summary>
	/// Helper class to simplify working with dynamically loaded assemblies. 
	/// </summary>
	public class AsmHelper : IDisposable
	{
		IAsmBrowser asmBrowser;
		AppDomain remoteAppDomain;

		bool deleteOnExit = false;
		/// <summary>
		/// Creates an instance of AsmHelper for working with assembly dynamically loaded to current AppDomain. 
		/// Calling "Dispose" is optional for "current AppDomain"scenario as no new AppDomain will be ever created.
		/// </summary>
		/// <param name="asm">Assembly object.</param>
		public AsmHelper(Assembly asm)
		{
			this.asmBrowser = (IAsmBrowser)(new AsmBrowser(asm));
		}
		/// <summary>
		/// Creates an instance of AsmHelper for working with assembly dynamically loaded to non-current AppDomain. 
		/// This mathod initialises instance and creates new ('remote') AppDomain with 'domainName' name. New AppDomain is automatically unloaded as result of "disposable" behaviour of AsmHelper.
		/// </summary>
		/// <param name="asmFile">File name of the assembly to be loaded.</param>
		/// <param name="domainName">Name of the domain to be created.</param>
		/// <param name="deleteOnExit">'true' if assembly file should be deleted when new AppDomain is unloaded; otherwise, 'false'.</param>
		public AsmHelper(string asmFile, string domainName, bool deleteOnExit)
		{
			this.deleteOnExit = deleteOnExit;
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationBase = Path.GetDirectoryName(asmFile);
			setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
			setup.ApplicationName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
			setup.ShadowCopyFiles = "true";
			setup.ShadowCopyDirectories = Path.GetDirectoryName(asmFile);
			remoteAppDomain = AppDomain.CreateDomain(domainName, null, setup);
	
			AsmRemoteBrowser asmBrowser = (AsmRemoteBrowser)remoteAppDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(AsmRemoteBrowser).ToString());
			asmBrowser.asmFile = asmFile;
			this.asmBrowser = (IAsmBrowser)asmBrowser;
		}
		/// <summary>
		/// Executes static method of the underlying assembly.
		/// </summary>
		/// <param name="methodName">'Method' name including 'Type' name (eg. MyType.DoJob).</param>
		/// <param name="list">List of 'Method' arguments.</param>
		/// <returns>Returns object of the same type as 'Method' return type.</returns>
		public object Invoke(string methodName, params object[] list)
		{
			if (this.disposed)
				throw new ObjectDisposedException(this.ToString());
			return asmBrowser.Invoke(methodName, list);	
		}
		/// <summary>
		/// Executes an instance method of the underlying assembly.
		/// </summary>
		/// <param name="obj">Instance of the object whos method is to be invoked.</param>
		/// <param name="methodName">'Method' name (excncluding 'Type' name).</param>
		/// <param name="list">List of 'Method' arguments.</param>
		/// <returns>Returns object of the same type as 'Method' return type.</returns>
		public object InvokeInst(object obj, string methodName, params object[] list)
		{
			if (this.disposed)
				throw new ObjectDisposedException(this.ToString());
			return asmBrowser.Invoke(obj, methodName, list);
		}
		/// <summary>
		/// Creates instance of a class from underlying assembly.
		/// </summary>
		/// <param name="typeName">The 'Type' full name of the type to create. (see Assembly.CreateInstance())</param>
		/// <returns>Instance of the 'Type'.</returns>
		public object CreateObject(string typeName)
		{
			if(this.disposed)
				throw new ObjectDisposedException(this.ToString());
			return asmBrowser.CreateInstance(typeName);
		}
		/// <summary>
		/// Unloads 'remote' AppDomain if it was created.
		/// </summary>
		private void Unload()
		{
			try
			{
				if (remoteAppDomain != null)
				{
					string asmFile = ((AsmRemoteBrowser)this.asmBrowser).asmFile;
					AppDomain.Unload(remoteAppDomain);
					remoteAppDomain = null;
					if (deleteOnExit)
					{
						File.Delete(asmFile);
					}
				}
			}
			catch 
			{
				//ignore exception as it is possible that we are trying to unload AppDomain 
				//during the object finalization (which is illegal).
			}
		}
		/// <summary>
		/// Implementation of IDisposable.Dispose(). Disposes allocated exetrnal resources if any. Call this method to unload non-current AppDomain (if it was created).
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Actual implementation of IDisposable.Dispose()
		/// </summary>
		/// <param name="disposing">'false' if the method has been called by the runtime from inside the finalizer ; otherwise, 'true'.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(!this.disposed)
			{
				Unload();
			}
			disposed = true;		 
		}
		/// <summary>
		/// Finalizer
		/// </summary>
		~AsmHelper()	  
		{
			Dispose(false);
		}
		bool disposed = false;

	}
	/// <summary>
	/// Defines method for calling assembly methods and instantiating assembly types.
	/// </summary>
	interface IAsmBrowser
	{
		object Invoke(string methodName, params object[] list);
		object Invoke(object obj, string methodName, params object[] list);
		object CreateInstance(string typeName);
	}
	class AsmRemoteBrowser: MarshalByRefObject, IAsmBrowser
	{
		string workingDir;
		
		public AsmBrowser asmBrowser;
		public string asmFile;
		
		public Assembly ResolveEventHandler(object sender, ResolveEventArgs args)
		{
			return AssemblyResolver.ResolveAssembly(args.Name, workingDir);
		}
		public object Invoke(string methodName, params object[] list)
		{
			if (asmBrowser == null)
			{
				if (asmFile == null)
					throw new Exception("Assembly name (asmFile) was not set");

				workingDir = Path.GetDirectoryName(asmFile);
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveEventHandler);
				asmBrowser = new AsmBrowser(Assembly.LoadFrom(asmFile));
			}
			return asmBrowser.Invoke(methodName, list);	
		}
		public object Invoke(object obj, string methodName, params object[] list)
		{
			if (asmBrowser == null)
			{
				if (asmFile == null)
					throw new Exception("Assembly name (asmFile) was not set");

				workingDir = Path.GetDirectoryName(asmFile);
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveEventHandler);
				asmBrowser = new AsmBrowser(Assembly.LoadFrom(asmFile));
			}
			return asmBrowser.Invoke(obj, methodName, list);	
		}
		//creates instance of a Type from underying assembly
		public object CreateInstance(string typeName)
		{
			if (asmBrowser == null)
			{
				if (asmFile == null)
					throw new Exception("Assembly name (asmFile) was not set");

				workingDir = Path.GetDirectoryName(asmFile);
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveEventHandler);
				asmBrowser = new AsmBrowser(Assembly.LoadFrom(asmFile));
			}
			return asmBrowser.CreateInstance(typeName);
		}
	}
	
	class AsmBrowser : IAsmBrowser
	{
		private Assembly asm;
		public AsmBrowser(Assembly asm)
		{
			if (asm == null)
				throw new ArgumentNullException("asm");
			this.asm = asm;
		}
		//executes static method of underying assembly
		public object Invoke(string methodName, params object[] list)
		{
			string[] names = methodName.Split(".".ToCharArray());
			if (names.Length < 2)
				throw new Exception("Invalid method name format (must be: \"<type>.<method>\")");
			Module module = asm.GetModules()[0];
			Type[] result = module.FindTypes(Module.FilterTypeName, names[names.Length-2]);
			if (result.Length == 0)
				throw new Exception("Invalid type name.");
			
			MethodInfo method = result[0].GetMethod(names[names.Length-1]);
			if (method == null)
				throw new Exception("Invalid method name.");
				
			return method.Invoke(null, list);	
		}
		//executes instance method of underying assembly
		public object Invoke(object obj, string methodName, params object[] list)
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;
			return obj.GetType().InvokeMember(methodName, flags, null, obj, list, CultureInfo.InvariantCulture);
			
			//MethodInfo method = obj.GetType().GetMethod(names[names.Length - 1]);
			//if (method == null)
			//	throw new Exception("Invalid method name.");

			//return method.Invoke(obj, list);	
		}
		//creates instance of a Type from underying assembly
		public object CreateInstance(string typeName)
		{
			return asm.CreateInstance(typeName);
		}
	}
}

