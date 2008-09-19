//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Security Application Block
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

namespace Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability
{
	/// <summary>
	/// Represents the general configuration information for the Security Application Block.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.SecuritySettings"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class SecurityBlockSetting : ConfigurationSetting
	{
		private String defaultAuthorizationProvider;
		private String defaultSecurityCacheProvider;

		internal SecurityBlockSetting(String defaultAuthorizationProvider, String defaultSecurityCacheProvider)
		{
			this.defaultAuthorizationProvider = defaultAuthorizationProvider;
			this.defaultSecurityCacheProvider = defaultSecurityCacheProvider;
		}

		/// <summary>
		/// Gets the name of the default hash provider for the represented configuration section.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.SecuritySettings.DefaultAuthorizationProviderName">
		/// SecuritySettings.DefaultAuthorizationProviderName</seealso>
		public String DefaultAuthorizationProvider
		{
			get { return defaultAuthorizationProvider; }
			set { defaultAuthorizationProvider = value; }
		}

		/// <summary>
		/// Gets the name of the default hash provider for the represented configuration section.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.SecuritySettings.DefaultSecurityCacheProviderName">
		/// SecuritySettings.DefaultSecurityCacheProviderName</seealso>
		public String DefaultSecurityCacheProvider
		{
			get { return defaultSecurityCacheProvider; }
			set { defaultSecurityCacheProvider = value; }
		}
	}
}
