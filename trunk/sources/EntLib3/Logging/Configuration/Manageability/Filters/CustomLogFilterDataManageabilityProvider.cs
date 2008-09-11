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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Filters
{
	internal class CustomLogFilterDataManageabilityProvider
		: CustomProviderDataManageabilityProvider<CustomLogFilterData>
	{
		internal const String TypePropertyName = CustomProviderDataManageabilityProvider<CustomLogFilterData>.ProviderTypePropertyName;
		internal new const String AttributesPropertyName = CustomProviderDataManageabilityProvider<CustomLogFilterData>.AttributesPropertyName;

		public CustomLogFilterDataManageabilityProvider()
			: base(Resources.FilterPolicyNameTemplate)
		{ }

		protected override NamedConfigurationSetting CreateSetting(CustomLogFilterData data,
			String[] attributes)
		{
			return new CustomFilterSetting(data.Name, data.Type.AssemblyQualifiedName, attributes);
		}
	}
}
