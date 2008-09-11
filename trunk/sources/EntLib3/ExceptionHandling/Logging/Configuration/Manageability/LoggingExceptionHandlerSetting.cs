//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Management.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class LoggingExceptionHandlerSetting : ExceptionHandlerSetting
	{
		private int eventId;
		private String formatterType;
		private String logCategory;
		private int priority;
		private String severity;
		private String title;

		internal LoggingExceptionHandlerSetting(String name,
			int eventId,
			String formatterType,
			String logCategory,
			int priority,
			String severity,
			String title)
			: base(name)
		{
			this.eventId = eventId;
			this.formatterType = formatterType;
			this.logCategory = logCategory;
			this.priority = priority;
			this.severity = severity;
			this.title = title;
		}

		/// <summary>
		/// Gets the event id for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData.EventId">LoggingExceptionHandlerData.EventId</seealso>
		public int EventId
		{
			get { return eventId; }
			internal set { eventId = value; }
		}

		/// <summary>
		/// Gets the name of the formatter type for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData.FormatterType">LoggingExceptionHandlerData.FormatterType</seealso>
		public String FormatterType
		{
			get { return formatterType; }
			internal set { formatterType = value; }
		}

		/// <summary>
		/// Gets the log category for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData.LogCategory">LoggingExceptionHandlerData.LogCategory</seealso>
		public String LogCategory
		{
			get { return logCategory; }
			internal set { logCategory = value; }
		}

		/// <summary>
		/// Gets the priority for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData.Priority">LoggingExceptionHandlerData.Priority</seealso>
		public int Priority
		{
			get { return priority; }
			internal set { priority = value; }
		}

		/// <summary>
		/// Gets the severity for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData.Severity">LoggingExceptionHandlerData.Severity</seealso>
		public String Severity
		{
			get { return severity; }
			internal set { severity = value; }
		}

		/// <summary>
		/// Gets the title for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging.Configuration.LoggingExceptionHandlerData.Title">LoggingExceptionHandlerData.Title</seealso>
		public String Title
		{
			get { return title; }
			internal set { title = value; }
		}
	}
}
