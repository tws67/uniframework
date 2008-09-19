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

using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability
{
	internal class IsolatedStorageCacheStorageDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<IsolatedStorageCacheStorageData>
	{
		public const String PartitionNamePropertyName = "partitionName";

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			IsolatedStorageCacheStorageData configurationObject,
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
			IsolatedStorageCacheStorageData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.IsolatedStorageCacheStorageDataPartitionNamePartName,
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
				return null;	// no policy for these elements
			}
		}
		
		protected override void OverrideWithGroupPolicies(IsolatedStorageCacheStorageData configurationObject, IRegistryKey policyKey)
		{
			String partitionNameOverride = policyKey.GetStringValue(PartitionNamePropertyName);

			configurationObject.PartitionName = partitionNameOverride;
		}

		protected override void GenerateWmiObjects(IsolatedStorageCacheStorageData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new IsolatedStorageCacheStorageSetting(configurationObject.Name,
														configurationObject.PartitionName,
														configurationObject.StorageEncryption));
		}
	}
}
