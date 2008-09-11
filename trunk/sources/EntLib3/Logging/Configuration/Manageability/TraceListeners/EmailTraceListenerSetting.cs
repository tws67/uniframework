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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.EmailTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class EmailTraceListenerSetting : TraceListenerSetting
	{
		private String formatter;
		private String fromAddress;
		private int smtpPort;
		private String smtpServer;
		private String subjectLineEnder;
		private String subjectLineStarter;
		private String toAddress;

		internal EmailTraceListenerSetting(String name,
			String formatter,
			String fromAddress,
			int smtpPort,
			String smtpServer,
			String subjectLineEnder,
			String subjectLineStarter,
			String toAddress,
			String traceOutputOptions)
			: base(name, traceOutputOptions)
		{
			this.formatter = formatter;
			this.fromAddress = fromAddress;
			this.smtpPort = smtpPort;
			this.smtpServer = smtpServer;
			this.subjectLineEnder = subjectLineEnder;
			this.subjectLineStarter = subjectLineStarter;
			this.toAddress = toAddress;
		}

		/// <summary>
		/// Gets the name of the formatter for the represented configuration element.
		/// </summary>
		public String Formatter
		{
			get { return formatter; }
			internal set { formatter = value; }
		}

		/// <summary>
		/// Gets the from address for the represented configuration element.
		/// </summary>
		public String FromAddress
		{
			get { return fromAddress; }
			internal set { fromAddress = value; }
		}

		/// <summary>
		/// Gets the smtp port for the represented configuration element.
		/// </summary>
		public int SmtpPort
		{
			get { return smtpPort; }
			internal set { smtpPort = value; }
		}

		/// <summary>
		/// Gets the smtp server for the represented configuration element.
		/// </summary>
		public String SmtpServer
		{
			get { return smtpServer; }
			internal set { smtpServer = value; }
		}

		/// <summary>
		/// Gets the subject line ender for the represented configuration element.
		/// </summary>
		public String SubjectLineEnder
		{
			get { return subjectLineEnder; }
			internal set { subjectLineEnder = value; }
		}

		/// <summary>
		/// Gets the subject line starter for the represented configuration element.
		/// </summary>
		public String SubjectLineStarter
		{
			get { return subjectLineStarter; }
			internal set { subjectLineStarter = value; }
		}

		/// <summary>
		/// Gets the to address for the represented configuration element.
		/// </summary>
		public String ToAddress
		{
			get { return toAddress; }
			internal set { toAddress = value; }
		}
	}
}
