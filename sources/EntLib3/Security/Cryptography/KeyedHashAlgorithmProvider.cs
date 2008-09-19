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
using System.Security.Cryptography;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Properties;
using System.IO;


namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography
{
    /// <summary>
    /// A hash provider for any hash algorithm which derives from <see cref="System.Security.Cryptography.KeyedHashAlgorithm"/>.
    /// </summary>
	[ConfigurationElementType(typeof(KeyedHashAlgorithmProviderData))]
	public class KeyedHashAlgorithmProvider : HashAlgorithmProvider
    {
		private ProtectedKey key;

		/// <summary>
		/// Initialize a new instance of the <see cref="KeyedHashAlgorithmProvider"/> class with a <see cref="KeyedHashAlgorithm"/>, if salt is enabled, and the key to use.
		/// </summary>
		/// <param name="algorithmType">
		/// The <see cref="KeyedHashAlgorithm"/> to use.
		/// </param>
		/// <param name="saltEnabled"><see langword="true"/> if salt should be used; otherwise, <see langword="false"/>.</param>
		/// <param name="protectedKeyFileName">File name of DPAPI-protected key used to encrypt and decrypt secrets through this provider.</param>
		/// <param name="protectedKeyFileProtectionScope"><see cref="DataProtectionScope"/> used to protect the key on disk. </param>
		public KeyedHashAlgorithmProvider(Type algorithmType, bool saltEnabled, string protectedKeyFileName, DataProtectionScope protectedKeyFileProtectionScope)
			: this(algorithmType, saltEnabled, KeyManager.Read(protectedKeyFileName, protectedKeyFileProtectionScope))
		{
		}

		internal KeyedHashAlgorithmProvider(Type algorithmType, bool saltEnabled, ProtectedKey protectedKey)
			: base(algorithmType, saltEnabled)
		{
			if(protectedKey == null) throw new ArgumentNullException("key");
			if (!typeof(KeyedHashAlgorithm).IsAssignableFrom(algorithmType)) throw new ArgumentException(Resources.ExceptionMustBeAKeyedHashAlgorithm, "algorithmType");

			this.key = protectedKey;
		}

		/// <summary>
        /// Gets the cryptographer used for hashing.
        /// </summary>
        /// <returns>The cryptographer initialized with the configured key.</returns>
        protected override HashCryptographer HashCryptographer
        {
			get { return new HashCryptographer(AlgorithmType, key); }
        }
    }
}