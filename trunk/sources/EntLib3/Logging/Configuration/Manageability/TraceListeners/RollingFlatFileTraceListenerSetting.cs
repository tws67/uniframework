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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.RollingFlatFileTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class RollingFlatFileTraceListenerSetting : TraceListenerSetting
	{
		private String fileName;
		private String formatter;
		private String rollFileExistsBehavior;
		private String rollInterval;
		private int rollSizeKB;
		private string timeStampPattern;
		private String header;
		private String footer;

		internal RollingFlatFileTraceListenerSetting(String name,
			String fileName,
			String header,
			String footer,
			String formatter,
			String rollFileExistsBehavior,
			String rollInterval,
			int rollSizeKB,
			string timeStampPattern,
			String traceOutputOptions)
			: base(name, traceOutputOptions)
		{
			this.fileName = fileName;
			this.header = header;
			this.footer = footer;
			this.formatter = formatter;
			this.rollFileExistsBehavior = rollFileExistsBehavior;
			this.rollInterval = rollInterval;
			this.rollSizeKB = rollSizeKB;
			this.timeStampPattern = timeStampPattern;
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
		/// Gets the name of the formatter for the represented configuration element.
		/// </summary>
		public String Formatter
		{
			get { return formatter; }
			set { formatter = value; }
		}

		/// <summary>
		/// Gets the rollFileExistsBehavior for the represented configuration element.
		/// </summary>
		public String RollFileExistsBehavior
		{
			get { return rollFileExistsBehavior; }
			set { rollFileExistsBehavior = value; }
		}

		/// <summary>
		/// Gets the roll interval for the represented configuration element.
		/// </summary>
		public String RollInterval
		{
			get { return rollInterval; }
			set { rollInterval = value; }
		}

		/// <summary>
		/// Gets the roll size for the represented configuration element.
		/// </summary>
		public int RollSizeKB
		{
			get { return rollSizeKB; }
			set { rollSizeKB = value; }
		}

		/// <summary>
		/// Gets the timestamp pattern for the represented configuration element.
		/// </summary>
		public String TimeStampPattern
		{
			get { return timeStampPattern; }
			set { timeStampPattern = value; }
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
	}
}