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
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.CustomAuthorizationProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.CustomAuthorizationProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>	
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomAuthorizationProviderSetting : AuthorizationProviderSetting
	{
		private String providerType;
		private String[] attributes;

		internal CustomAuthorizationProviderSetting(String name, String providerType, String[] attributes)
			: base(name)
		{
			this.providerType = providerType;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the name of the type for the custom authorization provider for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Common.Configuration.NameTypeConfigurationElement.Type">
		/// Inherited NameTypeConfigurationElement.Type</seealso>
		public String ProviderType
		{
			get { return providerType; }
			set { providerType = value; }
		}

		/// <summary>
		/// Gets the collection of attributes for the custom authorization provider represented as a 
		/// <see cref="String"/> array of key/value pairs for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.CustomAuthorizationProviderData.Attributes">
		/// CustomAuthorizationProviderData.Attributes</seealso>
		public String[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}
	}
}
