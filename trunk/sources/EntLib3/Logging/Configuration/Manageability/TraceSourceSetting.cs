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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.TraceSourceData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class TraceSourceSetting : NamedConfigurationSetting
	{
		private String defaultLevel;
		private String[] traceListeners;
		private String kind;

		internal TraceSourceSetting(String name, String defaultLevel, String[] traceListeners, String kind)
			: base(name)
		{
			this.defaultLevel = defaultLevel;
			this.traceListeners = traceListeners;
			this.kind = kind;
		}

		/// <summary>
		/// Gets the name of the value of the default level for the represented configuration element.
		/// </summary>
		public String DefaultLevel
		{
			get { return defaultLevel; }
			internal set { defaultLevel = value; }
		}

		/// <summary>
		/// Gets the names of the referenced trace listeners for the represented configuration element.
		/// </summary>
		public String[] TraceListeners
		{
			get { return traceListeners; }
			internal set { traceListeners = value; }
		}

		/// <summary>
		/// Gets the kind of the represented configuration element.
		/// </summary>
		/// <remarks>
		/// Trace sources can be:
		/// <list type="table">
		/// <listheader>
		/// <term>Kind</term>
		/// <description>Description</description>
		/// </listheader>
		/// <item><term>Category</term>
		/// <description>A plain category source.</description></item>
		/// <item><term>All events</term>
		/// <description>The <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.SpecialTraceSourcesData.AllEventsTraceSource">allEvents</see> special source.</description></item>
		/// <item><term>Errors</term>
		/// <description>The <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.SpecialTraceSourcesData.ErrorsTraceSource">errors</see> special source.</description></item>
		/// <item><term>Not processed</term>
		/// <description>The <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.SpecialTraceSourcesData.NotProcessedTraceSource">notProcessed</see> special source.</description></item>
		/// </list>
		/// </remarks>
		public String Kind
		{
			get { return kind; }
		}
	}
}
