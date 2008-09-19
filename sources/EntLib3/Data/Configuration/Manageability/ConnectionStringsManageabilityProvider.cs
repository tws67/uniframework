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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
	internal class ConnectionStringsManageabilityProvider
		: ConfigurationSectionManageabilityProviderBase<ConnectionStringsSection>
	{
		public const String ConnectionStringPropertyName = "connectionString";
		public const String ProviderNamePropertyName = "providerName";

		public ConnectionStringsManageabilityProvider(IDictionary<Type, ConfigurationElementManageabilityProvider> subProviders)
			: base(subProviders)
		{ }

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			ConnectionStringsSection configurationSection,
			IConfigurationSource configurationSource,
			String sectionKey)
		{
			contentBuilder.StartCategory(Resources.ConnectionStringsCategoryName);
			{
				foreach (ConnectionStringSettings connectionString in configurationSection.ConnectionStrings)
				{
					contentBuilder.StartPolicy(String.Format(CultureInfo.InvariantCulture,
															Resources.ConnectionStringPolicyNameTemplate,
															connectionString.Name),
						sectionKey + @"\" + connectionString.Name);

					contentBuilder.AddEditTextPart(Resources.ConnectionStringConnectionStringPartName,
						ConnectionStringPropertyName,
						connectionString.ConnectionString,
						500,
						true);

					contentBuilder.AddComboBoxPart(Resources.ConnectionStringProviderNamePartName,
						ProviderNamePropertyName,
						connectionString.ProviderName,
						255,
						true,
						"System.Data.SqlClient",
						"System.Data.OracleClient");

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
			get { return "connectionStrings"; }
		}

		/// <summary>
		/// Overrides the <paramref name="configurationSection"/>'s properties with the Group Policy values from 
		/// the registry.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="policyKey">The <see cref="IRegistryKey"/> which holds the Group Policy overrides.</param>
		protected override void OverrideWithGroupPoliciesForConfigurationSection(ConnectionStringsSection configurationSection,
			IRegistryKey policyKey)
		{
			// no section values to override
		}

		/// <summary>
		/// Creates the <see cref="ConfigurationSetting"/> instances that describe the <paramref name="configurationSection"/>.
		/// </summary>
		/// <param name="configurationSection">The configuration section that must be managed.</param>
		/// <param name="wmiSettings">A collection to where the generated WMI objects are to be added.</param>
		protected override void GenerateWmiObjectsForConfigurationSection(ConnectionStringsSection configurationSection,
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
		protected override void OverrideWithGroupPoliciesAndGenerateWmiObjectsForConfigurationElements(ConnectionStringsSection configurationSection,
			bool readGroupPolicies, IRegistryKey machineKey, IRegistryKey userKey,
			bool generateWmiObjects, ICollection<ConfigurationSetting> wmiSettings)
		{
			List<ConnectionStringSettings> elementsToRemove = new List<ConnectionStringSettings>();

			foreach (ConnectionStringSettings connectionString in configurationSection.ConnectionStrings)
			{
				IRegistryKey machineOverrideKey = null;
				IRegistryKey userOverrideKey = null;

				try
				{
					LoadRegistrySubKeys(connectionString.Name, machineKey, userKey, out machineOverrideKey, out userOverrideKey);

					if (!OverrideWithGroupPoliciesAndGenerateWmiObjectsForConnectionString(connectionString,
							readGroupPolicies, machineOverrideKey, userOverrideKey,
							generateWmiObjects, wmiSettings))
					{
						elementsToRemove.Add(connectionString);
					}
				}
				finally
				{
					ReleaseRegistryKeys(machineOverrideKey, userOverrideKey);
				}
			}

			foreach (ConnectionStringSettings connectionString in elementsToRemove)
			{
				configurationSection.ConnectionStrings.Remove(connectionString);
			}
		}

		private bool OverrideWithGroupPoliciesAndGenerateWmiObjectsForConnectionString(ConnectionStringSettings connectionString,
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
						String connectionStringOverride = policyKey.GetStringValue(ConnectionStringPropertyName);
						String providerNameOverride = policyKey.GetStringValue(ProviderNamePropertyName);

						connectionString.ConnectionString = connectionStringOverride;
						connectionString.ProviderName = providerNameOverride;
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
					new ConnectionStringSetting(connectionString.Name,
						connectionString.ConnectionString,
						connectionString.ProviderName));
			}

			return true;
		}
	}
}
