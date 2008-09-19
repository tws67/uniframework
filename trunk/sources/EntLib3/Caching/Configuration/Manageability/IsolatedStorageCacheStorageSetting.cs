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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.IsolatedStorageCacheStorageData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.IsolatedStorageCacheStorageData"/>
	/// <seealso cref="CacheStorageSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class IsolatedStorageCacheStorageSetting : CacheStorageSetting
	{
		private String partitionName;
		private String storageEncryption;

		internal IsolatedStorageCacheStorageSetting(String name, String partitionName, String storageEncryption)
			: base(name)
		{
			this.partitionName = partitionName;
			this.storageEncryption = storageEncryption;
		}

		/// <summary>
		/// Gets the name of partition for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.IsolatedStorageCacheStorageData.PartitionName">IsolatedStorageCacheStorageData.PartitionName</seealso>
		public String PartitionName
		{
			get { return partitionName; }
			internal set { partitionName = value; }
		}

		/// <summary>
		/// Gets the name of the optional storage encryption provider for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheStorageData.StorageEncryption">Inherited CacheStorageData.StorageEncryption</seealso>
		public String StorageEncryption
		{
			get { return storageEncryption; }
			internal set { storageEncryption = value; }
		}
	}
}
