//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
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
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
	internal class OracleConnectionSettingsManageabilityProvider
		: ConfigurationSectionManageabilityProviderBase<OracleConnectionSettings>
	{
		public const String PackagesPropertyName = "packages";

		public OracleConnectionSettingsManageabilityProvider(IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
			: base(subProviders)
		{ }

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			OracleConnectionSettings configurationSection,
			IConfigurationSource configurationSource,
			String sectionKey)
		{
			contentBuilder.StartCategory(Resources.OracleConnectionsCategoryName);
			{
				foreach (OracleConnectionData data in configurationSection.OracleConnectionsData)
				{
					String connectionPolicyKey = sectionKey + @"\" + data.Name;

					contentBuilder.StartPolicy(String.Format(CultureInfo.InvariantCulture,
														Resources.OracleConnectionPolicyNameTemplate,
														data.Name),
						connectionPolicyKey);
					{
						contentBuilder.AddEditTextPart(Resources.OracleConnectionPackagesPartName,
							PackagesPropertyName,
							GenerateRulesString(data.Packages),
							1024,
							true);
					}
					contentBuilder.EndPolicy();
				}
			}
			contentBuilder.EndCategory();
		}

		/// <summary>
		/// Gets the name of the category that represents the whole configuration section.
		/// </summary>
		protected override string SectionCategoryName
		{
			get { return Resources.DatabaseCategoryName; }
		}

		/// <summary>
		/// Gets the name of the managed configuration section.
		/// </summary>
		protected override string SectionName
		{
			get { return OracleConnectionSettings.SectionName; }
		}

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s properties with the Group Policy values from 
		/// the registry.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="policyKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides.</param>
		protected override void OverrideWithGroupPoliciesForConfigurationSection(OracleConnectionSettings configurationSection,
			IRegistryKey policyKey)
		{
			// no section values to override
		}

		/// <summary>
		/// Creates the <see cref="ConfigurationSetting"/> instances that describe the <paramref name="configurationSection"/>.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void GenerateWmiObjectsForConfigurationSection(OracleConnectionSettings configurationSection,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			// no wmi objects to generate
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
		protected override void OverrideWithGroupPoliciesAndGenerateWmiObjectsForConfigurationElements(OracleConnectionSettings configurationSection,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			List<OracleConnectionData> connectionsToRemove = new List<OracleConnectionData>();

			foreach (OracleConnectionData connectionData in configurationSection.OracleConnectionsData)
			{
				IRegistryKey machineConnectionKey = null;
				IRegistryKey userConnectionKey = null;

				try
				{
					LoadRegistrySubKeys(connectionData.Name,
						machineKey, userKey,
						out machineConnectionKey, out userConnectionKey);

					if (!OverrideWithGroupPoliciesAndGenerateWmiObjectsForOracleConnection(connectionData,
							readGroupPolicies, machineConnectionKey, userConnectionKey,
							generateWmiObjects, wmiSettings))
					{
						connectionsToRemove.Add(connectionData);
					}
				}
				finally
				{
					ReleaseRegistryKeys(machineConnectionKey, userConnectionKey);
				}
			}

			foreach (OracleConnectionData connectionData in connectionsToRemove)
			{
				configurationSection.OracleConnectionsData.Remove(connectionData.Name);
			}
		}

		private bool OverrideWithGroupPoliciesAndGenerateWmiObjectsForOracleConnection(OracleConnectionData connectionData,
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
						String packagesOverride = policyKey.GetStringValue(PackagesPropertyName);

						connectionData.Packages.Clear();
						Dictionary<String, String> packagesDictionary = new Dictionary<string, string>();
						KeyValuePairParser.ExtractKeyValueEntries(packagesOverride, packagesDictionary);
						foreach (KeyValuePair<String, String> kvp in packagesDictionary)
						{
							connectionData.Packages.Add(new OraclePackageData(kvp.Key, kvp.Value));
						}
					}
					catch (RegistryAccessException ex)
					{
						LogExceptionWhileOverriding(ex);
					}
				}
			}
			if (generateWmiObjects)
			{
				String[] packages = GeneratePackagesArray(connectionData.Packages);

				wmiSettings.Add(new OracleConnectionSetting(connectionData.Name, packages));
			}

			return true;
		}

		private static String[] GeneratePackagesArray(NamedElementCollection<OraclePackageData> packages)
		{
			String[] packagesArray = new String[packages.Count];
			int i = 0;
			foreach (OraclePackageData package in packages)
			{
				packagesArray[i++] = KeyValuePairEncoder.EncodeKeyValuePair(package.Name, package.Prefix);
			}
			return packagesArray;
		}

		private static String GenerateRulesString(NamedElementCollection<OraclePackageData> packages)
		{
			KeyValuePairEncoder encoder = new KeyValuePairEncoder();

			foreach (OraclePackageData packageData in packages)
			{
				encoder.AppendKeyValuePair(packageData.Name, packageData.Prefix);
			}

			return encoder.GetEncodedKeyValuePairs();
		}
	}
}
