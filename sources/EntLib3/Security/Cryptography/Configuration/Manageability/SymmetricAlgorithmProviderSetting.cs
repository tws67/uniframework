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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.SymmetricAlgorithmProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.SymmetricAlgorithmProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class SymmetricAlgorithmProviderSetting : SymmetricCryptoProviderSetting
	{
		private String algorithmType;
		private String protectedKeyFilename;
		private String protectedKeyProtectionScope;

		internal SymmetricAlgorithmProviderSetting(String name,
			String algorithmType,
			String protectedKeyFilename,
			String protectedKeyProtectionScope)
			: base(name)
		{
			this.algorithmType = algorithmType;
			this.protectedKeyFilename = protectedKeyFilename;
			this.protectedKeyProtectionScope = protectedKeyProtectionScope;
		}

		/// <summary>
		/// Gets the name of the algorithm type for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.SymmetricAlgorithmProviderData.AlgorithmType">
		/// SymmetricAlgorithmProviderData.AlgorithmType</seealso>
		public String AlgorithmType
		{
			get { return algorithmType; }
			internal set { algorithmType = value; }
		}

		/// <summary>
		/// Gets the name of the protected key file for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.SymmetricAlgorithmProviderData.ProtectedKeyFilename">
		/// SymmetricAlgorithmProviderData.ProtectedKeyFilename</seealso>
		public String ProtectedKeyFilename
		{
			get { return protectedKeyFilename; }
			internal set { protectedKeyFilename = value; }
		}

		/// <summary>
		/// Gets the name of the protected key scope value for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.SymmetricAlgorithmProviderData.ProtectedKeyProtectionScope">
		/// SymmetricAlgorithmProviderData.ProtectedKeyProtectionScope</seealso>
		public String ProtectedKeyProtectionScope
		{
			get { return protectedKeyProtectionScope; }
			internal set { protectedKeyProtectionScope = value; }
		}
	}
}
