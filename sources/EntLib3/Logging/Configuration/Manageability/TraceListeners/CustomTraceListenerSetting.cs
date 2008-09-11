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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CustomTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomTraceListenerSetting : TraceListenerSetting
	{
		private String[] attributes;
		private String initData;
		private String formatter;
		private String listenerType;

		internal CustomTraceListenerSetting(String name,
			String listenerType,
			String initData,
			String[] attributes,
			String traceOutputOptions,
			String formatter)
			: base(name, traceOutputOptions)
		{
			this.listenerType = listenerType;
			this.initData = initData;
			this.attributes = attributes;
			this.formatter = formatter;
		}

		/// <summary>
		/// Gets the attributes for the represented configuration element.
		/// </summary>
		/// <remarks>
		/// The attributes are encoded as an string array of name/value pairs.
		/// </remarks>
		public String[] Attributes
		{
			get { return attributes; }
			internal set { attributes = value; }
		}

		/// <summary>
		/// Gets the initialization data for the represented configuration element.
		/// </summary>
		public String InitData
		{
			get { return initData; }
			internal set { initData = value; }
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
		/// Gets the assembly qualified name of the listener type for 
		/// the represented configuration element.
		/// </summary>
		public String ListenerType
		{
			get { return listenerType; }
			internal set { listenerType = value; }
		}
	}
}
