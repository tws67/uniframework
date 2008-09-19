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
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching.Instrumentation;

namespace Microsoft.Practices.EnterpriseLibrary.Caching
{
    /// <summary>
    /// This type supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
    /// Represents the process used to build an instance of <see cref="CacheManagerData"/> described by the <see cref="CacheManagerSettings"/> configuration section.
    /// </summary>
    /// <remarks>
    /// This is used by the <see cref="ConfiguredObjectStrategy"/> when an instance of the <see cref="CacheManagerData"/> class is requested to 
    /// a properly configured <see cref="IBuilder{TStageEnum}"/> instance.
    /// </remarks>
    public class CacheManagerCustomFactory : ICustomFactory
	{
        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// Builds a <see cref="CacheManagerData"/> described by the <see cref="CacheManagerSettings"/> configuration section.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> that represents the current building process.</param>
        /// <param name="name">The name of the instance to build. It is part of the <see cref="ICustomFactory.CreateObject(IBuilderContext, string, IConfigurationSource, ConfigurationReflectionCache)"/> method, but it is not used in this implementation.</param>
        /// <param name="configurationSource">The source for configuration objects.</param>
        /// <param name="reflectionCache">The cache to use retrieving reflection information.</param>
        /// <returns>A fully initialized instance of <see cref="CacheManager"/>.</returns>
        public object CreateObject(IBuilderContext context, string name, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
		{
			CacheManagerData objectConfiguration = GetConfiguration(name, configurationSource);

			IBackingStore backingStore
				= BackingStoreCustomFactory.Instance.Create(context, objectConfiguration.CacheStorage, configurationSource, reflectionCache);

			CachingInstrumentationProvider instrumentationProvider = CreateInstrumentationProvider(objectConfiguration.Name, configurationSource, reflectionCache);

			CacheManager createdObject
				= new CacheManagerFactoryHelper().BuildCacheManager(
					objectConfiguration.Name,
					backingStore,
					objectConfiguration.MaximumElementsInCacheBeforeScavenging,
					objectConfiguration.NumberToRemoveWhenScavenging,
					objectConfiguration.ExpirationPollFrequencyInSeconds,
					instrumentationProvider);

			RegisterObject(context, name, createdObject);

			return createdObject;
		}

		private static CachingInstrumentationProvider CreateInstrumentationProvider(string name, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
		{
			CachingInstrumentationProvider instrumentationProvider = new CachingInstrumentationProvider();
			new InstrumentationAttachmentStrategy().AttachInstrumentation(name, instrumentationProvider, configurationSource, reflectionCache);

			return instrumentationProvider;
		}

		private CacheManagerData GetConfiguration(string id, IConfigurationSource configurationSource)
		{
			CachingConfigurationView view = new CachingConfigurationView(configurationSource);
			return view.GetCacheManagerData(id);
		}

		private void RegisterObject(IBuilderContext context, string name, CacheManager createdObject)
		{
			if (context.Locator != null)
			{
				ILifetimeContainer lifetime = context.Locator.Get<ILifetimeContainer>(typeof(ILifetimeContainer), SearchMode.Local);

				if (lifetime != null)
				{
					context.Locator.Add(new DependencyResolutionLocatorKey(typeof(CacheManager), name), createdObject);
					lifetime.Add(createdObject);
				}
			}
		}
	}

	/// <summary>
	/// Public for thesting purposes
	/// </summary>
	internal class CacheManagerFactoryHelper
	{
		/// <summary>
		/// Made public for testing purposes.
		/// </summary>
		public CacheManager BuildCacheManager(
			string cacheManagerName,
			IBackingStore backingStore,
			int maximumElementsInCacheBeforeScavenging,
			int numberToRemoveWhenScavenging,
			int expirationPollFrequencyInSeconds,
			CachingInstrumentationProvider instrumentationProvider)
		{
			CacheCapacityScavengingPolicy scavengingPolicy =
				new CacheCapacityScavengingPolicy(maximumElementsInCacheBeforeScavenging);

			Cache cache = new Cache(backingStore, scavengingPolicy, instrumentationProvider);

			ExpirationPollTimer timer = new ExpirationPollTimer();
			ExpirationTask expirationTask = CreateExpirationTask(cache, instrumentationProvider);
			ScavengerTask scavengerTask = new ScavengerTask(numberToRemoveWhenScavenging, scavengingPolicy, cache, instrumentationProvider);
			BackgroundScheduler scheduler = new BackgroundScheduler(expirationTask, scavengerTask, instrumentationProvider);
			cache.Initialize(scheduler);

			scheduler.Start();
			timer.StartPolling(new TimerCallback(scheduler.ExpirationTimeoutExpired), expirationPollFrequencyInSeconds * 1000);

			return new CacheManager(cache, scheduler, timer);
		}

		/// <summary>
		/// Made protected for testing purposes.
		/// </summary>
		/// <param name="cacheOperations">For testing only.</param>
		/// <param name="instrumentationProvider">For testing only.</param>
		/// <returns>For testing only.</returns>
		protected internal virtual ExpirationTask CreateExpirationTask(ICacheOperations cacheOperations, CachingInstrumentationProvider instrumentationProvider)
		{
			return new ExpirationTask(cacheOperations, instrumentationProvider);
		}
	}
}
