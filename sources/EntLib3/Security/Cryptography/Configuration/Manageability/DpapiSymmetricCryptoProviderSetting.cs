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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.DpapiSymmetricCryptoProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.DpapiSymmetricCryptoProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class DpapiSymmetricCryptoProviderSetting : SymmetricCryptoProviderSetting
	{
		private String scope;

		internal DpapiSymmetricCryptoProviderSetting(String name, String scope)
			: base(name)
		{
			this.scope = scope;
		}

		/// <summary>
		/// Gets the name of the scope value for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.DpapiSymmetricCryptoProviderData.Scope">
		/// DpapiSymmetricCryptoProviderData.Scope</seealso>
		public String Scope
		{
			get { return scope; }
			internal set { scope = value; }
		}
	}
}
