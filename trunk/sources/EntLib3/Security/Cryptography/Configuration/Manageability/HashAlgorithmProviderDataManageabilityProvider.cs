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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability
{
	internal class HashAlgorithmProviderDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<HashAlgorithmProviderData>
	{
		public const String SaltEnabledPropertyName = "saltEnabled";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			HashAlgorithmProviderData configurationObject, 
			IConfigurationSource configurationSource, 
			string elementPolicyKeyName)
		{
			contentBuilder.AddCheckboxPart(Resources.HashAlgorithmProviderSaltEnabledPartName,
				SaltEnabledPropertyName,
				configurationObject.SaltEnabled);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.HashProviderPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(HashAlgorithmProviderData configurationObject, IRegistryKey policyKey)
		{
			bool? saltEnabledOverride = policyKey.GetBoolValue(SaltEnabledPropertyName);
			// algorithm type is read only in the configuration tool

			configurationObject.SaltEnabled = saltEnabledOverride.Value;
		}

		protected override void GenerateWmiObjects(HashAlgorithmProviderData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(
				new HashAlgorithmProviderSetting(configurationObject.Name,
					configurationObject.AlgorithmType.AssemblyQualifiedName,
					configurationObject.SaltEnabled));
		}
	}
}