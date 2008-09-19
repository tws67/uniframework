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
	/// Represents the general configuration information for the Cryptography Application Block.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CryptographySettings"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CryptographyBlockSetting : ConfigurationSetting
	{
		private String defaultHashProvider;
		private String defaultSymmetricCryptoProvider;

		internal CryptographyBlockSetting(String defaultHashProvider, String defaultSymmetricCryptoProvider)
		{
			this.defaultHashProvider = defaultHashProvider;
			this.defaultSymmetricCryptoProvider = defaultSymmetricCryptoProvider;
		}

		/// <summary>
		/// Gets the name of the default hash provider for the represented configuration section.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CryptographySettings.DefaultHashProviderName">
		/// CryptographySettings.DefaultHashProviderName</seealso>
		public String DefaultHashProvider
		{
			get { return defaultHashProvider; }
			internal set { defaultHashProvider = value; }
		}

		/// <summary>
		/// Gets the name of the default symmetric crypto provider for the represented configuration section.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.CryptographySettings.DefaultSymmetricCryptoProviderName">
		/// CryptographySettings.DefaultSymmetricCryptoProviderName</seealso>
		public String DefaultSymmetricCryptoProvider
		{
			get { return defaultSymmetricCryptoProvider; }
			internal set { defaultSymmetricCryptoProvider = value; }
		}
	}
}
