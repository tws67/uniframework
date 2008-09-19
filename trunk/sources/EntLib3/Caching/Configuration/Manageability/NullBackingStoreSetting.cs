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

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheStorageData"/> instance.
	/// </summary>
	/// <remarks>
	/// Class CacheStorageData is both the root of the cache storage implementation configuration objects and the
	/// configuration object that represents a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Caching.BackingStoreImplementations.NullBackingStore"/>.
	/// </remarks>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheStorageData"/>
	/// <seealso cref="CacheStorageSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class NullBackingStoreSetting : CacheStorageSetting
	{
		private String storageEncryption;

		internal NullBackingStoreSetting(String name, String storageEncryption)
			: base(name)
		{
			this.storageEncryption = storageEncryption;
		}

		/// <summary>
		/// Gets the name of the optional storage encryption provider for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheStorageData.StorageEncryption"/>
		public String StorageEncryption
		{
			get { return storageEncryption; }
			internal set { storageEncryption = value; }
		}
	}
}
