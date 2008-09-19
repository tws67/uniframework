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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.HashAlgorithmProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.HashAlgorithmProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class HashAlgorithmProviderSetting : HashProviderSetting
	{
		private String algorithmType;
		private bool saltEnabled;
	
		internal HashAlgorithmProviderSetting(String name, String algorithmType, bool saltEnabled)
			: base(name)
		{
			this.algorithmType = algorithmType;
			this.saltEnabled = saltEnabled;
		}

		/// <summary>
		/// Gets the name of the algorithm type for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.HashAlgorithmProviderData.AlgorithmType">
		/// HashAlgorithmProviderData.AlgorithmType</seealso>
		public String AlgorithmType
		{
			get { return algorithmType; }
			internal set { algorithmType = value; }
		}

		/// <summary>
		/// Gets the value of the salt enabled property for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.HashAlgorithmProviderData.SaltEnabled">
		/// HashAlgorithmProviderData.SaltEnabled</seealso>
		public bool SaltEnabled
		{
			get { return saltEnabled; }
			internal set { saltEnabled = value; }
		}
	}
}
