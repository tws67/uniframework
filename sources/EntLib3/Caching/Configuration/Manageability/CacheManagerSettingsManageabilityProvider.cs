//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Caching Application Block
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability
{
	/// <summary>
	/// <para>This type supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
	/// be used directly from your code.</para>
	/// Represents the behavior required to provide Group Policy updates and to publish the <see cref="ConfigurationSetting"/> 
	/// instances associated to the configuration information for the Caching Application Block, and it also manages
	/// the creation of the ADM template categories and policies required to edit Group Policy Objects for the block.
	/// </summary>
	/// <remarks>
	/// This class performs the actual Group Policy update and Wmi object generation for the <see cref="CacheManagerSettings"/>
	/// configuration section and the <see cref="CacheManagerData"/> instances contained by it. Processing for 
	/// <see cref="CacheStorageData"/> and <see cref="StorageEncryptionProviderData"/> instances is delegated to 
	/// <see cref="ConfigurationElementManageabilityProvider"/> objects registered to the configuration object data types.
	/// <para>
	/// The Group Policy directives for the Caching Application Block differ from other block's directives in that policies are
	/// only generated for cache managers, and these policies contain the parts used to override the settings for the
	/// CacheStorageData instance identified by the <see cref="CacheManagerData.CacheStorage">CacheManagerData.CacheStorage</see> 
	/// property and the StorageEncryptionProviderData instance identified by the <see cref="CacheStorageData.StorageEncryption">
	/// CacheStorageData.StorageEncryption</see> property, if any. Manageability providers registered for CacheStorageData and 
	/// StorageEncryptionProviderData subclasses must not generate policies, and the parts they generate must include the
	/// corresponding key name, as they will be included in the cache managers' policies. The purpose for this policy structure
	/// is to make the experience of editing a Group Policy Object's policies for the Caching Application Block similar to that
	/// of the Enterprise Library Configuration Console.
	/// </para>
	/// </remarks>
	/// <seealso cref="ConfigurationSectionManageabilityProvider"/>
	/// <seealso cref="ConfigurationElementManageabilityProvider"/>
	public sealed class CacheManagerSettingsManageabilityProvider
		: ConfigurationSectionManageabilityProviderBase<CacheManagerSettings>
	{
		internal const String DefaultCacheManagerPropertyName = "defaultCacheManager";
		internal const String BackingStoresKeyName = "backingStores";
		internal const String EncryptionProvidersKeyName = "encryptionProviders";

		internal const String CacheManagersKeyName = "cacheManagers";
		internal const String CacheManagerExpirationPollFrequencyInSecondsPropertyName = "expirationPollFrequencyInSeconds";
		internal const String CacheManagerMaximumElementsInCacheBeforeScavengingPropertyName = "maximumElementsInCacheBeforeScavenging";
		internal const String CacheManagerNumberToRemoveWhenScavengingPropertyName = "numberToRemoveWhenScavenging";

		/// <summary>
		/// <para>This method supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
		/// be used directly from your code.
		/// </para>
		/// Initializes a new instance of the <see cref="CacheManagerSettingsManageabilityProvider"/> class with a 
		/// given set of manageability providers to use when dealing with the configuration for cache storage and encryption providers.
		/// </summary>
		/// <param name="subProviders">The mapping from configuration element type to
		/// <see cref="ConfigurationElementManageabilityProvider"/>.</param>
		public CacheManagerSettingsManageabilityProvider(IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
			: base(subProviders)
		{ }

		/// <summary>
		/// <para>This method supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
		/// be used directly from your code.</para>
		/// Adds the ADM instructions that describe the policies that can be used to override the configuration
		/// information for the Caching Application Block.
		/// </summary>
		/// <seealso cref="ConfigurationSectionManageabilityProvider.AddAdministrativeTemplateDirectives(AdmContentBuilder, ConfigurationSection, IConfigurationSource, String)"/>
		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			CacheManagerSettings configurationSection,
			IConfigurationSource configurationSource,
			String sectionKey)
		{
			AddAdministrativeTemplateDirectivesForSection(contentBuilder, configurationSection, sectionKey);
			AddAdministrativeTemplateDirectivesForCacheManagers(contentBuilder, configurationSection, configurationSource, sectionKey);
		}

		/// <summary>
		/// Gets the name of the category that represents the whole configuration section.
		/// </summary>
		protected override string SectionCategoryName
		{
			get { return Resources.CachingSectionCategoryName; }
		}

		/// <summary>
		/// Gets the name of the managed configuration section.
		/// </summary>
		protected override string SectionName
		{
			get { return CacheManagerSettings.SectionName; }
		}

		private void AddAdministrativeTemplateDirectivesForSection(AdmContentBuilder contentBuilder,
			CacheManagerSettings configurationSection,
			String sectionKey)
		{
			contentBuilder.StartPolicy(Resources.CacheManagerSettingsPolicyName, sectionKey);
			{
				contentBuilder.AddDropDownListPartForNamedElementCollection<CacheManagerData>(Resources.CacheManagerSettingsDefaultCacheManagerPartName,
					DefaultCacheManagerPropertyName,
					configurationSection.CacheManagers,
					configurationSection.DefaultCacheManager,
					false);
			}
			contentBuilder.EndPolicy();
		}

		/// <devdoc>
		/// ADM templates for caching are different from the other blocks' templates to match the configuration console's 
		/// user experience. Instead of having separate categories with policies for cache managers, backing stores and 
		/// encryption providers, the policy for a cache manager includes the parts for its backing store and eventual
		/// encryption provider.
		/// </devdoc>
		private void AddAdministrativeTemplateDirectivesForCacheManagers(AdmContentBuilder contentBuilder,
			CacheManagerSettings configurationSection,
			IConfigurationSource configurationSource,
			String sectionKey)
		{
			String cacheManagersKey = sectionKey + @"\" + CacheManagersKeyName;
			String backingStoresKey = sectionKey + @"\" + BackingStoresKeyName;
			String encryptionProvidersKey = sectionKey + @"\" + EncryptionProvidersKeyName;

			contentBuilder.StartCategory(Resources.CacheManagersCategoryName);
			{
				foreach (CacheManagerData cacheManagerData in configurationSection.CacheManagers)
				{
					String cacheManagerPolicyKey = cacheManagersKey + @"\" + cacheManagerData.Name;

					contentBuilder.StartPolicy(String.Format(CultureInfo.InvariantCulture,
															Resources.CacheManagerPolicyNameTemplate,
															cacheManagerData.Name),
						cacheManagerPolicyKey);
					{
						contentBuilder.AddNumericPart(Resources.CacheManagerExpirationPollFrequencyInSecondsPartName,
							CacheManagerExpirationPollFrequencyInSecondsPropertyName,
							cacheManagerData.ExpirationPollFrequencyInSeconds);

						contentBuilder.AddNumericPart(Resources.CacheManagerMaximumElementsInCacheBeforeScavengingPartName,
							CacheManagerMaximumElementsInCacheBeforeScavengingPropertyName,
							cacheManagerData.MaximumElementsInCacheBeforeScavenging);

						contentBuilder.AddNumericPart(Resources.CacheManagerNumberToRemoveWhenScavengingPartName,
							CacheManagerNumberToRemoveWhenScavengingPropertyName,
							cacheManagerData.NumberToRemoveWhenScavenging);

						// append the cache manager's backing store parts
						contentBuilder.AddTextPart(Resources.BackingStoreSettingsPartName);
						CacheStorageData backingStoreData
							= configurationSection.BackingStores.Get(cacheManagerData.CacheStorage);
						ConfigurationElementManageabilityProvider backingStoreDataManageablityProvider
							= GetSubProvider(backingStoreData.GetType());
						AddAdministrativeTemplateDirectivesForElement<CacheStorageData>(contentBuilder,
							backingStoreData, backingStoreDataManageablityProvider,
							configurationSource,
							backingStoresKey);

						// append the backing store's encryption provider parts
						if (!String.IsNullOrEmpty(backingStoreData.StorageEncryption))
						{
							contentBuilder.AddTextPart(Resources.StorageEncryptionProviderSettingsPartName);
							StorageEncryptionProviderData encryptionProviderData
								= configurationSection.EncryptionProviders.Get(backingStoreData.StorageEncryption);
							ConfigurationElementManageabilityProvider encryptionProviderDataManageabilityProvider
								= GetSubProvider(encryptionProviderData.GetType());
							AddAdministrativeTemplateDirectivesForElement<StorageEncryptionProviderData>(contentBuilder,
								encryptionProviderData, encryptionProviderDataManageabilityProvider,
								configurationSource,
								encryptionProvidersKey);
						}
					}
					contentBuilder.EndPolicy();
				}
			}
			contentBuilder.EndCategory();
		}

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s properties with the Group Policy values from 
		/// the registry.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="policyKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides.</param>
		protected override void OverrideWithGroupPoliciesForConfigurationSection(CacheManagerSettings configurationSection,
			IRegistryKey policyKey)
		{
			String defaultCacheManagerOverride = policyKey.GetStringValue(DefaultCacheManagerPropertyName);

			configurationSection.DefaultCacheManager = defaultCacheManagerOverride;
		}

		/// <summary>
		/// Creates the <see cref="ConfigurationSetting"/> instances that describe the <paramref name="configurationSection"/>.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void GenerateWmiObjectsForConfigurationSection(CacheManagerSettings configurationSection,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(new CachingBlockSetting(configurationSection.DefaultCacheManager));
		}

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s configuration elements' properties 
		/// with the Group Policy values from the registry, if any, and creates the <see cref="ConfigurationSetting"/> 
		/// instances that describe these configuration elements.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="readGroupPolicies"><see langword="true"/> if Group Policy overrides must be applied; otherwise, 
		/// <see langword="false"/>.</param>
		/// <param name="machineKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides for the 
		/// configuration section at the machine level, or <see langword="null"/> 
		/// if there is no such registry key.</param>
		/// <param name="userKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides for the 
		/// configuration section at the user level, or <see langword="null"/> 
		/// if there is no such registry key.</param>
		/// <param name="generateWmiObjects"><see langword="true"/> if WMI objects must be generated; otherwise, 
		/// <see langword="false"/>.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void OverrideWithGroupPoliciesAndGenerateWmiObjectsForConfigurationElements(CacheManagerSettings configurationSection,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForCacheManagers(configurationSection.CacheManagers,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.BackingStores,
				BackingStoresKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.EncryptionProviders,
				EncryptionProvidersKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
		}

		private void OverrideWithGroupPoliciesAndGenerateWmiObjectsForCacheManagers(NamedElementCollection<CacheManagerData> cacheManagers,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			List<CacheManagerData> elementsToRemove = new List<CacheManagerData>();

			IRegistryKey machineCacheManagersKey = null;
			IRegistryKey userCacheManagersKey = null;

			try
			{
				LoadRegistrySubKeys(CacheManagersKeyName,
					machineKey, userKey,
					out machineCacheManagersKey, out userCacheManagersKey);

				foreach (CacheManagerData data in cacheManagers)
				{
					IRegistryKey machineCacheManagerKey = null;
					IRegistryKey userCacheManagerKey = null;

					try
					{
						LoadRegistrySubKeys(data.Name,
							machineCacheManagersKey, userCacheManagersKey,
							out machineCacheManagerKey, out userCacheManagerKey);

						if (!OverrideWithGroupPoliciesAndGenerateWmiObjectsForCacheManager(data,
								readGroupPolicies, machineCacheManagerKey, userCacheManagerKey,
								generateWmiObjects, wmiSettings))
						{
							elementsToRemove.Add(data);
						}
					}
					finally
					{
						ReleaseRegistryKeys(machineCacheManagerKey, userCacheManagerKey);
					}
				}
			}
			finally
			{
				ReleaseRegistryKeys(machineCacheManagersKey, userCacheManagersKey);
			}

			foreach (CacheManagerData data in elementsToRemove)
			{
				cacheManagers.Remove(data.Name);
			}
		}

		private bool OverrideWithGroupPoliciesAndGenerateWmiObjectsForCacheManager(CacheManagerData data,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			if (readGroupPolicies)
			{
				IRegistryKey policyKey = machineKey != null ? machineKey : userKey;
				if (policyKey != null)
				{
					if (policyKey.IsPolicyKey && !policyKey.GetBoolValue(PolicyValueName).Value)
					{
						return false;
					}
					try
					{
						// cache storage is not overrideable
						int? expirationPollFrequencyInSecondsOverride
							= policyKey.GetIntValue(CacheManagerExpirationPollFrequencyInSecondsPropertyName);
						int? maximumElementsInCacheBeforeScavengingOverride
							= policyKey.GetIntValue(CacheManagerMaximumElementsInCacheBeforeScavengingPropertyName);
						int? numberToRemoveWhenScavengingOverride
							= policyKey.GetIntValue(CacheManagerNumberToRemoveWhenScavengingPropertyName);

						data.ExpirationPollFrequencyInSeconds = expirationPollFrequencyInSecondsOverride.Value;
						data.MaximumElementsInCacheBeforeScavenging = maximumElementsInCacheBeforeScavengingOverride.Value;
						data.NumberToRemoveWhenScavenging = numberToRemoveWhenScavengingOverride.Value;
					}
					catch (RegistryAccessException ex)
					{
						LogExceptionWhileOverriding(ex);
					}
				}
			}
			if (generateWmiObjects)
			{
				wmiSettings.Add(
					new CacheManagerSetting(data.Name,
								data.CacheStorage,
								data.ExpirationPollFrequencyInSeconds,
								data.MaximumElementsInCacheBeforeScavenging,
								data.NumberToRemoveWhenScavenging));
			}

			return true;
		}
	}
}
