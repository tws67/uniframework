//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
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

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information for an database connection defined by the connection 
	/// strings configuration section.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class ConnectionStringSetting : NamedConfigurationSetting
	{
		private String connectionString;
		private String providerName;

		internal ConnectionStringSetting(
			String name,
			String connectionString,
			String providerName
		)
			: base(name)
		{
			this.connectionString = connectionString;
			this.providerName = providerName;
		}

		/// <summary>
		/// Gets the connection string for the represented <see cref="System.Configuration.ConnectionStringSettings"/> instance.
		/// </summary>
		public String ConnectionString
		{
			get { return connectionString; }
			internal set { connectionString = value; }
		}

		/// <summary>
		/// Gets the provider name for the represented <see cref="System.Configuration.ConnectionStringSettings"/> instance.
		/// </summary>
		public String ProviderName
		{
			get { return providerName; }
			internal set { providerName = value; }
		}
	}
}
