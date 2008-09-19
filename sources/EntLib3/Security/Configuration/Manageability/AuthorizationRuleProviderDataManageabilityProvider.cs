//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Security Application Block
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
using Microsoft.Practices.EnterpriseLibrary.Security.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability
{
	internal class AuthorizationRuleProviderDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<AuthorizationRuleProviderData>
	{
		public const String RulesPropertyName = "rules";

		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder,
			AuthorizationRuleProviderData configurationObject,
			IConfigurationSource configurationSource,
			String elementPolicyKeyName)
		{
			contentBuilder.AddEditTextPart(Resources.AuthorizationRuleProviderRulesPartName,
				RulesPropertyName,
				GenerateRulesString(configurationObject.Rules),
				1024,
				true);
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.AuthorizationProviderPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(AuthorizationRuleProviderData configurationObject, IRegistryKey policyKey)
		{
			String rulesOverride = policyKey.GetStringValue(RulesPropertyName);

			if (rulesOverride != null)
			{
				configurationObject.Rules.Clear();

				Dictionary<String, String> attributesDictionary = new Dictionary<string, string>();
				KeyValuePairParser.ExtractKeyValueEntries(rulesOverride, attributesDictionary);
				foreach (KeyValuePair<String, String> kvp in attributesDictionary)
				{
					configurationObject.Rules.Add(new AuthorizationRuleData(kvp.Key, kvp.Value));
				}
			}
		}

		protected override void GenerateWmiObjects(AuthorizationRuleProviderData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			String[] rules = GenerateRulesArray(configurationObject.Rules);

			wmiSettings.Add(new AuthorizationRuleProviderSetting(configurationObject.Name, rules));
		}

		private static string[] GenerateRulesArray(NamedElementCollection<AuthorizationRuleData> rules)
		{
			String[] rulesArray = new String[rules.Count];
			int i = 0;
			foreach (AuthorizationRuleData rule in rules)
			{
				rulesArray[i++] = KeyValuePairEncoder.EncodeKeyValuePair(rule.Name, rule.Expression);
			}
			return rulesArray;
		}

		private static String GenerateRulesString(NamedElementCollection<AuthorizationRuleData> rules)
		{
			KeyValuePairEncoder encoder = new KeyValuePairEncoder();

			foreach (AuthorizationRuleData ruleData in rules)
			{
				encoder.AppendKeyValuePair(ruleData.Name, ruleData.Expression);
			}

			return encoder.GetEncodedKeyValuePairs();
		}
	}
}