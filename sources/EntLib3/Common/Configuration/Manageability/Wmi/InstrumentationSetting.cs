//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Management.Instrumentation;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information for the <see cref="Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation.Configuration.InstrumentationConfigurationSection">Instrumentation</see>
	/// features provided by the Enterprise Library.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class InstrumentationSetting : ConfigurationSetting
	{
		private Boolean eventLoggingEnabled;
		private Boolean performanceCountersEnabled;
		private Boolean wmiEnabled;

		internal InstrumentationSetting(Boolean eventLoggingEnabled, Boolean performanceCountersEnabled, Boolean wmiEnabled)
			: base()
		{
			this.eventLoggingEnabled = eventLoggingEnabled;
			this.performanceCountersEnabled = performanceCountersEnabled;
			this.wmiEnabled = wmiEnabled;
		}

		/// <summary>
		/// Gets the event logging enablement status on the represented instrumentation configuration.
		/// </summary>
		public Boolean EventLoggingEnabled
		{
			get { return eventLoggingEnabled; }
			internal set { eventLoggingEnabled = value; }
		}

		/// <summary>
		/// Gets the performance counter enablement status on the represented instrumentation configuration.
		/// </summary>
		public Boolean PerformanceCountersEnabled
		{
			get { return performanceCountersEnabled; }
			internal set { performanceCountersEnabled = value; }
		}

		/// <summary>
		/// Gets the wmi enablement status on the represented instrumentation configuration.
		/// </summary>
		public Boolean WmiEnabled
		{
			get { return wmiEnabled; }
			internal set { wmiEnabled = value; }
		}
	}
}
