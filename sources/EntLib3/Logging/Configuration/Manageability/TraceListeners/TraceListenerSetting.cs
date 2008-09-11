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
	/// Represents the configuration information for an instance of a concrete subclass of
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.TraceListenerData"/>.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.TraceListenerData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Abstract)]
	public abstract class TraceListenerSetting : NamedConfigurationSetting
	{
		private String traceOutputOptions;

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceListenerSetting"/> class.
		/// </summary>
		protected TraceListenerSetting(String name, String traceOutputOptions)
			: base(name)
		{
			this.traceOutputOptions = traceOutputOptions;
		}

		/// <summary>
		/// Gets the trace options for the represented configuration element.
		/// </summary>
		public String TraceOutputOptions
		{
			get { return traceOutputOptions; }
			internal set { traceOutputOptions = value; }
		}
	}
}
