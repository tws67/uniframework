//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
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
using System.Security.Cryptography;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability
{
	internal class DpapiSymmetricCryptoProviderDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<DpapiSymmetricCryptoProviderData>
	{
		public const String ScopePropertyName = "scope";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			DpapiSymmetricCryptoProviderData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			contentBuilder.AddDropDownListPartForEnumeration<DataProtectionScope>(Resources.DpapiSymmetricProviderScopePartName,
				ScopePropertyName,
				configurationObject.Scope);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.SymmetricCryptoProviderPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(DpapiSymmetricCryptoProviderData configurationObject, IRegistryKey policyKey)
		{
			DataProtectionScope? scopeOverride = policyKey.GetEnumValue<DataProtectionScope>(ScopePropertyName);

			configurationObject.Scope = scopeOverride.Value;
		}

		protected override void GenerateWmiObjects(DpapiSymmetricCryptoProviderData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new DpapiSymmetricCryptoProviderSetting(configurationObject.Name, configurationObject.Scope.ToString()));
		}
	}
}