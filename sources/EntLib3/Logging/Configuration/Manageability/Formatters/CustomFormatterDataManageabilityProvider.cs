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

using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Formatters
{
	internal class CustomFormatterDataManageabilityProvider
		: CustomProviderDataManageabilityProvider<CustomFormatterData>
	{
		internal const String TypePropertyName = CustomProviderDataManageabilityProvider<CustomFormatterData>.ProviderTypePropertyName;
		internal new const String AttributesPropertyName = CustomProviderDataManageabilityProvider<CustomFormatterData>.AttributesPropertyName;

		public CustomFormatterDataManageabilityProvider()
			: base(Resources.FormatterPolicyNameTemplate)
		{ }

		protected override NamedConfigurationSetting CreateSetting(CustomFormatterData data,
			String[] attributes)
		{
			return new CustomFormatterSetting(data.Name, data.Type.AssemblyQualifiedName, attributes);
		}
	}
}
