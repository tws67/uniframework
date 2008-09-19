//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Caching Application Block
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
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.SymmetricStorageEncryptionProviderData"/> 
	/// instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.SymmetricStorageEncryptionProviderData"/> 
	/// <seealso cref="StorageEncryptionProviderSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class SymmetricStorageEncryptionProviderSetting : StorageEncryptionProviderSetting
	{
		private String symmetricInstance;

		internal SymmetricStorageEncryptionProviderSetting(String name, String symmetricInstance)
			: base(name)
		{
			this.symmetricInstance = symmetricInstance;
		}

		/// <summary>
		/// Gets the name of symmetric encryption provider for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.SymmetricStorageEncryptionProviderData.SymmetricInstance">
		/// SymmetricStorageEncryptionProviderData.SymmetricInstance</seealso>
		public String SymmetricInstance
		{
			get { return symmetricInstance; }
			set { symmetricInstance = value; }
		}
	}
}
