//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using ManagementInstrumentation = System.Management.Instrumentation.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Properties;
using System.Diagnostics;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Instrumentation
{
	/// <summary>
	/// The instrumentation gateway when no instances of the objects from the block are involved.
	/// </summary>
	[EventLogDefinition("Application", EventLogSourceName)]
	[CustomFactory(typeof(DefaultCryptographyEventLoggerCustomFactory))]
	public class DefaultCryptographyEventLogger : InstrumentationListener
	{
		private IEventLogEntryFormatter eventLogEntryFormatter;

		/// For testing purposes
		public const string EventLogSourceName = "Enterprise Library Cryptography";

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultCryptographyEventLogger"/> class, specifying whether 
		/// logging to the event log and firing WMI events is allowed.
		/// </summary>
		/// <param name="eventLoggingEnabled"><b>true</b> if writing to the event log is allowed, <b>false</b> otherwise.</param>
		/// <param name="wmiEnabled"><b>true</b> if firing WMI events is allowed, <b>false</b> otherwise.</param>
		public DefaultCryptographyEventLogger(bool eventLoggingEnabled, bool wmiEnabled)
			: base((string)null, false, eventLoggingEnabled, wmiEnabled, null)
		{
			eventLogEntryFormatter = new EventLogEntryFormatter(Resources.BlockName);
		}

		/// <summary>
		/// Logs the occurrence of a configuration error for the Enterprise Library Cryptography Application Block through the 
		/// available instrumentation mechanisms.
		/// </summary>
		/// <param name="instanceName">Name of the cryptographic provider instance in which the configuration error was detected.</param>
		/// <param name="messageTemplate">The template to build the event log entry.</param>
		/// <param name="exception">The exception raised for the configuration error.</param>
		public void LogConfigurationError(string instanceName, string messageTemplate, Exception exception)
		{
			if (WmiEnabled) FireManagementInstrumentation(new CryptographyConfigurationFailureEvent(instanceName, exception.ToString()));
			if (EventLoggingEnabled)
			{
				string errorMessage
					= string.Format(
						Resources.Culture,
						messageTemplate,
						instanceName);
				string entryText = eventLogEntryFormatter.GetEntryText(errorMessage, exception);

				EventLog.WriteEntry(GetEventSourceName(), entryText, EventLogEntryType.Error);
			}
		}
	}
}
