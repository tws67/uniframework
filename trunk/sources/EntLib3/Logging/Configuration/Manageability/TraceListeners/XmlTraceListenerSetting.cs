//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Management.Instrumentation;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	/// <summary>
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.XmlTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class XmlTraceListenerSetting : TraceListenerSetting
	{
		private String fileName;

		internal XmlTraceListenerSetting(String name,
			String fileName,
			String traceOutputOptions)
			: base(name, traceOutputOptions)
		{
			this.fileName = fileName;
		}

		/// <summary>
		/// Gets the file name for the represented configuration element.
		/// </summary>
		public String FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}
	}
}
