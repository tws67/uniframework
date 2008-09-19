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
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cache.CachingStore.Configuration.Manageability
{
	internal class CachingStoreProviderDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<CachingStoreProviderData>
	{
		public const String CacheManagerPropertyName = "cacheManager";
		public const String AbsoluteExpirationPropertyName = "defaultAbsoluteSessionExpirationInMinutes";
		public const String SlidingExpirationPropertyName = "defaultSlidingSessionExpirationInMinutes";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			CachingStoreProviderData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			CacheManagerSettings cachingConfigurationSection
				= (CacheManagerSettings)configurationSource.GetSection(CacheManagerSettings.SectionName);

			contentBuilder.AddDropDownListPartForNamedElementCollection<CacheManagerData>(Resources.CachingStoreProviderCacheManagerPartName,
				CacheManagerPropertyName,
				cachingConfigurationSection.CacheManagers,
				configurationObject.CacheManager,
				false);

			contentBuilder.AddNumericPart(Resources.CachingStoreProviderAbsoluteExpirationPartName,
				AbsoluteExpirationPropertyName,
				configurationObject.AbsoluteExpiration);

			contentBuilder.AddNumericPart(Resources.CachingStoreProviderSlidingExpirationPartName,
				SlidingExpirationPropertyName,
				configurationObject.SlidingExpiration);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.SecurityCacheProviderPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(CachingStoreProviderData configurationObject, IRegistryKey policyKey)
		{
			String cacheManagerOverride = policyKey.GetStringValue(CacheManagerPropertyName);
			int? absoluteExpirationOverride = policyKey.GetIntValue(AbsoluteExpirationPropertyName);
			int? slidingExpirationOverride = policyKey.GetIntValue(SlidingExpirationPropertyName);

			configurationObject.CacheManager = cacheManagerOverride;
			configurationObject.AbsoluteExpiration = absoluteExpirationOverride.Value;
			configurationObject.SlidingExpiration = slidingExpirationOverride.Value;
		}

		protected override void GenerateWmiObjects(CachingStoreProviderData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new CachingStoreProviderSetting(configurationObject.Name,
					configurationObject.CacheManager,
					configurationObject.AbsoluteExpiration,
					configurationObject.SlidingExpiration));
		}
	}
}
