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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.KeyedHashAlgorithmProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.KeyedHashAlgorithmProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class KeyedHashAlgorithmProviderSetting : HashProviderSetting
	{
		private String algorithmType;
		private bool saltEnabled;
		private String protectedKeyFilename;
		private String protectedKeyProtectionScope;

		internal KeyedHashAlgorithmProviderSetting(String name,
			String algorithmType,
			String protectedKeyFilename,
			String protectedKeyProtectionScope,
			bool saltEnabled)
			: base(name)
		{
			this.algorithmType = algorithmType;
			this.protectedKeyFilename = protectedKeyFilename;
			this.protectedKeyProtectionScope = protectedKeyProtectionScope;
			this.saltEnabled = saltEnabled;
		}

		/// <summary>
		/// Gets the name of the algorithm type for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.HashAlgorithmProviderData.AlgorithmType">
		/// Inherited HashAlgorithmProviderData.AlgorithmType</seealso>
		public String AlgorithmType
		{
			get { return algorithmType; }
			internal set { algorithmType = value; }
		}

		/// <summary>
		/// Gets the name of the protected key file for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.KeyedHashAlgorithmProviderData.ProtectedKeyFilename">
		/// KeyedHashAlgorithmProviderData.ProtectedKeyFilename</seealso>
		public String ProtectedKeyFilename
		{
			get { return protectedKeyFilename; }
			internal set { protectedKeyFilename = value; }
		}

		/// <summary>
		/// Gets the name of the protected key scope value for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.KeyedHashAlgorithmProviderData.ProtectedKeyProtectionScope">
		/// KeyedHashAlgorithmProviderData.ProtectedKeyProtectionScope</seealso>
		public String ProtectedKeyProtectionScope
		{
			get { return protectedKeyProtectionScope; }
			internal set { protectedKeyProtectionScope = value; }
		}

		/// <summary>
		/// Gets the value of the salt enabled property for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.HashAlgorithmProviderData.SaltEnabled">
		/// Inherited HashAlgorithmProviderData.SaltEnabled</seealso>
		public bool SaltEnabled
		{
			get { return saltEnabled; }
			internal set { saltEnabled = value; }
		}
	}
}
