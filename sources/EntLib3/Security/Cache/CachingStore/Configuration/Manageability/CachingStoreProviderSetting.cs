//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Security Application Block
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
using Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.CachingStoreProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.CachingStoreProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>	
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CachingStoreProviderSetting : SecurityCacheProviderSetting
	{
		private int absoluteExpiration;
		private String cacheManager;
		private int slidingExpiration;

		internal CachingStoreProviderSetting(String name,
			String cacheManager,
			int absoluteExpiration,
			int slidingExpiration)
			: base(name)
		{
			this.cacheManager = cacheManager;
			this.absoluteExpiration = absoluteExpiration;
			this.slidingExpiration = slidingExpiration;
		}

		/// <summary>
		/// Gets the absolute expiration for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.CachingStoreProviderData.AbsoluteExpiration">
		/// CachingStoreProviderData.AbsoluteExpiration</seealso>
		public int AbsoluteExpiration
		{
			get { return absoluteExpiration; }
			internal set { absoluteExpiration = value; }
		}

		/// <summary>
		/// Gets the name of the cache manager for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.CachingStoreProviderData.CacheManager">
		/// CachingStoreProviderData.CacheManager</seealso>
		public String CacheManager
		{
			get { return cacheManager; }
			internal set { cacheManager = value; }
		}

		/// <summary>
		/// Gets the sliding expiration for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.CachingStoreProviderData.SlidingExpiration">
		/// CachingStoreProviderData.SlidingExpiration</seealso>
		public int SlidingExpiration
		{
			get { return slidingExpiration; }
			internal set { slidingExpiration = value; }
		}
	}
}