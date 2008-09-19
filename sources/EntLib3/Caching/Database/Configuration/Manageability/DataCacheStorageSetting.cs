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

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.DataCacheStorageData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.DataCacheStorageData"/>
	/// <seealso cref="CacheStorageSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class DataCacheStorageSetting : CacheStorageSetting
	{
		private String databaseInstanceName;
		private String partitionName;
		private String storageEncryption;

		internal DataCacheStorageSetting(String name, String databaseInstanceName, String partitionName, String storageEncryption)
			: base(name)
		{
			this.databaseInstanceName = databaseInstanceName;
			this.partitionName = partitionName;
			this.storageEncryption = storageEncryption;
		}

		/// <summary>
		/// Gets the name of database for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.DataCacheStorageData.DatabaseInstanceName">DataCacheStorageData.DatabaseInstanceName</seealso>
		public String DatabaseInstanceName
		{
			get { return databaseInstanceName; }
			set { databaseInstanceName = value; }
		}

		/// <summary>
		/// Gets the name of partition for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.DataCacheStorageData.PartitionName">DataCacheStorageData.PartitionName</seealso>
		public String PartitionName
		{
			get { return partitionName; }
			set { partitionName = value; }
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
