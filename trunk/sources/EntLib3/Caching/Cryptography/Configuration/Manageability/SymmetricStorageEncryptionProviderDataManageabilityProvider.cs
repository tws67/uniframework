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

using Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Cryptography.Configuration.Manageability
{
	internal class SymmetricStorageEncryptionProviderDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<SymmetricStorageEncryptionProviderData>
	{
		public const String SymmetricInstancePropertyName = "symmetricInstance";

		protected override void AddAdministrativeTemplateDirectives(AdmContentBuilder contentBuilder,
			SymmetricStorageEncryptionProviderData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			// parts for encryption providers are part of their cache manager's policies
			AddElementAdministrativeTemplateParts(contentBuilder, configurationObject, configurationSource, elementPolicyKeyName);
		}

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			SymmetricStorageEncryptionProviderData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			CryptographySettings cryptographySection
				= configurationSource.GetSection("securityCryptographyConfiguration") as CryptographySettings;
			contentBuilder.AddDropDownListPartForNamedElementCollection<SymmetricProviderData>(Resources.SymmetricStorageEncryptionProviderSymmetricInstancePartName,
				elementPolicyKeyName,
				SymmetricInstancePropertyName,
				cryptographySection.SymmetricCryptoProviders,
				configurationObject.SymmetricInstance,
				false);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return null;
			}
		}

		protected override void OverrideWithGroupPolicies(SymmetricStorageEncryptionProviderData configurationObject, IRegistryKey policyKey)
		{
			String symmetricInstanceOverride = policyKey.GetStringValue(SymmetricInstancePropertyName);

			configurationObject.SymmetricInstance = symmetricInstanceOverride;
		}

		protected override void GenerateWmiObjects(SymmetricStorageEncryptionProviderData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(new SymmetricStorageEncryptionProviderSetting(configurationObject.Name, configurationObject.SymmetricInstance));
		}
	}
}
