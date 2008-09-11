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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.FormattedEventLogTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class FormattedEventLogTraceListenerSetting : TraceListenerSetting
	{
		private String formatter;
		private String log;
		private String machineName;
		private String source;

		internal FormattedEventLogTraceListenerSetting(String name,
			String source,
			String log,
			String machineName,
			String formatter,
			String traceOutputOptions)
			: base(name, traceOutputOptions)
		{
			this.source = source;
			this.log = log;
			this.machineName = machineName;
			this.formatter = formatter;
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
		/// Gets the log name for the represented configuration element.
		/// </summary>
		public String Log
		{
			get { return log; }
			set { log = value; }
		}

		/// <summary>
		/// Gets the machine name for the represented configuration element.
		/// </summary>
		public String MachineName
		{
			get { return machineName; }
			set { machineName = value; }
		}

		/// <summary>
		/// Gets the source name for the represented configuration element.
		/// </summary>
		public String Source
		{
			get { return source; }
			set { source = value; }
		}
	}
}
