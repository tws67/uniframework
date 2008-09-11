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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.PriorityFilterData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class PriorityFilterSetting : LogFilterSetting
	{
		private int maximumPriority;
		private int minimumPriority;

		internal PriorityFilterSetting(String name, int maximumPriority, int minimumPriority)
			: base(name)
		{
			this.maximumPriority = maximumPriority;
			this.minimumPriority = minimumPriority;
		}

		/// <summary>
		/// Gets the minimum priority for the represented configuration element.
		/// </summary>
		public int MinimumPriority
		{
			get { return minimumPriority; }
			internal set { minimumPriority = value; }
		}

		/// <summary>
		/// Gets the maximum priority for the represented configuration element.
		/// </summary>
		public int MaximumPriority
		{
			get { return maximumPriority; }
			internal set { maximumPriority = value; }
		}	
	}
}
