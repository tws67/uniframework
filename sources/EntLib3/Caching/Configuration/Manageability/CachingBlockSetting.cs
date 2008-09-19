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
	/// Represents the general configuration information for the Caching Application Block.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerSettings"/>
	/// <seealso cref="ConfigurationSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CachingBlockSetting : ConfigurationSetting
	{
		private String defaultCacheManager;

		internal CachingBlockSetting(String defaultCacheManager)
		{
			this.defaultCacheManager = defaultCacheManager;
		}

		/// <summary>
		/// Gets the name of the default cache manager for the represented configuration section.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.CacheManagerSettings.DefaultCacheManager">CacheManagerSettings.DefaultCacheManager</seealso>
		public String DefaultCacheManager
		{
			get { return defaultCacheManager; }
			internal set { defaultCacheManager = value; }
		}
	}
}
