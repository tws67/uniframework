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
	/// Represents a subset of a running application's configuration identifiable by name
	/// as an instrumentation instance class.
	/// </summary>
	/// <remarks>
	/// Class <see cref="NamedConfigurationSetting"/> instances usually represent configuration information
	/// residing in instances of a subclass of <see cref="Microsoft.Practices.EnterpriseLibrary.Common.Configuration.NamedConfigurationElement"/>.
	/// </remarks>
	[InstrumentationClass(InstrumentationType.Abstract)]
	public abstract class NamedConfigurationSetting : ConfigurationSetting
	{
		private String name;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigurationSetting"/> class with 
		/// the given <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name that identifies the represented configuration information</param>
		protected NamedConfigurationSetting(String name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets the name that identifies the represented configuration information.
		/// </summary>
		public String Name
		{
			get { return name; }
			internal set { name = value; }
		}
	}
}
