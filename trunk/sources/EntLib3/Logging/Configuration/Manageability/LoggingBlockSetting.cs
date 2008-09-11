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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information for the <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings">Logging</see>
	/// features provided by the Enterprise Library.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class LoggingBlockSetting : ConfigurationSetting
	{
		private String defaultCategory;
		private bool logWarningWhenNoCategoriesMatch;
		private bool tracingEnabled;

		internal LoggingBlockSetting(String defaultCategory, bool logWarningWhenNoCategoriesMatch, bool tracingEnabled)
		{
			this.defaultCategory = defaultCategory;
			this.logWarningWhenNoCategoriesMatch = logWarningWhenNoCategoriesMatch;
			this.tracingEnabled = tracingEnabled;
		}

		/// <summary>
		/// Gets the name of the default log category for the represented configuration section.
		/// </summary>
		public String DefaultCategory
		{
			get { return defaultCategory; }
			internal set { defaultCategory = value; }
		}

		/// <summary>
		/// Gets the value for the represented configuration section.
		/// </summary>
		public bool LogWarningWhenNoCategoriesMatch
		{
			get { return logWarningWhenNoCategoriesMatch; }
			internal set { logWarningWhenNoCategoriesMatch = value; }
		}

		/// <summary>
		/// Gets the tracing enabled status for the represented configuration section.
		/// </summary>
		public bool TracingEnabled
		{
			get { return tracingEnabled; }
			internal set { tracingEnabled = value; }
		}
	}
}
