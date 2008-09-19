//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability
{
	/// <summary>
	/// <para>This type supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
	/// be used directly from your code.</para>
	/// Represents the behavior required to provide Group Policy updates and to publish the <see cref="ConfigurationSetting"/> 
	/// instances associated to the configuration information for the Cryptography Application Block, and it also manages
	/// the creation of the ADM template categories and policies required to edit Group Policy Objects for the block.
	/// </summary>
	/// <remarks>
	/// This class performs the actual Group Policy update and Wmi object generation for the <see cref="CryptographySettings"/>
	/// configuration section. Processing for <see cref="HashProviderData"/> and <see cref="SymmetricProviderData"/> 
	/// instances is delegated to <see cref="ConfigurationElementManageabilityProvider"/> objects registered to the 
	/// configuration object data types.
	/// </remarks>
	/// <seealso cref="ConfigurationSectionManageabilityProvider"/>
	/// <seealso cref="ConfigurationElementManageabilityProvider"/>
	public sealed class CryptographySettingsManageabilityProvider
		: ConfigurationSectionManageabilityProviderBase<CryptographySettings>
	{
		internal const String DefaultHashProviderPropertyName = "defaultHashInstance";
		internal const String DefaultSymmetricCryptoProviderPropertyName = "defaultSymmetricCryptoInstance";
		internal const String HashProvidersKeyName = "hashProviders";
		internal const String SymmetricCryptoProvidersKeyName = "symmetricCryptoProviders";

		/// <summary>
		/// <para>This method supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
		/// be used directly from your code.
		/// </para>
		/// Initializes a new instance of the <see cref="CryptographySettingsManageabilityProvider"/> class with a 
		/// given set of manageability providers to use when dealing with the configuration hash and symmetric cryptography providers.
		/// </summary>
		/// <param name="subProviders">The mapping from configuration element type to
		/// <see cref="ConfigurationElementManageabilityProvider"/>.</param>
		public CryptographySettingsManageabilityProvider(IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
			: base(subProviders)
		{ }

		/// <summary>
		/// <para>This method supports the Enterprise Library Manageability Extensions infrastructure and is not intended to 
		/// be used directly from your code.</para>
		/// Adds the ADM instructions that describe the policies that can be used to override the configuration
		/// information for the Cryptography Application Block.
		/// </summary>
		/// <seealso cref="ConfigurationSectionManageabilityProvider.AddAdministrativeTemplateDirectives(AdmContentBuilder, ConfigurationSection, IConfigurationSource, String)"/>
		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			CryptographySettings configurationSection,
			IConfigurationSource configurationSource,
			String sectionKey)
		{
			contentBuilder.StartPolicy(Resources.CryptographySettingsPolicyName, sectionKey);
			{
				contentBuilder.AddDropDownListPartForNamedElementCollection<HashProviderData>(Resources.CryptographySettingsDefaultHashProviderPartName,
					DefaultHashProviderPropertyName,
					configurationSection.HashProviders,
					configurationSection.DefaultHashProviderName,
					true);

				contentBuilder.AddDropDownListPartForNamedElementCollection<SymmetricProviderData>(Resources.CryptographySettingsDefaultSymmetricCryptoProviderPartName,
					DefaultSymmetricCryptoProviderPropertyName,
					configurationSection.SymmetricCryptoProviders,
					configurationSection.DefaultSymmetricCryptoProviderName,
					true);
			}
			contentBuilder.EndPolicy();

			AddElementsPolicies<HashProviderData>(contentBuilder,
				configurationSection.HashProviders,
				configurationSource,
				sectionKey + @"\" + HashProvidersKeyName,
				Resources.HashProvidersCategoryName);
			AddElementsPolicies<SymmetricProviderData>(contentBuilder,
				configurationSection.SymmetricCryptoProviders,
				configurationSource,
				sectionKey + @"\" + SymmetricCryptoProvidersKeyName,
				Resources.SymmetricCryptoProvidersCategoryName);
		}

		/// <summary>
		/// Gets the name of the category that represents the whole configuration section.
		/// </summary>
		protected override string SectionCategoryName
		{
			get { return Resources.CryptographySectionCategoryName; }
		}

		/// <summary>
		/// Gets the name of the managed configuration section.
		/// </summary>
		protected override string SectionName
		{
			get { return "securityCryptographyConfiguration"; }
		}

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s properties with the Group Policy values from 
		/// the registry.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="policyKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides.</param>
		protected override void OverrideWithGroupPoliciesForConfigurationSection(CryptographySettings configurationSection,
			IRegistryKey policyKey)
		{
			String defaultHashProviderNameOverride
				= GetDefaultProviderPolicyOverride(DefaultHashProviderPropertyName, policyKey);
			String defaultSymmetricCryptoProviderNameOverride
				= GetDefaultProviderPolicyOverride(DefaultSymmetricCryptoProviderPropertyName, policyKey);

			configurationSection.DefaultHashProviderName = defaultHashProviderNameOverride;
			configurationSection.DefaultSymmetricCryptoProviderName = defaultSymmetricCryptoProviderNameOverride;
		}

		private static String GetDefaultProviderPolicyOverride(String propertyName, IRegistryKey policyKey)
		{
			String overrideValue = policyKey.GetStringValue(propertyName);

			return AdmContentBuilder.NoneListItem.Equals(overrideValue) ? String.Empty : overrideValue;
		}

		/// <summary>
		/// Creates the <see cref="ConfigurationSetting"/> instances that describe the <paramref name="configurationSection"/>.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void GenerateWmiObjectsForConfigurationSection(CryptographySettings configurationSection,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new CryptographyBlockSetting(configurationSection.DefaultHashProviderName,
					configurationSection.DefaultSymmetricCryptoProviderName));
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
		protected override void OverrideWithGroupPoliciesAndGenerateWmiObjectsForConfigurationElements(CryptographySettings configurationSection,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.HashProviders,
				HashProvidersKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
			OverrideWithGroupPoliciesAndGenerateWmiObjectsForElementCollection(configurationSection.SymmetricCryptoProviders,
				SymmetricCryptoProvidersKeyName,
				readGroupPolicies, machineKey, userKey,
				generateWmiObjects, wmiSettings);
		}
	}
}