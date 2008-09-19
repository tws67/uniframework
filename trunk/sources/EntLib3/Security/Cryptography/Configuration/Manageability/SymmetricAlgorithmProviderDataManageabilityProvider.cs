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
	internal class SymmetricAlgorithmProviderDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<SymmetricAlgorithmProviderData>
	{
		public const String ProtectedKeyFilenamePropertyName = "protectedKeyFilename";
		public const String ProtectedKeyProtectionScopePropertyName = "protectedKeyProtectionScope";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			SymmetricAlgorithmProviderData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.SymmetricAlgorithmProviderKeyFileNamePartName,
				ProtectedKeyFilenamePropertyName,
				configurationObject.ProtectedKeyFilename,
				255,
				true);

			contentBuilder.AddDropDownListPartForEnumeration<DataProtectionScope>(Resources.SymmetricAlgorithmProviderKeyProtectionScopePartName,
				ProtectedKeyProtectionScopePropertyName,
				configurationObject.ProtectedKeyProtectionScope);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.SymmetricCryptoProviderPolicyNameTemplate;
			}
		}
		
		protected override void OverrideWithGroupPolicies(SymmetricAlgorithmProviderData configurationObject, IRegistryKey policyKey)
		{
			String protectedKeyFilenameOverride = policyKey.GetStringValue(ProtectedKeyFilenamePropertyName);
			DataProtectionScope? protectedKeyProtectionScopeOverride
				= policyKey.GetEnumValue<DataProtectionScope>(ProtectedKeyProtectionScopePropertyName);
			// algorithm type is read only in the configuration tool

			configurationObject.ProtectedKeyFilename = protectedKeyFilenameOverride;
			configurationObject.ProtectedKeyProtectionScope = protectedKeyProtectionScopeOverride.Value;
		}

		protected override void GenerateWmiObjects(SymmetricAlgorithmProviderData configurationObject, ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new SymmetricAlgorithmProviderSetting(configurationObject.Name,
					configurationObject.AlgorithmType.AssemblyQualifiedName,
					configurationObject.ProtectedKeyFilename,
					configurationObject.ProtectedKeyProtectionScope.ToString()));
		}
	}
}
