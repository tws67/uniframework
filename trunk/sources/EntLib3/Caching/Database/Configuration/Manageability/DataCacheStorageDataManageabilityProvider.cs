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
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.Manageability.Properties;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Database.Configuration.Manageability
{
	internal class DataCacheStorageDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<DataCacheStorageData>
	{
		public const String DatabaseInstanceNamePropertyName = "databaseInstanceName";
		public const String PartitionNamePropertyName = "partitionName";

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder, 
			DataCacheStorageData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			// parts for stores are part of their cache manager's policies
			AddElementAdministrativeTemplateParts(contentBuilder, 
				configurationObject, 
				configurationSource, 
				elementPolicyKeyName);
		}

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			DataCacheStorageData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			List<AdmDropDownListItem> connectionStrings = new List<AdmDropDownListItem>();
			ConnectionStringsSection connectionStringsSection
				= (ConnectionStringsSection)configurationSource.GetSection("connectionStrings");
			if (connectionStringsSection != null)
			{
				foreach (ConnectionStringSettings connectionString in connectionStringsSection.ConnectionStrings)
				{
					connectionStrings.Add(new AdmDropDownListItem(connectionString.Name, connectionString.Name));
				}
			}
			contentBuilder.AddDropDownListPart(Resources.DataCacheStorageDatabaseInstanceNamePartName,
				elementPolicyKeyName,
				DatabaseInstanceNamePropertyName,
				connectionStrings,
				configurationObject.DatabaseInstanceName);

			contentBuilder.AddEditTextPart(Resources.DataCacheStoragePartitionNamePartName,
				elementPolicyKeyName,
				PartitionNamePropertyName,
				configurationObject.PartitionName,
				255,
				true);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return null;
			}
		}

		protected override void OverrideWithGroupPolicies(DataCacheStorageData configurationObject, IRegistryKey policyKey)
		{
			String databaseInstanceNameOverride = policyKey.GetStringValue(DatabaseInstanceNamePropertyName);
			String partitionNameOverride = policyKey.GetStringValue(PartitionNamePropertyName);

			configurationObject.DatabaseInstanceName = databaseInstanceNameOverride;
			configurationObject.PartitionName = partitionNameOverride;
		}

		protected override void GenerateWmiObjects(DataCacheStorageData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new DataCacheStorageSetting(configurationObject.Name,
					configurationObject.DatabaseInstanceName,
					configurationObject.PartitionName,
					configurationObject.StorageEncryption));
		}
	}
}
