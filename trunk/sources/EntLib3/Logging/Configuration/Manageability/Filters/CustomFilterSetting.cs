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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Filters
{
	/// <summary>
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CustomLogFilterData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomFilterSetting : LogFilterSetting
	{
		private String filterType;
		private String[] attributes;

		internal CustomFilterSetting(String name, String filterType, String[] attributes)
			: base(name)
		{
			this.filterType = filterType;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the assembly qualified name of the filter type for the represented configuration element.
		/// </summary>
		public String FilterType
		{
			get { return filterType; }
			internal set { filterType = value; }
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
	}
}
