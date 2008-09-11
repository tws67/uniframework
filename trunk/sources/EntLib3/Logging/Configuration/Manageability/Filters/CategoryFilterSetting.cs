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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CategoryFilterData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CategoryFilterSetting : LogFilterSetting
	{
		private String categoryFilterMode;
		private String[] categoryFilters;

		internal CategoryFilterSetting(String name, String categoryFilterMode, String[] categoryFilters)
			: base(name)
		{
			this.categoryFilterMode = categoryFilterMode;
			this.categoryFilters = categoryFilters;
		}

		/// <summary>
		/// Gets the name of the filter mode value for the represented configuration element.
		/// </summary>
		public String CategoryFilterMode
		{
			get { return categoryFilterMode; }
			internal set { categoryFilterMode = value; }
		}

		/// <summary>
		/// Gets the set of filtered categories for the represented configuration element.
		/// </summary>
		/// <remarks>
		/// The categories are encoded as an string array of category names.
		/// </remarks>
		public String[] CategoryFilters
		{
			get { return categoryFilters; }
			internal set { categoryFilters = value; }
		}
	}
}
