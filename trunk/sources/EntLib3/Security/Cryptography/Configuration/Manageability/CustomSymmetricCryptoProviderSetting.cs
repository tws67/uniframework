//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
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

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CustomSymmetricCryptoProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CustomSymmetricCryptoProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomSymmetricCryptoProviderSetting : SymmetricCryptoProviderSetting
	{
		private String providerType;
		private String[] attributes;

		internal CustomSymmetricCryptoProviderSetting(String name, String providerType, String[] attributes)
			: base(name)
		{
			this.providerType = providerType;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the name of the type for the custom symmetric crypto provider for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Common.Configuration.NameTypeConfigurationElement.Type">
		/// Inherited NameTypeConfigurationElement.Type</seealso>
		public String ProviderType
		{
			get { return providerType; }
			set { providerType = value; }
		}

		/// <summary>
		/// Gets the collection of attributes for the custom symmetric crypto provider represented as a 
		/// <see cref="String"/> array of key/value pairs for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CustomSymmetricCryptoProviderData.Attributes">
		/// CustomSymmetricCryptoProviderData.Attributes</seealso>
		public String[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}
	}
}
