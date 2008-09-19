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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CacheManagerSetting : NamedConfigurationSetting
	{
		private String cacheStorage;
		private int expirationPollFrequencyInSeconds;
		private int maximumElementsInCacheBeforeScavenging;
		private int numberToRemoveWhenScavenging;

		internal CacheManagerSetting(String name, 
			String cacheStorage, 
			int expirationPollFrequencyInSeconds, 
			int maximumElementsInCacheBeforeScavenging, 
			int numberToRemoveWhenScavenging)
			: base(name)
		{
			this.cacheStorage = cacheStorage;
			this.expirationPollFrequencyInSeconds = expirationPollFrequencyInSeconds;
			this.maximumElementsInCacheBeforeScavenging = maximumElementsInCacheBeforeScavenging;
			this.numberToRemoveWhenScavenging = numberToRemoveWhenScavenging;
		}

		/// <summary>
		/// Gets the name of the cache storage for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerData.CacheStorage">CacheManagerData.CacheStorage</seealso>
		public String CacheStorage
		{
			get { return cacheStorage; }
			internal set { cacheStorage = value; }
		}

		/// <summary>
		/// Gets the expiration poll frequency for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerData.ExpirationPollFrequencyInSeconds">CacheManagerData.ExpirationPollFrequencyInSeconds</seealso>
		public int ExpirationPollFrequencyInSeconds
		{
			get { return expirationPollFrequencyInSeconds; }
			internal set { expirationPollFrequencyInSeconds = value; }
		}

		/// <summary>
		/// Gets the maximum number of elements in cache before scavenging for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerData.MaximumElementsInCacheBeforeScavenging">CacheManagerData.MaximumElementsInCacheBeforeScavenging</seealso>
		public int MaximumElementsInCacheBeforeScavenging
		{
			get { return maximumElementsInCacheBeforeScavenging; }
			internal set { maximumElementsInCacheBeforeScavenging = value; }
		}

		/// <summary>
		/// Gets the number of elements to remove when scavenging for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerData.NumberToRemoveWhenScavenging">CacheManagerData.NumberToRemoveWhenScavenging</seealso>
		public int NumberToRemoveWhenScavenging
		{
			get { return numberToRemoveWhenScavenging; }
			internal set { numberToRemoveWhenScavenging = value; }
		}
	}
}
