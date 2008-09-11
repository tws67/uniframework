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
	/// Represents a subset of a running application's configuration as an instrumentation instance class.
	/// </summary>
	/// <remarks>
	/// Class <see cref="ConfigurationSetting"/> is the base of the hierarchy of classes that represent configuration
	/// information as WMI objects. It allows generic queries to be written to retrieve all the configuration 
	/// information published for a given application. Properties <see cref="ConfigurationSetting.ApplicationName"/> and
	/// <see cref="ConfigurationSetting.SectionName"/> provide a way to filter the required information to a single
	/// application or configuration section.
	/// </remarks>
	[InstrumentationClass(InstrumentationType.Abstract)]
	public abstract class ConfigurationSetting
	{
		private String applicationName;
		private String sectionName;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationSetting"/> class.
		/// </summary>
		protected ConfigurationSetting()
		{}

		/// <summary>
		/// Gets the name of the application to which the <see cref="ConfigurationSetting"/> instance represents
		/// configuration information.
		/// </summary>
		public String ApplicationName
		{
			get { return applicationName; }
			internal set { applicationName = value; }
		}

		/// <summary>
		/// Gets the name of the section where the <see cref="ConfigurationSetting"/> instance represented
		/// configuration information resides.
		/// </summary>
		public String SectionName
		{
			get { return sectionName; }
			internal set { sectionName = value; }
		}
	}
}
