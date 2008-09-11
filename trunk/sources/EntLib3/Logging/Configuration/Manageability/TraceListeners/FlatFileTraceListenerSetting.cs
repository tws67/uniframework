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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.FlatFileTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class FlatFileTraceListenerSetting : TraceListenerSetting
	{
		private String fileName;
		private String header;
		private String footer;
		private String formatter;

		internal FlatFileTraceListenerSetting(String name,
			String fileName,
			String header,
			String footer,
			String formatter,
			String traceOutputOptions)
			: base(name, traceOutputOptions)
		{
			this.fileName = fileName;
			this.header = header;
			this.footer = footer;
			this.formatter = formatter;
		}

		/// <summary>
		/// Gets the file name for the represented configuration element.
		/// </summary>
		public String FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		/// <summary>
		/// Gets the header for the represented configuration element.
		/// </summary>
		public String Header
		{
			get { return header; }
			set { header = value; }
		}

		/// <summary>
		/// Gets the footer for the represented configuration element.
		/// </summary>
		public String Footer
		{
			get { return footer; }
			set { footer = value; }
		}

		/// <summary>
		/// Gets the name of the formatter for the represented configuration element.
		/// </summary>
		public String Formatter
		{
			get { return formatter; }
			set { formatter = value; }
		}
	}
}
