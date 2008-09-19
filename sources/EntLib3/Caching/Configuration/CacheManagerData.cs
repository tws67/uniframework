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

using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Configuration
{
	/// <summary>
	/// Configuration data defining CacheManagerData. Defines the information needed to properly configure
	/// a CacheManager instance.
	/// </summary>    	
	public class CacheManagerData : NamedConfigurationElement
	{
		private const string expirationPollFrequencyInSecondsProperty = "expirationPollFrequencyInSeconds";
		private const string maximumElementsInCacheBeforeScavengingProperty = "maximumElementsInCacheBeforeScavenging";
		private const string numberToRemoveWhenScavengingProperty = "numberToRemoveWhenScavenging";
		private const string backingStoreNameProperty = "backingStoreName";

		/// <summary>
		/// Initialize a new instance of the <see cref="CacheManagerData"/> class.
		/// </summary>
		public CacheManagerData()
		{
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="CacheManagerData"/> class.
		/// </summary>
		/// <param name="name">
		/// The name of the <see cref="CacheManagerData"/>.
		/// </param>
		/// <param name="expirationPollFrequencyInSeconds">
		/// Frequency in seconds of expiration polling cycle
		/// </param>
		/// <param name="maximumElementsInCacheBeforeScavenging">
		/// Maximum number of items in cache before an add causes scavenging to take place
		/// </param>
		/// <param name="numberToRemoveWhenScavenging">
		/// Number of items to remove from cache when scavenging
		/// </param>
		/// <param name="cacheStorage">
		/// CacheStorageData object from configuration describing how data is stored 
		/// in the cache.
		/// </param>
		public CacheManagerData(string name, int expirationPollFrequencyInSeconds, int maximumElementsInCacheBeforeScavenging, int numberToRemoveWhenScavenging, string cacheStorage)
			: base(name)
		{
			this.ExpirationPollFrequencyInSeconds = expirationPollFrequencyInSeconds;
			this.MaximumElementsInCacheBeforeScavenging = maximumElementsInCacheBeforeScavenging;
			this.NumberToRemoveWhenScavenging = numberToRemoveWhenScavenging;
			this.CacheStorage = cacheStorage;
		}

		/// <summary>
		/// Frequency in seconds of expiration polling cycle
		/// </summary>
		[ConfigurationProperty(expirationPollFrequencyInSecondsProperty, IsRequired = true)]
		public int ExpirationPollFrequencyInSeconds
		{
			get { return (int)base[expirationPollFrequencyInSecondsProperty]; }
			set { base[expirationPollFrequencyInSecondsProperty] = value; }
		}

		/// <summary>
		/// Maximum number of items in cache before an add causes scavenging to take place
		/// </summary>
		[ConfigurationProperty(maximumElementsInCacheBeforeScavengingProperty, IsRequired = true)]
		public int MaximumElementsInCacheBeforeScavenging
		{
			get { return (int)base[maximumElementsInCacheBeforeScavengingProperty]; }
			set { base[maximumElementsInCacheBeforeScavengingProperty] = value; }
		}

		/// <summary>
		/// Number of items to remove from cache when scavenging
		/// </summary>
		[ConfigurationProperty(numberToRemoveWhenScavengingProperty, IsRequired = true)]
		public int NumberToRemoveWhenScavenging
		{
			get { return (int)base[numberToRemoveWhenScavengingProperty]; }
			set { base[numberToRemoveWhenScavengingProperty] = value; }
		}

		/// <summary>
		/// CacheStorageData object from configuration describing how data is stored 
		/// in the cache.
		/// </summary>
		[ConfigurationProperty(backingStoreNameProperty, IsRequired = true)]
		public string CacheStorage
		{
			get { return (string)base[backingStoreNameProperty]; }
			set { base[backingStoreNameProperty] = value; }
		}
	}

	/// <summary>
	/// This type supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
	/// Resolves default names for cache managers.
	/// </summary>
	public class CacheManagerDataRetriever : IConfigurationNameMapper
	{
		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Returns the default cache manager name from the configuration in the <paramref name="configSource"/>, if the
		/// value for <paramref name="name"/> is <see langword="null"/> (<b>Nothing</b> in Visual Basic).
		/// </summary>
		/// <param name="name">The current name.</param>
		/// <param name="configurationSource">The source for configuration information.</param>
		/// <returns>The default cache manager name if <paramref name="name"/> is <see langword="null"/> (<b>Nothing</b> in Visual Basic),
		/// otherwise the original value for <b>name</b>.</returns>
		public string MapName(string name, IConfigurationSource configurationSource)
		{
			if (name == null)
			{
				CachingConfigurationView view = new CachingConfigurationView(configurationSource);
				return view.DefaultCacheManager;
			}

			return name;
		}
	}
}