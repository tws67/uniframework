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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CustomHashProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CustomHashProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomHashProviderSetting : HashProviderSetting
	{
		private String providerType;
		private String[] attributes;

		internal CustomHashProviderSetting(String name, String providerType, String[] attributes)
			: base(name)
		{
			this.providerType = providerType;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the name of the type for the custom hash provider for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Common.Configuration.NameTypeConfigurationElement.Type">Inherited NameTypeConfigurationElement.Type</seealso>
		public String ProviderType
		{
			get { return providerType; }
			set { providerType = value; }
		}

		/// <summary>
		/// Gets the collection of attributes for the custom hash provider represented as a 
		/// <see cref="String"/> array of key/value pairs for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CustomHashProviderData.Attributes">
		/// CustomHashProviderData.Attributes</seealso>
		public String[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}
	}
}
