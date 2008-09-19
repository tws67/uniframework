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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Manageability
{
	internal class CustomCacheStorageDataManageabilityProvider
		: CustomProviderDataManageabilityProvider<CustomCacheStorageData>
	{
		public new const String ProviderTypePropertyName = CustomProviderDataManageabilityProvider<CustomCacheStorageData>.ProviderTypePropertyName;
		public new const String AttributesPropertyName = CustomProviderDataManageabilityProvider<CustomCacheStorageData>.AttributesPropertyName;

		public CustomCacheStorageDataManageabilityProvider()
			: base("")
		{ }

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			CustomCacheStorageData configurationObject,
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
			CustomCacheStorageData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.CustomProviderTypePartName,
				elementPolicyKeyName,
				ProviderTypePropertyName,
				configurationObject.Type.AssemblyQualifiedName,
				1024,
				true);

			contentBuilder.AddEditTextPart(Resources.CustomProviderAttributesPartName,
				elementPolicyKeyName,
				AttributesPropertyName,
				GenerateAttributesString(configurationObject.Attributes),
				1024,
				false);
		}

		protected override NamedConfigurationSetting CreateSetting(CustomCacheStorageData data,
			String[] attributes)
		{
			return new CustomCacheStorageSetting(data.Name, data.Type.AssemblyQualifiedName, attributes);
		}
	}
}
