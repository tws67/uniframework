//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
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
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Formatters
{
	internal class BinaryLogFormatterDataManageabilityProvider
		: ConfigurationElementManageabilityProviderBase<BinaryLogFormatterData>
	{
		protected override void AddElementAdministrativeTemplateParts(AdmContentBuilder contentBuilder, 
			BinaryLogFormatterData configurationObject, 
			IConfigurationSource configurationSource, 
			String elementPolicyKeyName)
		{
			// no parts for formatter
		}

		protected override string ElementPolicyNameTemplate
		{
			get
			{
				return Resources.FormatterPolicyNameTemplate;
			}
		}

		protected override void OverrideWithGroupPolicies(BinaryLogFormatterData configurationObject, IRegistryKey policyKey)
		{
			// no overrides available
		}

		protected override void GenerateWmiObjects(BinaryLogFormatterData configurationObject,
			ICollection<ConfigurationSetting> wmiSettings)
		{
			wmiSettings.Add(new BinaryFormatterSetting(configurationObject.Name));
		}
	}
}
