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
	internal class CacheStorageDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<CacheStorageData>
	{
		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			CacheStorageData configurationObject,
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
			CacheStorageData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			// nothing to set for this part
			contentBuilder.AddTextPart(Resources.NullBackingStoreNoSettingsPartName);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return null;	// no policy for these elements
			}
		}

		protected override void OverrideWithGroupPolicies(CacheStorageData configurationObject, IRegistryKey policyKey)
		{
			// no need for group policies support
		}

		protected override void GenerateWmiObjects(CacheStorageData configurationObject, ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(new NullBackingStoreSetting(configurationObject.Name, configurationObject.StorageEncryption));
		}
	}
}
